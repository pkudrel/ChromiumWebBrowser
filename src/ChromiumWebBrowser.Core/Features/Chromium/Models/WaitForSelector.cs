using System;

namespace ChromiumWebBrowser.Core.Features.Chromium.Models
{
    public class WaitForSelector
    {
        public WaitForSelector()
        {
        }

        public WaitForSelector(string selector, SelectorState selectorState)
        {
            Selector = selector;
            SelectorState = selectorState;
            Hidden = selectorState == SelectorState.Hidden;
            Visible = selectorState == SelectorState.Visible;
        }

        public WaitForSelector(string selector, SelectorState selectorState, int timeoutInMs)
        {
            Selector = selector;
            SelectorState = selectorState;
            Hidden = selectorState == SelectorState.Hidden;
            Visible = selectorState == SelectorState.Visible;
            Timeout = timeoutInMs;
        }

        public WaitForSelector(string selector, SelectorState selectorState, TimeSpan timeoutTimeSpan)
        {
            Selector = selector;
            SelectorState = selectorState;
            Hidden = selectorState == SelectorState.Hidden;
            Visible = selectorState == SelectorState.Visible;
            Timeout = Convert.ToInt32(timeoutTimeSpan.TotalMilliseconds);
        }

        public int Timeout { get; set; } = 30000;
        public bool Visible { get; set; }
        public bool Hidden { get; set; }
        public string Selector { get; set; }
        public SelectorState SelectorState { get; set; }
    }
}