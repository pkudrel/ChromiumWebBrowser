using System.Collections.Generic;

namespace ChromiumWebBrowser.Core.Features.HttpClients.Models
{
    public class DefaultOptions : ClientOptionsBase
    {
        public DefaultOptions()
        {
            Headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(
                    "Accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"),
                new KeyValuePair<string, string>(
                    "Accept-Encoding",
                    "gzip, deflate"),
                new KeyValuePair<string, string>(
                    "User-Agent",
                    Consts.HEADER_USER_AGENT)




            };
        }
    }
}