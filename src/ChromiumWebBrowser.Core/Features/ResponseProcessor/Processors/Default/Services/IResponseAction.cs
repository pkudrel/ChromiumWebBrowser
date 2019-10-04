namespace ChromiumWebBrowser.Core.Features.ResponseProcessor.Processors.Default.Services
{
    public interface IResponseAction
    {

        string Name { get; }
        string Process(string stringIn);
    }
}