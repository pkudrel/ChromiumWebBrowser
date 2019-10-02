namespace ChromiumWebBrowser.Core.Features.WebProxies.Models
{
    public class ProxySettings
    {
        public ProxySettings()
        {
            IsEmpty = true;
        }

        public ProxySettings(string host, int port, string userName, string password)
        {
            Host = host;
            Port = port;
            UserName = userName;
            Password = password;
            IsEmpty = false;
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Address
        {
            get
            {
                if (string.IsNullOrEmpty(Host) || Port == 0)
                    return string.Empty;
                return $"{Host}:{Port}";
            }
        }

        public bool IsEmpty { get; set; }
    }
}