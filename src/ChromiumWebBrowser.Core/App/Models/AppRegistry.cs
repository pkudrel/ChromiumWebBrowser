using System.IO;
using ChromiumWebBrowser.Core.App.Bootstrap;
using ChromiumWebBrowser.Core.Common.Io;

namespace ChromiumWebBrowser.Core.App.Models
{
    public class AppRegistry
    {
        public AppRegistry(AppEnvironment env, AppConfig config)
        {
            Env = env;
            WorkDir = config.WorkDir ?? env.WorkDir;
            ProjectPath = config.WorkDir ?? Path.Combine(env.WorkDir, Consts.PROJECTS_DIR);
            Misc.CreateDirIfNotExist(ProjectPath);
            DefaultProject = config.DefaultProject ?? string.Empty;
        }

        public AppEnvironment Env { get; }
        public string WorkDir { get; }
        public string ProjectPath { get; }
        public string DefaultProject { get; }
    }
}