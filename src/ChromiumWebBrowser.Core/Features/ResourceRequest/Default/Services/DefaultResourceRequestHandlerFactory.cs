using System;
using System.Collections.Generic;
using AngleSharp;
using CefSharp;
using ChromiumWebBrowser.Core.Features.Projects.Models;
using ChromiumWebBrowser.Core.Features.Projects.Services;
using ChromiumWebBrowser.Core.Features.ResourceRequest.Models;
using ChromiumWebBrowser.Features.Projects.Models;
using NLog;

namespace ChromiumWebBrowser.Core.Features.ResourceRequest.Default.Services
{
    public class DefaultResourceRequestHandlerFactory : IResourceRequestHandlerFactory

    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ProjectConfig _config;

        private readonly Func<ProjectRegistry, List<string>, DefaultResourceRequestHandler>
            _createDefaultResourceRequestHandlerFn;

        private readonly ProjectRegistry _projectRegistry;
        private readonly IUrlRuleService _urlRuleService;

        public DefaultResourceRequestHandlerFactory(
            IProjectManager projectManager,
            IUrlRuleService urlRuleService,
            Func<ProjectRegistry, List<string>, DefaultResourceRequestHandler> createDefaultResourceRequestHandlerFn
        )
        {
            _urlRuleService = urlRuleService;
            _createDefaultResourceRequestHandlerFn = createDefaultResourceRequestHandlerFn;

            _projectRegistry = projectManager.CurrentProjectRegistry;
            _config = projectManager.CurrentProjectRegistry.Config;
        }


        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame,
            IRequest request, bool isNavigation,
            bool isDownload,
            string requestInitiator,
            ref bool disableDefaultHandling)
        {
            _log.Debug($"Type: {request.ResourceType}; Url: {request.Url}");
            try
            {
                var url = Url.Create(request.Url);
                if (_config.ResourceRequestTypesAllowed.Contains(request.ResourceType) == false) return null;
                if (_config.ResourceRequestProtocolsAllowed.Contains(url.Scheme) == false) return null;
                var matchUrl = _urlRuleService.Match(_config.ResourceRequestUrlsActionsAllowed, url);
                if (matchUrl.Match == false) return null;
                var defaultResourceRequestHandler =
                    _createDefaultResourceRequestHandlerFn(_projectRegistry, matchUrl.Actions);
                return defaultResourceRequestHandler;
            }
            finally
            {
                request.Dispose();
            }
        }

        public bool HasHandlers => true;
    }
}