using System;
using CefSharp;
using CefSharp.Handler;
using NLog;

namespace ChromiumWebBrowser.Features.Chromium.ResourceHandlerTest
{
    public class TestResourceRequestHandlerFactory : IResourceRequestHandlerFactory
    
    {

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {

            _log.Debug($"Type: {request.ResourceType}; Url: {request.Url}");
            try
            {
                if (request.ResourceType != ResourceType.MainFrame) return null;
                if (request.Url.StartsWith("http") == false ) return null;
               
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