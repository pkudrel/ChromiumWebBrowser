using System.Collections.Generic;

namespace ChromiumWebBrowser.Core.Features.HttpClients.Models
{
    public class AdvanceBrowserSettings
    {
        public List<KeyValuePair<string, string>> Headers { get; set; }
    }
}