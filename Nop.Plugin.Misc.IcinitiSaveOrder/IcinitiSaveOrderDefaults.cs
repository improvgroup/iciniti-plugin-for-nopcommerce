namespace Nop.Plugin.Misc.IcinitiSaveOrder
{
    /// <summary>
    /// Represents Iciniti plugin constants
    /// </summary>
    public class IcinitiSaveOrderDefaults
    {
        /// <summary>
        /// Gets Iciniti plugin system name
        /// </summary>
        public static string SystemName => "Misc.IcinitiSaveOrder";

        /// <summary>
        /// Gets Iciniti service API URL
        /// </summary>
        public static string DefaultApiUrl => "http://staging:8070/Service.svc";

        /// <summary>
        /// Gets generic attribute name to hide general settings block on the plugin configuration page
        /// </summary>
        public static string HideGeneralBlock = "IcinitiSaveOrderPage.HideGeneralBlock";
    }
}