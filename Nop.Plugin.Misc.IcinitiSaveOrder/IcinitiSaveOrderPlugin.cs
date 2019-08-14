using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.IcinitiSaveOrder
{
    /// <summary>
    /// Represents the ICINITI SaveOrder Miscellaneous plugin is part of an overall system that integrates
    /// NopCommerce web orders into a Sage 300 Backend using custom ICINITI Corporation provided software. 
    /// </summary>
    public class IcinitiSaveOrderPlugin : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public IcinitiSaveOrderPlugin(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/IcinitiSaveOrder/Configure";
        }

        /// <summary>
        /// This method is called during installation of the the plugin and works to setup the appropriate values it needs.
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new IcinitiSaveOrderSettings
            {
                ApiUrl = IcinitiSaveOrderDefaults.DefaultApiUrl
            });

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Configuration", "Configuration");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.ApiUrl", "API URL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.ApiUrl.Hint", "Do not change this value unless you are told to do so by an ICINITI support representative.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.CompanyName", "Company Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.CompanyName.Hint", "Enter the company name exactly as it appears in the e-mail notification you should have received");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.ContactName", "Contact Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.ContactName.Hint", "Enter the contact name exactly as it appears in the e-mail notification you should have received.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.LicenseKey", "License Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.LicenseKey.Hint", "Enter the license key exactly as it appears in the e-mail notification you should have received.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Test", "Test Settings");

            base.Install();
        }

        /// <summary>
        /// Uninstalls the plugin and associated settings.
        /// </summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<IcinitiSaveOrderSettings>();

            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Configuration");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.ApiUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.ApiUrl.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.CompanyName");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.CompanyName.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.ContactName");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.ContactName.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.LicenseKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Fields.LicenseKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.IcinitiSaveOrder.Test");

            base.Uninstall();
        }

        #endregion
    }
}