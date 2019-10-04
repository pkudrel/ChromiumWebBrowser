using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AngleSharp;
using NLog;

namespace ChromiumWebBrowser.Core.Features.ResourceRequest.Models
{
    public interface IUrlRuleService
    {
        UrlRuleMatchResult Match(List<UrlRule> allowedRules, Url url);
    }


    public class UrlRuleMatchResult
    {
        public UrlRuleMatchResult(bool match)
        {
            Match = match;
        }

        public UrlRuleMatchResult(bool match, List<string> actions)
        {
            Match = match;
            Actions.AddRange(actions);
        }

        public bool Match { get; }
        public List<string> Actions { get; } = new List<string>();
    }

    public class UrlRuleService : IUrlRuleService
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public UrlRuleMatchResult Match(List<UrlRule> allowedRules, Url url)
        {
            foreach (var allowedRule in allowedRules)
                switch (allowedRule.MatchType)
                {
                    case MatchType.Exact:
                        if (url.Href == allowedRule.Value)
                        {
                            _log.Debug($"Rule match type: Exact; Value: {allowedRule.Value}; UrlHref: {url.Href}");
                            return new UrlRuleMatchResult(true, allowedRule.Actions);
                        }

                        break;
                    case MatchType.StartsWith:
                        if (url.Href.StartsWith(allowedRule.Value))
                        {
                            _log.Debug($"Rule match type: StartsWith; Value: {allowedRule.Value}; UrlHref: {url.Href}");
                            return new UrlRuleMatchResult(true, allowedRule.Actions);
                        }

                        break;
                    case MatchType.Regexp:
                        var reg = new Regex(allowedRule.Value);
                        if (reg.IsMatch(url.Href))
                        {
                            _log.Debug($"Rule match type: Regexp; Value: {allowedRule.Value}; UrlHref: {url.Href}");
                            return new UrlRuleMatchResult(true, allowedRule.Actions);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


            return new UrlRuleMatchResult(false);
        }
    }
}