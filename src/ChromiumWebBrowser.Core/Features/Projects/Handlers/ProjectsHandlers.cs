using System.Threading;
using System.Threading.Tasks;
using ChromiumWebBrowser.Core.App.ReqRes;
using ChromiumWebBrowser.Core.Features.Projects.Services;
using MediatR;
using NLog;

namespace ChromiumWebBrowser.Core.Features.Projects.Handlers
{
    public class ProjectsHandlers : INotificationHandler<AppStartingEvent>
    {
        private readonly IProjectManager _projectManager;
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public ProjectsHandlers(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        public async Task Handle(AppStartingEvent notification, CancellationToken cancellationToken)
        {
            _log.Debug($"AppStartingEvent in ProjectsHandlers");
            _projectManager.Init();
            await Task.CompletedTask;
        }
    }
}