using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ChromiumWebBrowser.Core.Features.ResourceRequest.Models
{
    public class Processor : IProcessorDefinition
    {
        public Processor()
        {
        }

        private Processor(int val, string name)
        {
            Id = val;
            Name = name;
        }


        public static Processor Default { get; } = new Processor(1, "Default");


        public string Name { get; set; }
        public int Id { get; set; }

        private static List<Processor> GetUnavailable()
        {
            return new List<Processor>();
        }


        public static List<Processor> List()
        {
            var unavailable = GetUnavailable();

            return typeof(Processor).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Processor))
                .Select(pi => (Processor) pi.GetValue(null, null))
                .Where(x => unavailable.Contains(x) == false)
                .OrderBy(p => p.Name)
                .ToList();
        }

        public override string ToString()
        {
            return Name;
        }

        public static Processor FromString(string roleString)
        {
            return List().Single(r => string.Equals(r.Name, roleString, StringComparison.OrdinalIgnoreCase));
        }

        public static Processor FromValue(int value)
        {
            return List().Single(r => r.Id == value);
        }
    }
}