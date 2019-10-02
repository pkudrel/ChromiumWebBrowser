namespace ChromiumWebBrowser.Core.Features.WebProxies.Models
{
    public class ProxySettingsExtend : ProxySettings
    {
        public ProxySettingsExtend(string name, string countryCode)
        {
            Name = name;
            CountryCode = countryCode;
        }

        public ProxySettingsExtend(
            string host,
            int port,
            string userName,
            string password,
            string name,
            string countryCode)
            : base(host, port, userName, password)
        {
            Name = name;
            CountryCode = countryCode;
        }

        public string Name { get; set; }

        public string CountryCode { get; set; }
    }
}