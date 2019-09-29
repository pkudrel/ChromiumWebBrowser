using Nuke.Common.ProjectModel;

namespace Models
{
    public class ProjectDefinition
    {
        public string Name { get; set; }
        public Project Project { get; set; }
        public string Exe { get; set; }
        public string Dir { get; set; }
        public string DstExe { get; set; }
        public string AzureContainerName { get; set; }
    }
}