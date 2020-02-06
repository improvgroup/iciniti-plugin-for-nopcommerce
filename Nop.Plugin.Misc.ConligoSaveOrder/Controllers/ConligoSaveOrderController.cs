using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.ConligoSaveOrder.Models;
using Nop.Plugin.Misc.ConligoSaveOrder.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.ConligoSaveOrder.Controllers
{
    public class ConligoSaveOrderController : BasePluginController
    {
        #region Fields

        private readonly ConligoService _icinitiService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ConligoSaveOrderController(ConligoService icinitiService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _icinitiService = icinitiService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            //load settings for active store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var icinitiSettings = _settingService.LoadSetting<ConligoSaveOrderSettings>(storeId);

            //prepare model
            var model = new ConfigurationModel
            {
                ApiUrl = icinitiSettings.ApiUrl,
                LicenseKey = icinitiSettings.LicenseKey,
                CompanyName = icinitiSettings.CompanyName,
                ContactName = icinitiSettings.ContactName,
                ActiveStoreScopeConfiguration = storeId
            };

            //prepare panel visibility
            model.HideGeneralBlock = _genericAttributeService
                .GetAttribute<bool>(_workContext.CurrentCustomer, ConligoSaveOrderDefaults.HideGeneralBlock);

            //overridable settings
            if (storeId > 0)
            {
                model.ApiUrl_OverrideForStore = _settingService.SettingExists(icinitiSettings, settings => settings.ApiUrl, storeId);
                model.LicenseKey_OverrideForStore = _settingService.SettingExists(icinitiSettings, settings => settings.LicenseKey, storeId);
                model.CompanyName_OverrideForStore = _settingService.SettingExists(icinitiSettings, settings => settings.CompanyName, storeId);
                model.ContactName_OverrideForStore = _settingService.SettingExists(icinitiSettings, settings => settings.ContactName, storeId);
            }

            //whether plugin is configured
            model.IsConfigured = !string.IsNullOrEmpty(icinitiSettings.ApiUrl)
                && !string.IsNullOrEmpty(icinitiSettings.LicenseKey)
                && !string.IsNullOrEmpty(icinitiSettings.CompanyName)
                && !string.IsNullOrEmpty(icinitiSettings.ContactName);

            return View("~/Plugins/Misc.ConligoSaveOrder/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for active store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var icinitiSettings = _settingService.LoadSetting<ConligoSaveOrderSettings>(storeId);

            //save settings
            icinitiSettings.ApiUrl = model.ApiUrl;
            icinitiSettings.LicenseKey = model.LicenseKey;
            icinitiSettings.CompanyName = model.CompanyName;
            icinitiSettings.ContactName = model.ContactName;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(icinitiSettings, settings => settings.ApiUrl, model.ApiUrl_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(icinitiSettings, settings => settings.LicenseKey, model.LicenseKey_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(icinitiSettings, settings => settings.CompanyName, model.CompanyName_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(icinitiSettings, settings => settings.ContactName, model.ContactName_OverrideForStore, storeId, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("test")]
        public IActionResult Test(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for active store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var icinitiSettings = _settingService.LoadSetting<ConligoSaveOrderSettings>(storeId);

            var (isValid, message) = _icinitiService.VerifyCredentials(icinitiSettings);
            if (isValid)
                _notificationService.SuccessNotification(message);
            else
                _notificationService.ErrorNotification(message);

            return Configure();
        }

        #endregion
    }
}