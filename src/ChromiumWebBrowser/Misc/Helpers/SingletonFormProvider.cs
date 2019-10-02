using System;
using System.Windows.Forms;

namespace ChromiumWebBrowser.Misc.Helpers
{
    public class SingletonFormProvider<T> : IDisposable
        where T : Form

    {
        private readonly Func<T> _createForm;
        private T _currentInstance;

        public SingletonFormProvider(Func<T> createForm)
        {
            _createForm = createForm;
        }
        public T CurrentInstance
        {
            get
            {
                if (_currentInstance == null)
                    _currentInstance = _createForm();

                // TODO here: wire into _currentInstance close event
                // to null _currentInstance field

                return _currentInstance;
            }
        }



        public void Dispose()
        {
            Close();
        }

      
        public void Close()
        {
            _currentInstance?.Dispose();
        }
    }
}