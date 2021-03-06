﻿using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.ConligoSaveOrder
{
    /// <summary>
    /// Represents the Conligo SaveOrder Miscellaneous plugin is part of an overall system that integrates
    /// NopCommerce web orders into a Sage 300 Backend using custom Conligo provided software. 
    /// </summary>
    public class ConligoSaveOrderPlugin : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public ConligoSaveOrderPlugin(ILocalizationService localizationService,
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
            return $"{_webHelper.GetStoreLocation()}Admin/ConligoSaveOrder/Configure";
        }

        /// <summary>
        /// This method is called during installation of the the plugin and works to setup the appropriate values it needs.
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new ConligoSaveOrderSettings
            {
                ApiUrl = ConligoSaveOrderDefaults.DefaultApiUrl
            });

            //locales
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Misc.ConligoSaveOrder.Configuration"] = "Configuration",
                ["Plugins.Misc.ConligoSaveOrder.Fields.ApiUrl"] = "API URL",
                ["Plugins.Misc.ConligoSaveOrder.Fields.ApiUrl.Hint"] = "Do not change this value unless you are told to do so by an Conligo support representative.",
                ["Plugins.Misc.ConligoSaveOrder.Fields.CompanyName"] = "Company Name",
                ["Plugins.Misc.ConligoSaveOrder.Fields.CompanyName.Hint"] = "Enter the company name exactly as it appears in the e-mail notification you should have received",
                ["Plugins.Misc.ConligoSaveOrder.Fields.ContactName"] = "Contact Name",
                ["Plugins.Misc.ConligoSaveOrder.Fields.ContactName.Hint"] = "Enter the contact name exactly as it appears in the e-mail notification you should have received.",
                ["Plugins.Misc.ConligoSaveOrder.Fields.LicenseKey"] = "License Key",
                ["Plugins.Misc.ConligoSaveOrder.Fields.LicenseKey.Hint"] = "Enter the license key exactly as it appears in the e-mail notification you should have received.",
                ["Plugins.Misc.ConligoSaveOrder.Test"] = "Test Settings"
            });

            base.Install();
        }

        /// <summary>
        /// Uninstalls the plugin and associated settings.
        /// </summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<ConligoSaveOrderSettings>();

            _localizationService.DeletePluginLocaleResources("Plugins.Misc.ConligoSaveOrder");

            base.Uninstall();
        }

        #endregion
    }
}