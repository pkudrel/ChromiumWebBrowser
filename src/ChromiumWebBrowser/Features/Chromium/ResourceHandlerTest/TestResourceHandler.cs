using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace ChromiumWebBrowser.Features.Chromium.ResourceHandlerTest
{
    public class TestResourceHandler : ResourceHandler
    {
        public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
        {
            Task.Run(() =>
            {
                using (callback)
                {
                    var httpWebRequest =
                        (HttpWebRequest)WebRequest.Create(request.Url);

                    var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    // Get the stream associated with the response.
                    var receiveStream = httpWebResponse.GetResponseStream();
                    var mime = httpWebResponse.ContentType;

                    var stream2 = new MemoryStream();
                    if (receiveStream != null) receiveStream.CopyTo(stream2);
                    httpWebResponse.Close();

                    //Reset the stream position to 0 so the stream can be copied into the underlying unmanaged buffer
                    stream2.Position = 0;

                    var dataAsUtf8String = Encoding.UTF8.GetString(stream2.ToArray());
                    stream2.Dispose();
                    var newData = dataAsUtf8String.Replace("test", "best");
                    byte[] byteArray = Encoding.UTF8.GetBytes(newData);
                    MemoryStream stream = new MemoryStream(byteArray);
                    //Populate the response values - No longer need to implement GetResponseHeaders (unless you need to perform a redirect)

                    stream.Position = 0;
                    ResponseLength = stream.Length;
                    MimeType = mime;
                    StatusCode = (int)HttpStatusCode.OK;
                    Stream = stream;

                    callback.Continue();
                }
            });

            return CefReturnValue.ContinueAsync;
        }
    }
}