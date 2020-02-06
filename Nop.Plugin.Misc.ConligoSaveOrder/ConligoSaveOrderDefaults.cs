namespace Nop.Plugin.Misc.ConligoSaveOrder
{
    /// <summary>
    /// Represents Conligo plugin constants
    /// </summary>
    public class ConligoSaveOrderDefaults
    {
        /// <summary>
        /// Gets Conligo plugin system name
        /// </summary>
        public static string SystemName => "Misc.ConligoSaveOrder";

        /// <summary>
        /// Gets Conligo service API URL
        /// </summary>
        public static string DefaultApiUrl => "http://staging:8070/Service.svc";

        /// <summary>
        /// Gets generic attribute name to hide general settings block on the plugin configuration page
        /// </summary>
        public static string HideGeneralBlock = "ConligoSaveOrderPage.HideGeneralBlock";
    }
}