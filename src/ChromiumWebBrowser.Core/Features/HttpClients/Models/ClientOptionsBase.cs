using System.Collections.Generic;

namespace ChromiumWebBrowser.Core.Features.HttpClients.Models
{
    public abstract class ClientOptionsBase
    {
        public List<KeyValuePair<string, string>> Headers { get; set; }
    }
}