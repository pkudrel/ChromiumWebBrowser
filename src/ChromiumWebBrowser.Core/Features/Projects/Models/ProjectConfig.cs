using System.Collections.Generic;
using CefSharp;
using ChromiumWebBrowser.Core.Features.ResourceRequest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChromiumWebBrowser.Core.Features.Projects.Models
{
    public class ProjectConfig
    {
        public string Name { get; set; }
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<ResourceType> ResourceRequestTypesAllowed { get; set; } = new List<ResourceType>();
        public List<string> ResourceRequestProtocolsAllowed { get; set; } = new List<string>();
        public List<UrlRule> ResourceRequestUrlsActionsAllowed { get; set; } = new List<UrlRule>();
    }
}