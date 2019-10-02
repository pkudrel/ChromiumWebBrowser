using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using AngleSharp.Io.Network;
using ChromiumWebBrowser.Core.Features.HttpClients.Models;
using ChromiumWebBrowser.Core.Features.WebProxies.Models;
using NLog;

namespace ChromiumWebBrowser.Core.Features.HttpClients
{
    public interface IProxyBrowser
    {
        DenebLabHttpClient HttpClient { get; }
        Task<IDocument> GetDocument(DocumentRequest request, CancellationToken token = default);
        Task<HttpResponseMessage> GetResponseHttpClient(HttpRequestMessage httpRequestMessage, CancellationToken token);
    }

    public class ProxyBrowser : IProxyBrowser
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly AdvanceBrowserSettings _settings;
        private readonly IWebProxyService _webProxyService;
        public readonly IBrowsingContext BrowsingContext;

        public ProxyBrowser(
            DenebLabHttpClient httpClient,
            IWebProxyService webProxyService,
            AdvanceBrowserSettings settings)
        {
            _webProxyService = webProxyService;
            _settings = settings;

            var requester = new HttpClientRequester(httpClient);
            var configuration = Configuration.Default
                .With(requester)
                .WithDefaultLoader(new LoaderOptions {IsResourceLoadingEnabled = true})
                .WithCss()
                .WithCookies();

            BrowsingContext = AngleSharp.BrowsingContext.New(configuration);
            HttpClient = httpClient;
        }


        public Task<IDocument> GetDocument(DocumentRequest request, CancellationToken token = default)
        {
            ConfigureProxyHttpClient();
            DecorateRequest(request);
            var response1 = BrowsingContext.OpenAsync(request, token);
            return response1;
        }

        public async Task<HttpResponseMessage> GetResponseHttpClient(HttpRequestMessage httpRequestMessage,
            CancellationToken token = default)
        {
            ConfigureProxyHttpClient();

            var response = await HttpClient.SendWithOptionsAsync(httpRequestMessage, token);
            return response;
        }

        public DenebLabHttpClient HttpClient { get; }


        private void DecorateRequest(DocumentRequest request)
        {
            foreach (var pair in _settings.Headers)
                if (!request.Headers.ContainsKey(pair.Key))
                    request.Headers.Add(pair.Key, pair.Value);
        }


        private void ConfigureCookiesHttpClient(List<Cookie> cookies)
        {
            HttpClient.RemoveAllCookies();
            foreach (var cookie in cookies) HttpClient.SetCookieSafe(cookie.Name, cookie.Value, "/", cookie.Domain);
        }


        private void ConfigureProxyHttpClient()
        {
            if (_webProxyService.IsEnable && HttpClient.IsWebProxySet() == false)
            {
                var proxy = _webProxyService.GetWebProxy();
                HttpClient.SetWebProxy(proxy);
            }
        }
    }
}