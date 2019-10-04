using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChromiumWebBrowser.Core.Features.ResourceRequest.Models
{
    public class UrlRule
    {
        public string Value { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MatchType MatchType { get; set; }

        public List<string> Actions { get; set; } = new List<string>();
    }
}