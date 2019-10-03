using System.Collections.Generic;
using CefSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChromiumWebBrowser.Features.Projects.Models
{
    public class ProjectConfig
    {
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public List<ResourceType> ResourceRequestTypesAllowed { get; set; } = new List<ResourceType>();
    }
}