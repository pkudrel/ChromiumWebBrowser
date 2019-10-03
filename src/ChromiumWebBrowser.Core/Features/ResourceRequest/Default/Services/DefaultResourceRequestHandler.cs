using CefSharp;
using CefSharp.Handler;

namespace ChromiumWebBrowser.Core.Features.ResourceRequest.Default.Services
{
    public class DefaultResourceRequestHandler : ResourceRequestHandler
    {
        private readonly DefaultResourceHandler _defaultResourceHandler;

        public DefaultResourceRequestHandler(DefaultResourceHandler defaultResourceHandler)
        {
            _defaultResourceHandler = defaultResourceHandler;
        }

        protected override IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame, IRequest request)
        {
            return _defaultResourceHandler;
        }
    }
}