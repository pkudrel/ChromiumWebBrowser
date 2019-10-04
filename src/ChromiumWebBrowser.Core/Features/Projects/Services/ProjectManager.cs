using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CefSharp;
using ChromiumWebBrowser.Core.App.Models;
using ChromiumWebBrowser.Core.Features.Projects.Models;
using ChromiumWebBrowser.Features.Projects.Models;
using Newtonsoft.Json;

namespace ChromiumWebBrowser.Core.Features.Projects.Services
{
    public interface IProjectManager
    {
        void Init();
        ProjectRegistry CurrentProjectRegistry { get; }
    }



    public class ProjectManager : IProjectManager
    {
        private readonly List<ProjectRegistry> _projectRegistries = new List<ProjectRegistry>();
        private readonly AppRegistry _registry;
        public ProjectRegistry CurrentProjectRegistry { get; private set; }

        public ProjectManager(AppRegistry registry)
        {
            _registry = registry;
        }

        public void Init()
        {
            CreateProjectDefinitionList();
            SetDefaultProject();
        }

        private void SetDefaultProject()
        {
            var projectFromConfig = _projectRegistries
                .FirstOrDefault(x =>
                    x.Name.Equals(_registry.DefaultProject, StringComparison.OrdinalIgnoreCase));
            if (projectFromConfig != null)
            {
                CurrentProjectRegistry = projectFromConfig;
                return;
            }

            var firstProject = _projectRegistries.OrderBy(x => x.Name).FirstOrDefault();

            CurrentProjectRegistry = firstProject ?? throw new KeyNotFoundException(
                                          $"Please create one or more projects in: {_registry.ProjectPath}. Project should contains valid config file: {Consts.PROJECT_CONFIG_FILE}");
        }

        private void CreateProjectDefinitionList()
        {


            var di = new DirectoryInfo(_registry.ProjectPath);
            foreach (var directoryInfo in di.GetDirectories())
            {
                var path = Path.Combine(directoryInfo.FullName, "config", Consts.PROJECT_CONFIG_FILE);
                if (!File.Exists(path)) continue;
                var json = File.ReadAllText(path);
                var config = JsonConvert.DeserializeObject<ProjectConfig>(json);
                if (string.IsNullOrEmpty(config.Name)) continue;
                var projectRegistry = new ProjectRegistry(directoryInfo.FullName, config);
                _projectRegistries.Add(projectRegistry);
            }

        }
    }
}