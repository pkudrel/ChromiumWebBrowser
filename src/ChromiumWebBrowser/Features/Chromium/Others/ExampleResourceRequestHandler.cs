using System.Collections.Generic;
using System.Text;
using CefSharp;
using CefSharp.Handler;

namespace ChromiumWebBrowser.Features.Chromium.Others
{
    public class ExampleResourceRequestHandler : ResourceRequestHandler
    {
        private readonly Dictionary<ulong, StreamResponseFilter> _responseDictionary =
            new Dictionary<ulong, StreamResponseFilter>();

        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame, IRequest request, IRequestCallback callback)
        {
            var dataFilter = new StreamResponseFilter(request.Url, request.Identifier);
            _responseDictionary.Add(request.Identifier, dataFilter);
            return CefReturnValue.Continue;
        }

        protected override void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, IResponse response, ref string newUrl)
        {
            //Example of how to redirect - need to check `newUrl` in the second pass
            //if (request.Url.StartsWith("https://www.google.com", StringComparison.OrdinalIgnoreCase) && !newUrl.Contains("github"))
            //{
            //    newUrl = "https://github.com";
            //}
        }

        protected override bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request)
        {
            return request.Url.StartsWith("mailto");
        }

        protected override bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, IResponse response)
        {
            //NOTE: You cannot modify the response, only the request
            // You can now access the headers
            //var headers = response.Headers;

            return false;
        }

        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame, IRequest request, IResponse response)
        {

            if (_responseDictionary.TryGetValue(request.Identifier, out var filter))
            {
                var lenght = int.Parse(response.Headers["content-length"]);
                filter.SetLength(lenght);
                return filter;
            }

            return null;
        }



        protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            if (_responseDictionary.TryGetValue(request.Identifier, out var filter))
            {
                //TODO: Do something with the data here
                var data = filter.Data;
                var dataLength = filter.Data.Length;
                //NOTE: You may need to use a different encoding depending on the request
                var dataAsUtf8String = Encoding.UTF8.GetString(data);
                
                filter.Free();
               
                //this.OnResourceLoadComplete?.Invoke(this, new ResourceLoadCompleteEventArgs(arg4.Url, arg4.ResourceType, arg7, stream, arg6));
                var dd = response ;
            }
        }
    }
}