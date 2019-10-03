namespace ChromiumWebBrowser.Features.Projects.Models
{
    public class ProjectRegistry
    {
        public ProjectRegistry(string directoryInfoFullName, ProjectConfig config)
        {
            ProjectPath = directoryInfoFullName;
            Config = config;
            Name = config.Name;
        }

        public ProjectConfig Config { get; }
        public string ProjectPath { get; }
        public string Name { get; }
    }
}