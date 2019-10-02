namespace ChromiumWebBrowser.Core.Features.HttpClients.Models
{
    public class Cookie
    {
        public Cookie(string domain, string path, string name, string value)
        {
            Domain = domain;
            Path = path;
            Name = name;
            Value = value;
        }

        public string Domain { get; }

        public string Path { get; }

        public string Name { get; }

        public string Value { get; }
    }
}