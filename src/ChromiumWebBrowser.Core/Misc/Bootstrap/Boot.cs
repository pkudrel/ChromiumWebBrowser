using System;
using System.Collections.Generic;
using System.Reflection;
using NLog;

namespace ChromiumWebBrowser.Core.Misc.Bootstrap
{
    public class Boot
    {
        private static readonly List<string> _buffer = new List<string>();
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly Boot _instance = new Boot();
        private static readonly object _obj = new object();
        private AppEnvironment _appEnvironment;

        static Boot()
        {
        }

        private Boot()
        {
        }

        // ReSharper disable once ConvertToAutoProperty
        public static Boot Instance => _instance;




        public AppEnvironment Start(Assembly mainAssembly)
        {
            if (_appEnvironment is null)
                lock (_obj)
                {
                    if (_appEnvironment is null)
                        _appEnvironment = AppEnvironmentBuilder.Instance.GetAppEnvironment(mainAssembly);
                }

            return _appEnvironment;
        }


        public AppEnvironment GetAppEnvironment()
        {

            if (_appEnvironment == null)
                throw new NullReferenceException("Application has not been started correctly. Use Start method");
            return _appEnvironment;
        }

        private static void FlushBuffer(ICollection<string> list)
        {
            foreach (var msg in list) _log.Debug(msg);
            list.Clear();
        }

        public void AddAssembly(Assembly assembly, AssemblyInProject assemblyInProject)
        {
            AssemblyCollector.Instance.AddAssembly(assembly, assemblyInProject);
        }

        public Assembly[] GetAssemblies()
        {
            return AssemblyCollector.Instance.GetAssemblies();
        }
    }
}