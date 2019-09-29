using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class AppDomainExtensions
    {
        private const string AssemblieName = "DenebLab.UPlanet.";

        public static Assembly[] GetAppAssemblies(this AppDomain @this)
        {
            return @this.GetAssemblies()
                .Where(x => x.FullName.StartsWith(AssemblieName))
                .ToArray();
        }

        public static IEnumerable<Type> GetAppTypes(this AppDomain @this)
        {
            return @this.GetAssemblies()
                .Where(x => x.FullName.StartsWith(AssemblieName))
                .SelectMany(x => x.GetTypes());
        }

        public static Assembly[] GetAppAssemblies(this AppDomain @this, string[] startsWith)
        {
            var list = new List<Assembly>();
            foreach (string s in startsWith)
            {
                list.AddRange(@this.GetAssemblies()
                    .Where(x => x.FullName.StartsWith(s)));
            }

            return list.ToArray();
        }

        public static IEnumerable<Type> GetAppTypes(this AppDomain @this, string[] startsWith)
        {

            var list = new List<Type>();
            foreach (string s in startsWith)
            {
                list.AddRange(@this.GetAssemblies()
                .Where(x => x.FullName.StartsWith(s))
                .SelectMany(x => x.GetTypes()));
            }

            return list;


        }

    }
}