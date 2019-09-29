using System.Collections.Generic;
using System.Linq;

namespace ChromiumWebBrowser.Core.Common.Io
{
    public class QueryString
    {
        private List<KeyValuePair<string, string>> _list;

        public QueryString(string qs)
        {
            var r = qs.Split('&');
            _list = new List<KeyValuePair<string, string>>();
            foreach (var s in r)
            {
                var s2 = s.Split('=');
                var key = s2[0];
                var value = s2[1];

                if (key.StartsWith("?"))
                {
                    key = key.Substring(1);
                }
                _list.Add(new KeyValuePair<string, string>(key, value));
            }
            
        }

        public string Get(string key)
        {
            return _list.FirstOrDefault(x => x.Key == key).Value;
        }
    }
}