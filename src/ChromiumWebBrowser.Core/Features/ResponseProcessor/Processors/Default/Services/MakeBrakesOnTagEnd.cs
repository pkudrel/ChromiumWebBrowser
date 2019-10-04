namespace ChromiumWebBrowser.Core.Features.ResponseProcessor.Processors.Default.Services
{
    public class MakeBrakesOnTagEnd : IResponseAction
    {
        public string Name => "MakeBrakesOnTagEnd";

        public string Process(string stringIn)
        {
            var result = stringIn.Replace("</", "\r\n</");
            return result;
        }
    }
}