using System;
using System.Collections.Generic;
using ChromiumWebBrowser.Core.Features.ResponseProcessor.Processors.Default.Services;

namespace ChromiumWebBrowser.Core.Features.ResponseProcessor.Services
{
    public interface IResponseProcessorManager
    {
        string Process(string toProcess, List<IResponseAction> actions);
    }


    public class ResponseProcessorManager : IResponseProcessorManager
    {
        public string Process(string toProcess, List<IResponseAction> actions)
        {
            var str = toProcess;
            foreach (var action in actions)
            {
                str = action.Process(str);
            }

            return str;
        }
    }
}