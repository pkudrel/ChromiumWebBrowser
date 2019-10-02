using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ChromiumWebBrowser.Core.Common.Helpers;
using ChromiumWebBrowser.Core.Features.HttpClients.Models;
using NLog;
using Cookie = System.Net.Cookie;

namespace ChromiumWebBrowser.Core.Features.HttpClients
{
    public class DenebLabHttpClient : HttpClient
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ClientOptionsBase _clientOptions;
        private readonly CookieContainer _cookieContainer;
        private readonly HttpClientHandler _httpClientHandler;

        public DenebLabHttpClient(
            HttpClientHandler messageHandler,
            ClientOptionsBase clientOptions) : base(messageHandler)
        {
            _httpClientHandler = messageHandler;
            _cookieContainer = messageHandler.CookieContainer;
            _clientOptions = clientOptions;
        }


        public DenebLabHttpClient(HttpClientHandler messageHandler, CookieContainer cookieContainer,
            ClientOptionsBase clientOptions) : base(messageHandler)
        {
            _httpClientHandler = messageHandler;
            _cookieContainer = cookieContainer;
            _clientOptions = clientOptions;
        }


        public Task<HttpResponseMessage> SendWithOptionsAsync(HttpRequestMessage request,
            CancellationToken cancellationToken = default)
        {
            AddOptions(request);
            return SendAsync(request, cancellationToken);
        }

        private void AddOptions(HttpRequestMessage request)
        {
            if (_clientOptions.Headers != null)
                foreach (var kv in _clientOptions.Headers)
                    request.Headers.Add(kv.Key, kv.Value);
        }

        public CookieCollection GetCookie(Uri uri)
        {
            return _cookieContainer.GetCookies(uri);
        }

        public void SetCookie(string key, string value)
        {
            var c = new Cookie(key, value);
            _cookieContainer.Add(c);
        }


        public void SetWebProxy(WebProxy webProxy)
        {
            _httpClientHandler.Proxy = webProxy;
        }


        public bool IsWebProxySet()
        {
            var item = _httpClientHandler.Proxy;
            return item != null;
        }

        public void RemoveAllCookies()
        {
            foreach (var cookie in GetAllCookies())
            {
                cookie.Expires = DateTime.Now.AddDays(-10);
                cookie.Expired = true;
            }
        }

        public void SetCookie(string key, string value, string path)
        {
            var c = new Cookie(key, value, path);
            _cookieContainer.Add(c);
        }

        public void SetCookie(string key, string value, string path, string domain)
        {
            try
            {
                var c = new Cookie(key, value, path, domain);
                _cookieContainer.Add(c);
                return;
            }
            catch (CookieException e)
            {
                _log.Warn(e, "Problem with SetCookie. You should propably use 'SetCookieSafe' method.");
            }

            var valueEncode = WebHelpers.UrlEncode(value);
            var c1 = new Cookie(key, valueEncode, path, domain);
            _cookieContainer.Add(c1);
        }

        public void SetCookie(string key, string value, string path, string domain, int validHours)
        {
            var c = new Cookie(key, value, path, domain) {Expires = DateTime.Now.AddHours(validHours)};
            _cookieContainer.Add(c);
        }

        public IEnumerable<Cookie> GetAllCookies()
        {
            var k =
                (Hashtable)
                _cookieContainer.GetType()
                    .GetField("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.GetValue(_cookieContainer);
            foreach (DictionaryEntry element in k)
            {
                var l =
                    (SortedList)
                    element.Value.GetType()
                        .GetField("m_list", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?.GetValue(element.Value);
                if (l != null)
                    foreach (var e in l)
                    {
                        var cl = (CookieCollection) ((DictionaryEntry) e).Value;
                        foreach (Cookie fc in cl)
                            yield return fc;
                    }
            }
        }


        public void SetCookieSafe(string key, string value, string path, string domain)
        {
            try
            {
                var c = new Cookie(key, value, path, domain);
                _cookieContainer.Add(c);
                return;
            }
            catch (CookieException e)
            {
                _log.Warn(e, "Problem with SetCookie. Next try with 'UrlEncode' cookie value");
            }

            var valueEncode = WebHelpers.UrlEncode(value);
            var c1 = new Cookie(key, valueEncode, path, domain);
            _cookieContainer.Add(c1);
        }
    }
}