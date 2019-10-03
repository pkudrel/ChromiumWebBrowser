using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NLog;

namespace ChromiumWebBrowser.Core.App.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public async Task<TResponse> Handle(
            TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _log.Debug("Calling handler...");
            var response = await next();
            _log.Debug("Called handler with result {0}", response);
            return response;
        }
    }

    public class LoggingBehavior2<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public async Task<TResponse> Handle(
            TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _log.Debug("Calling handler...");
            var response = await next();
            _log.Debug("Called handler with result {0}", response);
            return response;
        }
    }
}