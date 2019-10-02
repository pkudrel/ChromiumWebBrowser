using System.Net;
using System.Net.Http;

namespace ChromiumWebBrowser.Core.Features.HttpClients.Models
{
    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler()
        {
            UseProxy = true;
            UseCookies = true;
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }
    }
}