using CefSharp;
using CefSharp.Handler;
using ChromiumWebBrowser.Features.Chromium.Others;

namespace ChromiumWebBrowser.Features.Chromium.ResourceHandlerTest
{
    public class TestResourceRequestHandler : ResourceRequestHandler
    {
        protected override IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return new TestResourceHandler();
        }
    }
}