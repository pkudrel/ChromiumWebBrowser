//using System;
//using AngleSharp;
//using AngleSharp.Io;
//using CefSharp;
//using CefSharp.Handler;
//using NLog;

//namespace ChromiumWebBrowser.Features.Chromium.Others
//{
//    public class RequestHandler2 : RequestHandler
//    {
//        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

//        public static readonly string VersionNumberString =
//            $"Chromium: {Cef.ChromiumVersion}, CEF: {Cef.CefVersion}, CefSharp: {Cef.CefSharpVersion}";

//        private readonly Request _request;
        
//        private ProxySettings _proxySettings;

//        public RequestHandler2(ProxySettings proxySettings, Request request)
//        {
//            _proxySettings = proxySettings;
//            _request = request;
//        }

//        public RequestHandler2(Request request)
//        {
//            _request = request;
//            _filterOptions = _navigationOptions.FilterOptions;
//        }
        

//        public override CefReturnValue OnBeforeResourceLoad(
//            IWebBrowser browserControl,
//            IBrowser browser,
//            IFrame frame,
//            IRequest request,
//            IRequestCallback callback)
//        {
//            SetReferrer(request);
//            //SetHeader(request, "User-Agent", Consts.HEADER_USER_AGENT);
//            var cefReturnValue = SetFilter(request);
//            return cefReturnValue;
//        }

//        private CefReturnValue SetFilter(IRequest request)
//        {
//            var url = Url.Create(request.Url);
//            // Exact host to ignore
//            if (_filterOptions.HostsToIgnore.Contains(url.Host)) return CefReturnValue.Cancel;

//            // Resources
//            var resourceTypeRobe = GetResourceTypeRobe(request.ResourceType);
//            if (_filterOptions.ResourcesTypeToIgnore.Contains(resourceTypeRobe)) return CefReturnValue.Cancel;

//            // Path
//            if (_filterOptions.PathsToIgnore.Contains(url.Path)) return CefReturnValue.Cancel;

//            // pathAndQuery
//            var pathAndQuery = $"{url.Path}?{url.Query}";
//            if (_filterOptions.PathsAndQueriesToIgnore.Contains(pathAndQuery)) return CefReturnValue.Cancel;

//            if (_filterOptions.DebugPassUrl) _log.Debug($"Pass url: {url.Href}");


//            return CefReturnValue.Continue;
//        }

//        private ResourceType GetResourceTypeRobe(CefSharp.ResourceType requestResourceType)
//        {
//            switch (requestResourceType)
//            {
//                case CefSharp.ResourceType.MainFrame:
//                    return ResourceType.MainFrame;
//                case CefSharp.ResourceType.SubFrame:
//                    return ResourceType.SubFrame;
//                case CefSharp.ResourceType.Stylesheet:
//                    return ResourceType.Stylesheet;
//                case CefSharp.ResourceType.Script:
//                    return ResourceType.Script;
//                case CefSharp.ResourceType.Image:
//                    return ResourceType.Image;
//                case CefSharp.ResourceType.FontResource:
//                    return ResourceType.FontResource;
//                case CefSharp.ResourceType.SubResource:
//                    return ResourceType.SubResource;
//                case CefSharp.ResourceType.Object:
//                    return ResourceType.Object;
//                case CefSharp.ResourceType.Media:
//                    return ResourceType.Media;
//                case CefSharp.ResourceType.Worker:
//                    return ResourceType.Worker;
//                case CefSharp.ResourceType.SharedWorker:
//                    return ResourceType.SharedWorker;
//                case CefSharp.ResourceType.Prefetch:
//                    return ResourceType.Prefetch;
//                case CefSharp.ResourceType.Favicon:
//                    return ResourceType.Favicon;
//                case CefSharp.ResourceType.Xhr:
//                    return ResourceType.Xhr;
//                case CefSharp.ResourceType.Ping:
//                    return ResourceType.Ping;
//                case CefSharp.ResourceType.ServiceWorker:
//                    return ResourceType.SharedWorker;
//                case CefSharp.ResourceType.CspReport:
//                    return ResourceType.CspReport;
//                case CefSharp.ResourceType.PluginResource:
//                    return ResourceType.PluginResource;
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(requestResourceType), requestResourceType, null);
//            }
//        }

//        private void SetReferrer(IRequest request)
//        {
//            if (string.IsNullOrEmpty(_request.Referer) == false)
//                request.SetReferrer(_request.Referer, ReferrerPolicy.Default);
//        }


//        private static void SetHeader(IRequest request, string key, string value)
//        {
//            var headers = request.Headers;
//            headers[key] = value;
//            request.Headers = headers;
//        }

//        public override bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame,
//            bool isProxy, string host, int port,
//            string realm, string scheme, IAuthCallback callback)
//        {
//            if (_proxySettings != null && _proxySettings.IsEmpty == false)
//            {
//                _log.Debug($"GetAuthCredentials; isProxy: {isProxy}; host: {host}; port: {port}; relm: {realm}");
//                if (isProxy)
//                {
//                    callback.Continue(_proxySettings.UserName, _proxySettings.Password);

//                    return true;
//                }
//            }


//            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
//            return base.GetAuthCredentials(browserControl, browser, frame, isProxy, host, port, realm, scheme,
//                callback);
//            ;
//        }

//        public void SetProxySettings(ProxySettings proxySettings)
//        {
//            _proxySettings = proxySettings;
//        }
//    }
//}