using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConligoOrderService;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.ConligoSaveOrder.Services
{
    /// <summary>
    /// Represents Conligo service
    /// </summary>
    public class ConligoService
    {
        #region Fields

        private readonly ConligoSaveOrderSettings _icinitiSaveOrderSettings;
        private readonly IAddressService _addressService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly IOrderService _orderService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ConligoService(ConligoSaveOrderSettings icinitiSaveOrderSettings,
            IAddressService addressService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            ICountryService countryService,
            IOrderService orderService,
            IStateProvinceService stateProvinceService,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            IProductService productService,
            IWorkContext workContext)
        {
            _icinitiSaveOrderSettings = icinitiSaveOrderSettings;
            _addressService = addressService;
            _currencyService = currencyService;
            _customerService = customerService;
            _countryService = countryService;
            _orderService = orderService;
            _stateProvinceService = stateProvinceService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _productService = productService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Obtain authentication token
        /// <param name="settings">Settings; pass null to use the default one</param>
        /// </summary>
        /// <returns>The asynchronous task whose result contains the authentication token</returns>
        private async Task<AuthToken> GetAuthTokenAsync(ConligoSaveOrderSettings settings = null)
        {
            settings ??= _icinitiSaveOrderSettings;

            //initialize the service
            using var service = new ServiceClient(ServiceClient.EndpointConfiguration.BasicHttpBinding_IService, settings.ApiUrl);

            //try to login with the saved credentials and get authentication token
            return await service.LoginAsync(settings.LicenseKey, settings.CompanyName, settings.ContactName);
        }

        /// <summary>
        /// Save order details
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="order">Order details</param>
        /// <returns>The asynchronous task whose result determines that order details successfully saved</returns>
        private async Task SaveOrderDetailsAsync(string token, APIOrder order)
        {
            //initialize the service
            using var service = new ServiceClient(ServiceClient.EndpointConfiguration.BasicHttpBinding_IService, _icinitiSaveOrderSettings.ApiUrl);

            //save order details
            await service.SaveOrderAsync(token, order);
        }

        /// <summary>
        /// Prepare order header
        /// </summary>
        /// <param name="apiHeader">Order header</param>
        /// <param name="order">Order</param>
        private void PrepareOrderHeader(
            APIOrderHeader apiHeader, Order order, Address shippingAddress, Country shippingCountry, StateProvince shippingStateProvince,
            Address billingAddress, Country billingCountry, StateProvince billingStateProvince, Customer customer)
        {
            var orderNotes = _orderService.GetOrderNotesByOrderId(order.Id);
            var orderItems = _orderService.GetOrderItems(order.Id);

            apiHeader.Id = Guid.NewGuid();
            apiHeader.LicenseKey = _icinitiSaveOrderSettings.LicenseKey;
            apiHeader.Status = 'N';
            apiHeader.AmountField = order.OrderTotal;
            apiHeader.BillAddress1 = billingAddress?.Address1;
            apiHeader.BillAddress2 = billingAddress?.Address2;
            apiHeader.BillCity = billingAddress?.City;
            apiHeader.BillContact = $"{billingAddress?.FirstName} {billingAddress?.LastName}";
            apiHeader.BillCountry = $"{billingCountry?.Name}|{billingCountry?.ThreeLetterIsoCode}";
            apiHeader.BillState = $"{billingStateProvince?.Name}|{billingStateProvince?.Abbreviation}";
            apiHeader.BillZip = billingAddress?.ZipPostalCode;
            apiHeader.ContactEmail = customer.Email;
            apiHeader.CurrencyCode = order.CustomerCurrencyCode;
            apiHeader.CustomerCode = customer.Id.ToString();
            apiHeader.CustomerEmail = customer.Email;
            apiHeader.CustomerName = _customerService.GetCustomerFullName(customer);
            apiHeader.DiscountLevel = order.OrderDiscount > 0 ? 1 : 0;
            apiHeader.FaxNum = billingAddress?.FaxNumber;
            apiHeader.MiscCharge = 0;
            apiHeader.NumberOfOrders = 1;
            apiHeader.OnHold = false;
            apiHeader.Options = new List<APIOptionalField>();
            apiHeader.OrderDate = order.PaidDateUtc ?? DateTime.Now;
            apiHeader.OrderNumber = order.CustomOrderNumber;
            apiHeader.OrderTotal = order.OrderTotal;
            apiHeader.OrderType = "???";
            apiHeader.PhoneNum = billingAddress?.PhoneNumber;
            apiHeader.PONumber = order.CustomOrderNumber;
            apiHeader.PriceList = "RETAIL";
            apiHeader.RequestedShipDate = apiHeader.OrderDate.Value.AddDays(7);
            apiHeader.Salesperson1 = "NopCommerce";
            apiHeader.SalesSplit1 = 100;
            apiHeader.SalesSplit2 = 0;
            apiHeader.SalesSplit3 = 0;
            apiHeader.SalesSplit4 = 0;
            apiHeader.SalesSplit5 = 0;
            apiHeader.ShipAddress1 = shippingAddress?.Address1;
            apiHeader.ShipAddress2 = shippingAddress?.Address2;
            apiHeader.ShipCity = shippingAddress?.City;
            apiHeader.ShipCode = order.ShippingMethod;
            apiHeader.ShipContact = $"{shippingAddress?.FirstName} {shippingAddress?.LastName}";
            apiHeader.ShipCountry = $"{shippingCountry?.Name}|{shippingCountry?.ThreeLetterIsoCode}";
            apiHeader.ShipFax = shippingAddress?.FaxNumber;
            apiHeader.ShipPhone = shippingAddress?.PhoneNumber;
            apiHeader.ShipState = $"{shippingStateProvince?.Name}|{shippingStateProvince?.Abbreviation}";
            apiHeader.SpecialInstruction = orderNotes.Aggregate(string.Empty, (message, note) => $"{message}{Environment.NewLine}{note.Note}");
            apiHeader.SubTotal = order.OrderSubtotalExclTax;
            apiHeader.Tax1 = order.OrderTax;
            apiHeader.Tax2 = 0;
            apiHeader.Tax3 = 0;
            apiHeader.Tax4 = 0;
            apiHeader.Tax5 = 0;
            apiHeader.TaxGroup = order.TaxRates;
            apiHeader.TaxStatus1 = 1;
            apiHeader.TaxStatus2 = 0;
            apiHeader.TaxStatus3 = 0;
            apiHeader.TaxStatus4 = 0;
            apiHeader.TaxStatus5 = 0;
            apiHeader.TotalDiscountAmount = order.OrderDiscount;
            apiHeader.TotalItems = orderItems.Count;
            apiHeader.TotalShippingAmount = order.OrderShippingExclTax;
            apiHeader.TotalTax = order.OrderTax;
        }

        /// <summary>
        /// Prepare order customer
        /// </summary>
        /// <param name="apiCustomer">Order customer</param>
        /// <param name="order">Order</param>
        /// <param name="headerId">Order header id</param>
        private void PrepareOrderCustomer(
            APICustomer apiCustomer, Guid headerId, Address billingAddress, 
            Country billingCountry, StateProvince billingStateProvince, Customer customer)
        {
            apiCustomer.Id = Guid.NewGuid();
            apiCustomer.HeaderId = headerId;
            apiCustomer.AccountSetCode = customer.Id.ToString();
            apiCustomer.AccountType = 0;
            apiCustomer.BillingAddrCity = billingAddress?.City;
            apiCustomer.BillingAddrCountryName = $"{billingCountry?.Name}|{billingCountry?.ThreeLetterIsoCode}";
            apiCustomer.BillingAddrLine1 = billingAddress?.Address1;
            apiCustomer.BillingAddrLine2 = billingAddress?.Address2;
            apiCustomer.BillingAddrPostalCode = billingAddress?.ZipPostalCode;
            apiCustomer.BillingAddrStateName = $"{billingStateProvince?.Name}|{billingStateProvince?.Abbreviation}";
            apiCustomer.BillingContactName = _customerService.GetCustomerFullName(customer);
            apiCustomer.BillingFaxNumber = billingAddress?.FaxNumber;
            apiCustomer.BillingTelNumber = billingAddress?.PhoneNumber;
            apiCustomer.ContactEmail = customer.Email;
            apiCustomer.ContactFax = billingAddress?.FaxNumber;
            apiCustomer.ContactPhone = billingAddress?.PhoneNumber;
            apiCustomer.CreditLimit = 0;
            apiCustomer.CurrencyCode = _currencyService.GetCurrencyById(_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CurrencyIdAttribute))?.CurrencyCode;
            apiCustomer.CustomerCode = customer.Id.ToString();
            apiCustomer.CustomerEmail = customer.Email;
            apiCustomer.CustomerName = _customerService.GetCustomerFullName(customer);
            apiCustomer.CustomerShortName = customer.Username;
            apiCustomer.GroupCode = customer.AffiliateId.ToString();
            apiCustomer.IsActive = customer.Active ? (short)1 : (short)0;
            apiCustomer.IsOnHold = 0;
            apiCustomer.Options = new List<APIOptionalField>();
            apiCustomer.PartShip = 0;
            apiCustomer.PriceListCode = "WEB";
            apiCustomer.SalespersonCode1 = "nopCommerce";
            apiCustomer.SalesSplitPercentage1 = 100;
            apiCustomer.SalesSplitPercentage2 = 0;
            apiCustomer.SalesSplitPercentage3 = 0;
            apiCustomer.SalesSplitPercentage4 = 0;
            apiCustomer.SalesSplitPercentage5 = 0;
            apiCustomer.ShopOnWeb = 1;
            apiCustomer.TaxClassCode1 = customer.IsTaxExempt ? 0 : 1;
            apiCustomer.TaxClassCode2 = 0;
            apiCustomer.TaxClassCode3 = 0;
            apiCustomer.TaxClassCode4 = 0;
            apiCustomer.TaxClassCode5 = 0;
            apiCustomer.TaxGroupCode = customer.IsTaxExempt ? "EXEMPT" : "TAX";
            apiCustomer.TaxRegistrationNo1 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute);
        }

        /// <summary>
        /// Prepare order shipping address
        /// </summary>
        /// <param name="apiShipTo">Order address</param>
        /// <param name="order">Order</param>
        /// <param name="headerId">Order header id</param>
        private void PrepareOrderAddress(
            APIAddress apiShipTo, Order order, Guid headerId, Address shippingAddress, 
            Country shippingCountry, StateProvince shippingStateProvince, Customer customer)
        {
            var orderNotes = _orderService.GetOrderNotesByOrderId(order.Id);

            apiShipTo.Id = Guid.NewGuid();
            apiShipTo.HeaderId = headerId;
            apiShipTo.IsActive = 1;
            apiShipTo.AddrCity = shippingAddress?.City;
            apiShipTo.AddrCountryName = $"{shippingCountry?.Name}|{shippingCountry?.ThreeLetterIsoCode}";
            apiShipTo.AddrLine1 = shippingAddress?.Address1;
            apiShipTo.AddrLine2 = shippingAddress?.Address2;
            apiShipTo.AddrPostalCode = shippingAddress?.ZipPostalCode;
            apiShipTo.AddrStateName = $"{shippingStateProvince?.Name}|{shippingStateProvince?.Abbreviation}";
            apiShipTo.ContactEmail = customer.Email;
            apiShipTo.ContactFax = shippingAddress?.FaxNumber;
            apiShipTo.ContactName = $"{shippingAddress?.FirstName} {shippingAddress?.LastName}";
            apiShipTo.ContactPhone = shippingAddress?.PhoneNumber;
            apiShipTo.CustomerCode = customer.Username;
            apiShipTo.CustomerEmail = customer.Email;
            apiShipTo.FaxNumber = shippingAddress?.FaxNumber;
            apiShipTo.Fob = shippingAddress?.City;
            apiShipTo.IsActive = 1;
            apiShipTo.LocationName = $"{shippingAddress?.FirstName} {shippingAddress?.LastName}";
            apiShipTo.Options = new List<APIOptionalField>();
            apiShipTo.PriceListCode = "WEB";
            apiShipTo.SalespersonCode1 = "nopCommerce";
            apiShipTo.SalesSplitPercentage1 = 100;
            apiShipTo.SalesSplitPercentage2 = 0;
            apiShipTo.SalesSplitPercentage3 = 0;
            apiShipTo.SalesSplitPercentage4 = 0;
            apiShipTo.SalesSplitPercentage5 = 0;
            apiShipTo.SpecialInstructions = orderNotes.Aggregate(string.Empty, (message, note) => $"{message}{Environment.NewLine}{note.Note}");
            apiShipTo.TaxClassCode1 = order.TaxRates;
            apiShipTo.TaxGroupCode = customer.IsTaxExempt ? "EXEMPT" : string.Empty;
            apiShipTo.TaxRegistrationNo1 = !string.IsNullOrEmpty(order.VatNumber) ? $"VatNumber: {order.VatNumber}" : string.Empty;
            apiShipTo.TelNumber = shippingAddress?.PhoneNumber;
            apiShipTo.WarehouseLocationCode = $"StoreId: {order.StoreId}";
        }

        /// <summary>
        /// Prepare order details
        /// </summary>
        /// <param name="apiDetails">Order details</param>
        /// <param name="order">Order</param>
        /// <param name="headerId">Order header id</param>
        private void PrepareOrderDetails(List<APIDetail> apiDetails, Order order, Guid headerId)
        {
            var orderItems = _orderService.GetOrderItems(order.Id);
            for (var i = 0; i < orderItems.Count; i++)
            {
                var item = orderItems[i];
                var product = _productService.GetProductById(item.ProductId);
                apiDetails.Add(new APIDetail()
                {
                    MiscCharge = null,
                    RegularItem = new APIRegularItem()
                    {
                        Id = Guid.NewGuid(),
                        HeaderId = headerId,
                        ItemCode = _productService.FormatSku(product, item.AttributesXml),
                        ItemDescription = product?.Name,
                        ItemDiscount = item.DiscountAmountExclTax,
                        ItemExtDiscount = item.DiscountAmountExclTax * item.Quantity,
                        ItemLineNumber = i + 1,
                        ItemPricingUnitPrice = item.UnitPriceExclTax,
                        ItemQuantity = item.Quantity,
                        ItemShipCost = 0,
                        ItemTax1 = item.PriceInclTax - item.PriceExclTax,
                        ItemTax2 = 0,
                        ItemTax3 = 0,
                        ItemTax4 = 0,
                        ItemTax5 = 0,
                        ItemTotalTax = 0,
                        ItemType = 1,
                        ItemWeight = 0,
                        Options = new List<APIOptionalField>(),
                        OrderingUOM = "EA",
                        OrderNumber = order.CustomOrderNumber,
                        PriceOverride = 0,
                        PricingUOM = "EA"
                    }
                });
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Verify credentials
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <returns>Result and message</returns>
        public (bool isValid, string message) VerifyCredentials(ConligoSaveOrderSettings settings)
        {
            try
            {
                //obtain auth token
                var authToken = GetAuthTokenAsync(settings).Result;

                //check whether token is valid
                if (authToken?.IsValid ?? false)
                {
                    _logger.Information($"Conligo Plugin. Test passed -> Token: {authToken.Token}", customer: _workContext.CurrentCustomer);
                    return (true, "Connected successfully");
                }

                _logger.Error("Conligo Plugin. Test failed -> Unable to get a token", customer: _workContext.CurrentCustomer);
                return (false, "Failed to get a token");
            }
            catch (Exception exception)
            {
                //log errors
                _logger.Error("Conligo Plugin. Error", exception, _workContext.CurrentCustomer);
            }

            return (default(bool), default(string));
        }

        /// <summary>
        /// Handle placed order event
        /// </summary>
        /// <param name="order">Order</param>
        public void HandleOrderPlacedEvent(Order order)
        {
            try
            {
                if (order == null)
                    throw new NopException("Unable to load order");

                if (_customerService.GetCustomerById(order.CustomerId) == null)
                    throw new NopException("Unable to load customer");

                _logger.Information($"Conligo Plugin. Saving Order: {order.CustomOrderNumber}", customer: _workContext.CurrentCustomer);

                //obtain auth token
                var authToken = GetAuthTokenAsync().Result;
                if (authToken?.IsValid ?? false)
                    _logger.Information($"Conligo Plugin. Authentication Successfull -> Order: {order.CustomOrderNumber}; Token: {authToken.Token}", customer: _workContext.CurrentCustomer);
                else
                    _logger.Information($"Conligo Plugin. Authentication Failed -> Order: {order.CustomOrderNumber}", customer: _workContext.CurrentCustomer);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);
                var billingCountry = _countryService.GetCountryById(billingAddress.CountryId ?? 0);
                var billingStateProvince = _stateProvinceService.GetStateProvinceById(billingAddress.StateProvinceId ?? 0);

                // get available shipping address
                Address shippingAddress = null;
                Country shippingCountry = null;
                StateProvince shippingStateProvince = null;

                if (order.PickupInStore && order.PickupAddressId.HasValue)
                    shippingAddress = _addressService.GetAddressById(order.PickupAddressId.Value);

                if (shippingAddress == null && order.ShippingAddressId.HasValue)
                    shippingAddress = _addressService.GetAddressById(order.ShippingAddressId.Value);

                if (shippingAddress == null)
                {
                    shippingAddress = billingAddress;
                    shippingCountry = billingCountry;
                    shippingStateProvince = billingStateProvince;
                }
                else
                {
                    shippingCountry = _countryService.GetCountryById(shippingAddress.CountryId ?? 0);
                    shippingStateProvince = _stateProvinceService.GetStateProvinceById(shippingAddress.StateProvinceId ?? 0);
                }

                var customer = _customerService.GetCustomerById(order.CustomerId);

                //prepare order details
                var apiOrder = new APIOrder();

                var apiHeader = new APIOrderHeader();
                PrepareOrderHeader(apiHeader, order, shippingAddress, shippingCountry, shippingStateProvince, 
                    billingAddress, billingCountry, billingStateProvince, customer);
                apiOrder.Header = apiHeader;

                var apiCustomer = new APICustomer();
                PrepareOrderCustomer(apiCustomer, apiHeader.Id, billingAddress, billingCountry, billingStateProvince, customer);
                apiOrder.Customer = apiCustomer;

                var apiShipTo = new APIAddress();
                PrepareOrderAddress(apiShipTo, order, apiHeader.Id, shippingAddress, shippingCountry, shippingStateProvince, customer);
                apiOrder.ShipTo = apiShipTo;

                var apiDetails = new List<APIDetail>();
                PrepareOrderDetails(apiDetails, order, apiHeader.Id);
                apiOrder.Details = apiDetails;

                //save order details
                SaveOrderDetailsAsync(authToken.Token, apiOrder).Wait();

                _logger.Information($"Conligo Plugin. Order successfully saved: {order.CustomOrderNumber}", customer: _workContext.CurrentCustomer);

            }
            catch (Exception exception)
            {
                //log errors
                _logger.Error("Conligo Plugin. Error", exception, _workContext.CurrentCustomer);
            }
        }

        #endregion
    }
}