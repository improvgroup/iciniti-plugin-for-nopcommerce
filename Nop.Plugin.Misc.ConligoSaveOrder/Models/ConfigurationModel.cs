using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.ConligoSaveOrder.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        public bool IsConfigured { get; set; }

        /// <summary>
        /// Gets or sets the API URL.
        /// </summary>
        [NopResourceDisplayName("Plugins.Misc.ConligoSaveOrder.Fields.ApiUrl")]
        public string ApiUrl { get; set; }
        public bool ApiUrl_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets the license key.
        /// </summary>
        [NopResourceDisplayName("Plugins.Misc.ConligoSaveOrder.Fields.LicenseKey")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string LicenseKey { get; set; }
        public bool LicenseKey_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        [NopResourceDisplayName("Plugins.Misc.ConligoSaveOrder.Fields.CompanyName")]
        public string CompanyName { get; set; }
        public bool CompanyName_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact.
        /// </summary>
        [NopResourceDisplayName("Plugins.Misc.ConligoSaveOrder.Fields.ContactName")]
        public string ContactName { get; set; }
        public bool ContactName_OverrideForStore { get; set; }

        public bool HideGeneralBlock { get; set; }
    }
}