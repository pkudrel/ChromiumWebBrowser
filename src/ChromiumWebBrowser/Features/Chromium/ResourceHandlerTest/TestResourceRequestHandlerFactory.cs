using System;
using CefSharp;
using CefSharp.Handler;

namespace ChromiumWebBrowser.Features.Chromium.ResourceHandlerTest
{
    public class TestResourceRequestHandlerFactory : IResourceRequestHandlerFactory
    
    {
        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            try
            {
                if (request.ResourceType != ResourceType.MainFrame) return null;

                return new TestResourceRequestHandler();

            }
            finally
            {
                request.Dispose();
            }
        }

        public bool HasHandlers => true;
    }
}