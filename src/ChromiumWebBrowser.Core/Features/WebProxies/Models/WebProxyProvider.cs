using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ChromiumWebBrowser.Core.Features.WebProxies.Models
{
    public class WebProxyProvider
    {
        private WebProxyProvider(int index, string name, string commandLineSwitch, bool enable)
        {
            Index = index;
            Name = name;
            CommandLineSwitch = commandLineSwitch;
            Enable = enable;
        }

        public int Index { get; }
        public string Name { get; }
        public string CommandLineSwitch { get; }
        public bool Enable { get; }
        public static WebProxyProvider None { get; } = new WebProxyProvider(1, "None", "None", false);
        public static WebProxyProvider Luminati { get; } = new WebProxyProvider(2, "Luminati", "luminati", true);
        private static List<WebProxyProvider> GetUnavailable()
        {
            return new List<WebProxyProvider>();
        }

        public override string ToString()
        {
            return Name;
        }

        public static WebProxyProvider FromString(string switchValue)
        {
            if (string.IsNullOrEmpty(switchValue)) return None;
            var item = List().FirstOrDefault(x =>
                string.Equals(x.CommandLineSwitch, switchValue, StringComparison.OrdinalIgnoreCase));
            if (item == null)
                throw new ArgumentOutOfRangeException("proxy", $"Unknown proxy name: '{switchValue}'");
            return item;
        }

        public static List<WebProxyProvider> List()
        {
            var unavailable = GetUnavailable();

            return typeof(WebProxyProvider).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(WebProxyProvider))
                .Select(pi => (WebProxyProvider) pi.GetValue(null, null))
                .Where(x => unavailable.Contains(x) == false)
                .OrderBy(p => p.Name)
                .ToList();
        }
    }
}