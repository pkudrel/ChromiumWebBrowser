using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using CefSharp;
using CefSharp.Internals;
using ChromiumWebBrowser.Core.Features.Chromium.Models;
using NLog;
using Cookie = ChromiumWebBrowser.Core.Features.Chromium.Models.Cookie;


namespace ChromiumWebBrowser.Features.Chromium.Controls
{
    public class AdvanceChromiumWebBrowser : CefSharp.WinForms.ChromiumWebBrowser
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly IConfiguration _config = Configuration.Default;
        private readonly IBrowsingContext _context = BrowsingContext.New(_config);
        private readonly List<string> _visitedUrls = new List<string>();

        public AdvanceChromiumWebBrowser(string address, IRequestContext requestContext = null) :
            base(address, requestContext)
        {
        }



        public void StartMonitoringVisitedUrls()
        {
            AddressChanged += OnAddressChanged;
        }

        public void StopMonitoringVisitedUrls()
        {
            AddressChanged -= OnAddressChanged;
        }

        public List<string> GetVisitedUrls()
        {
            return _visitedUrls;
        }


        private void OnAddressChanged(object sender, AddressChangedEventArgs e)
        {
            _visitedUrls.Add(e.Address);
        }

        /// <summary>
        ///     Active wait for selector. Sorry.
        /// </summary>
        /// <param name="waitForSelector"></param>
        /// <param name="activeWaitTimeMs"></param>
        /// <returns></returns>
        public async Task WaitForSelectorAsync(
            WaitForSelector waitForSelector,
            int activeWaitTimeMs = 100)
        {
            var sw = new Stopwatch();
            var tryNumber = 0;
            var endTime = DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(waitForSelector.Timeout));
            var waitTime = TimeSpan.FromMilliseconds(activeWaitTimeMs);
            sw.Start();
            while (true)
            {
                tryNumber++;
                //_log.Debug($"WaitForSelectorAsync: '{waitForSelector.Selector}'; Try number: {tryNumber}");
                var document = await GetDocument();
                var node = document.QuerySelector(waitForSelector.Selector);
                if (waitForSelector.Hidden && node == null)
                {
                    sw.Stop();
                    _log.Debug(
                        $"Node with selector is hidden; Wait time: {sw.ElapsedMilliseconds}ms; Tries: {tryNumber}");
                    return;
                }

                if (waitForSelector.Visible && node != null)
                {
                    sw.Stop();
                    _log.Debug(
                        $"Node with selector is visible; Wait time: {sw.ElapsedMilliseconds}ms; Tries: {tryNumber}");
                    return;
                }

                if (DateTime.UtcNow >= endTime)
                {
                    sw.Stop();
                    _log.Debug($"Timeout; Wait time: {sw.ElapsedMilliseconds}ms; Tries: {tryNumber}");
                    return;
                }

                await Task.Delay(waitTime).ConfigureAwait(false);
            }
        }

        public async Task WaitForSelectorAsync(
            IEnumerable<WaitForSelector> waitForSelectors,
            int activeWaitTimeMs = 100)
        {
            foreach (var selector in waitForSelectors) await WaitForSelectorAsync(selector, activeWaitTimeMs);
        }

        public async Task<IDocument> GetDocument(string address, WaitForSelector selector, CancellationToken token)
        {
            await Navigate(address, token);
            await WaitForSelectorAsync(selector);
            var document = await GetDocument();
            return document;
        }


        public async Task<List<Cookie>> GetCookies()
        {
            var manager = this.GetCookieManager();
            var cefCookies = await manager.VisitAllCookiesAsync();
            var cookies = CookieConverter.ConvertList(cefCookies);
            return cookies;
        }

        public async Task<List<Cookie>> GetCookiesForVisitedUrls(List<string> list, bool includeHttpOnly = false)
        {
            var res = new List<Cookie>();

            var manager = this.GetCookieManager();
            var urls = list.Select(Url.Create).DistinctBy(x => x.Host).Select(x => x.Href);
            foreach (var u in urls)
            {
                var cefCookies = await manager.VisitUrlCookiesAsync(u, includeHttpOnly);
                var cookies = CookieConverter.ConvertList(cefCookies);
                res.AddRange(cookies);
            }

            return res;
        }

        public async Task<List<Cookie>> GetCookiesForVisitedUrlsAll(List<string> list, bool includeHttpOnly = false)
        {
            string GetSecondLevelHost(string host)
            {
                var arr = host.Split('.');
                if (arr.Length < 2) return string.Empty;
                return $"{arr[arr.Length - 2]}.{arr[arr.Length - 1]}";
            }


            list.Add("https://www.google.com/recaptcha/api2");

            var res = new List<Cookie>();
            var manager = this.GetCookieManager();
            var hosts = list.Select(Url.Create)
                .DistinctBy(x => x.Host)
                .Select(x => GetSecondLevelHost(x.Host))
                .Where(x => string.IsNullOrEmpty(x) == false)
                .ToArray();

            var cefCookies = await manager.VisitAllCookiesAsync();
            foreach (var u in cefCookies)
            {
                if (string.IsNullOrEmpty(u.Domain)) continue;
                var normalized = u.Domain.StartsWith(".") ? u.Domain.Substring(1) : u.Domain;
                if (hosts.Any(x => normalized.EndsWith(x)) == false) continue;
                var c = CookieConverter.Convert(u);
                res.Add(c);
            }

            return res;
        }


        public async Task<IDocument> GetDocument()
        {
            var source = await this.GetMainFrame().GetSourceAsync();
            var document = await _context.OpenAsync(m => m.Content(source).Address(this.GetMainFrame().Url));
            return document;
        }

        public async Task<IDocument> GetDocument(string address, CancellationToken token)
        {
            await Navigate(address, token);
            var document = await GetDocument();
            return document;
        }

        public async Task<string> GetSource()
        {
            var source = await this.GetMainFrame().GetSourceAsync();
            return source;
        }

        public async Task<FrameSimple> GetMainFrameSimple()
        {
            var source = await this.GetMainFrame().GetSourceAsync();
            var url = Address;

            return new FrameSimple(url, source);
        }

        public async Task SetProxy(string address)
        {
            await Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var rc = GetBrowser().GetHost().RequestContext;
                var v = new Dictionary<string, object>();
                v["mode"] = "fixed_servers";
                v["server"] = address;
                var success = rc.SetPreference("proxy", v, out _);
            });
        }


        public Task WaitForLoadEnd()
        {
            var tcs = new TaskCompletionSource<bool>();

            void Handler(object sender, LoadingStateChangedEventArgs args)
            {
                if (args.IsLoading) return;
                LoadingStateChanged -= Handler;
                tcs.TrySetResultAsync(true);
            }

            LoadingStateChanged += Handler;
            return tcs.Task;
        }
        public async Task Navigate(string address, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();

            void Handler(object sender, LoadingStateChangedEventArgs arguments)
            {
                _log.Debug($"LoadingStateChangedEventArgs: {arguments.Browser.MainFrame.Url}");
                if (arguments.IsLoading) return;
                LoadingStateChanged -= Handler;
                tcs.TrySetResult(true);
            }

            using (token.Register(() =>
            {
                this.Stop();
                tcs.TrySetCanceled();
            },
                true))
            {
                LoadingStateChanged += Handler;
                try
                {
                    Load(address);
                    await tcs.Task;
                }
                finally
                {
                    LoadingStateChanged -= Handler;
                }
            }
        }

    }
}