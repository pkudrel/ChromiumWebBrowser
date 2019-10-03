using CefSharp;
using ChromiumWebBrowser.Core.Features.Projects.Services;
using ChromiumWebBrowser.Features.Projects.Models;
using NLog;

namespace ChromiumWebBrowser.Core.Features.ResourceRequest.Default.Services
{
    public class DefaultResourceRequestHandlerFactory : IResourceRequestHandlerFactory

    {
        private readonly IResourceRequestHandler _resourceRequestHandler;
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private ProjectRegistry _projectRegistry;


        public DefaultResourceRequestHandlerFactory(IProjectManager projectManager, DefaultResourceRequestHandler resourceRequestHandler)
        {
            _resourceRequestHandler = resourceRequestHandler;
            _projectRegistry = projectManager.CurrentProjectRegistry;
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame,
            IRequest request, bool isNavigation, bool isDownload, string requestInitiator,
            ref bool disableDefaultHandling)
        {
            _log.Debug($"Type: {request.ResourceType}; Url: {request.Url}");
            try
            {
                var aa = _projectRegistry.Config.ResourceRequestTypesAllowed;
                if (request.ResourceType != ResourceType.MainFrame) return null;
                if (request.Url.StartsWith("http") == false) return null;

                //  return new TestResourceRequestHandler();
                return _resourceRequestHandler;
          
            }
            finally
            {
                request.Dispose();
            }
        }

        public bool HasHandlers => true;
    }
}