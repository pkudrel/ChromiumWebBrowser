using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using ChromiumWebBrowser.Core.Features.HttpClients;
using ChromiumWebBrowser.Core.Features.ResponseProcessor.Processors.Default.Services;
using ChromiumWebBrowser.Core.Features.ResponseProcessor.Services;
using ChromiumWebBrowser.Features.Projects.Models;

namespace ChromiumWebBrowser.Core.Features.ResourceRequest.Default.Services
{
    public class DefaultResourceHandler : ResourceHandler
    {
        private readonly IProxyBrowser _browser;
        private readonly IResponseProcessorManager _processorManager;
        private readonly ProjectRegistry _projectRegistry;
        private readonly List<IResponseAction> _responseActions;

        public DefaultResourceHandler(IProxyBrowser browser,
            IResponseProcessorManager processorManager,
            ProjectRegistry projectRegistry,
            List<IResponseAction> responseActions,
            string mimeType = "text/html", Stream stream = null,
            bool autoDisposeStream = false) : base(mimeType, stream, autoDisposeStream)
        {
            _browser = browser;
            _processorManager = processorManager;
            _projectRegistry = projectRegistry;
            _responseActions = responseActions;
           
        }

        public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
        {
            Task.Run(async () =>
            {
                using (callback)
                {

                    string mime;
                    MemoryStream responseStream;
                    using (var httpWebResponse =
                        await _browser.HttpClient.SendWithOptionsAsync(new HttpRequestMessage(HttpMethod.Get,
                            request.Url)))
                    {
                        using (var receiveStream = await httpWebResponse.Content.ReadAsStreamAsync())
                        {
                            mime = httpWebResponse.Content.Headers.ContentType.MediaType;
                            responseStream = new MemoryStream();
                            receiveStream?.CopyTo(responseStream);
                        }
                    }


                    //Reset the stream position to 0 so the stream can be copied into the underlying unmanaged buffer
                    responseStream.Position = 0;

                    var dataAsUtf8String = Encoding.UTF8.GetString(responseStream.ToArray());
                    responseStream.Dispose();

                    var newData = _processorManager.Process(dataAsUtf8String, _responseActions);
                    //var newData = dataAsUtf8String.Replace("test", "best");
                    var byteArray = Encoding.UTF8.GetBytes(newData);
                    var stream = new MemoryStream(byteArray) {Position = 0};
                    //Populate the response values - No longer need to implement GetResponseHeaders (unless you need to perform a redirect)

                    ResponseLength = stream.Length;
                    MimeType = mime;
                    StatusCode = (int) HttpStatusCode.OK;
                    Stream = stream;

                    callback.Continue();
                }
            });

            return CefReturnValue.ContinueAsync;
        }
    }
}