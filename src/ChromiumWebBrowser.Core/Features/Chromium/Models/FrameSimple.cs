namespace ChromiumWebBrowser.Core.Features.Chromium.Models
{
    public class FrameSimple
    {
        public FrameSimple()
        {
        }

        public FrameSimple(string url, string source)
        {
            Url = url;
            Source = source;
        }

        public string Source { get; set; }
        public string Url { get; set; }
    }
}