using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.ConligoSaveOrder
{
    /// <summary>
    /// Represents class that is a simple model-container for the key values that the plugin requires.
    /// </summary>
    public class ConligoSaveOrderSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the API URL.
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the license key.
        /// </summary>
        public string LicenseKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact.
        /// </summary>
        public string ContactName { get; set; }
    }
}