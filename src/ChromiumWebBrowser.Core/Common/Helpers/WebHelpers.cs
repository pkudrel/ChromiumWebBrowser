using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using NLog;

namespace ChromiumWebBrowser.Core.Common.Helpers
{
    public static class WebHelpers
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static string Base64Decode(string base64EncodedData, bool silentException = false)
        {
            // The length of a base64 encoded string is always a multiple of 4.
            // https://stackoverflow.com/questions/2925729/invalid-length-for-a-base-64-char-array
            var mod4 = base64EncodedData.Length % 4;
            if (mod4 > 0) base64EncodedData += new string('=', 4 - mod4);
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception)
            {
                if (silentException == false) _log.Warn($"Invalid base64 string: {base64EncodedData}");
            }

            return string.Empty;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static List<string> GetUrls(string message)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(message)) return list;

            var urlRx = new Regex(
                @"(http|ftp|https)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?",
                RegexOptions.IgnoreCase);

            var matches = urlRx.Matches(message);
            foreach (Match match in matches) list.Add(match.Value);
            return list;
        }

        public static NameValueCollection ParseQueryString(string query)
        {
            if (string.IsNullOrEmpty(query)) return new NameValueCollection();
            return HttpUtility.ParseQueryString(query);
        }

        public static string UrlEncode(string text)
        {
            return HttpUtility.UrlEncode(text);
        }

        public static string UrlDecode(string text)
        {
            return HttpUtility.UrlDecode(text);
        }


        public static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                    from value in nvc.GetValues(key)
                    select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}")
                .ToArray();
            return "?" + string.Join("&", array);
        }


        public static List<string> GetUrlsAndAddScheme(List<string> items, string scheme)
        {
            var ret = new List<string>();


            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item)) continue;
                if (item.StartsWith("http"))
                    ret.Add(item);
                else if (item.StartsWith("//")) ret.Add($"{scheme}:{item}");
            }

            return ret;
        }


        public static List<string> GetPossibleUrls(string message)
        {
            (bool, char) IsDelimiterChar(char c1)
            {
                switch (c1)
                {
                    case '"':
                    case '\'':
                        return (true, c1);
                    default:
                        return (false, c1);
                }
            }

            var list = new List<string>();
            var sb = new StringBuilder(message.Length);
            var charsFromStar = 0;
            var isBetween = false;
            var invalidString = false;
            var startChar = (char) 0;
            foreach (var ch in message)
            {
                // if two chars at the beginning are invalid - the whole string is invalid 
                if (charsFromStar == 2)
                {
                    var two = sb.ToString();
                    switch (two)
                    {
                        case "ht":
                        case "//":
                            break;
                        default:
                            invalidString = true;
                            break;
                    }
                }

                // get a delimiter and a char that creates it
                var isDelimiter = IsDelimiterChar(ch);

                if (isDelimiter.Item1)
                {
                    if (isDelimiter.Item2 != startChar && isBetween)
                    {
                        ++charsFromStar;
                        sb.Append(ch);
                        continue;
                    }

                    if (isBetween)
                    {
                        if (invalidString == false && sb.Length > 2)
                        {
                            var text = sb.ToString();
                            if (IsValidUrl(text)) list.Add(sb.ToString());
                        }

                        sb.Clear();
                        isBetween = false;
                        invalidString = false;
                        charsFromStar = 0;
                    }
                    else
                    {
                        isBetween = true;
                        startChar = isDelimiter.Item2;
                    }
                }
                else
                {
                    if (isBetween)
                    {
                        ++charsFromStar;
                        sb.Append(ch);
                    }
                }
            }

            var ret = list.Where(x =>
                x.StartsWith("//", StringComparison.OrdinalIgnoreCase) ||
                x.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ||
                x.StartsWith("https:", StringComparison.OrdinalIgnoreCase)).ToList();
            return ret;
        }

        /// <summary>
        /// Valid url (in our domain) starts with: '//', 'http:', 'https:'
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool IsValidUrl(string text)
        {
            return text.StartsWith("//", StringComparison.OrdinalIgnoreCase) ||
                   text.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ||
                   text.StartsWith("https:", StringComparison.OrdinalIgnoreCase);
        }

        public static List<string> GetStringBetween(string message, char c)
        {
            var list = new List<string>();
            var isBetween = false;
            var sb = new StringBuilder(message.Length);
            foreach (var ch in message)
            {
                if (ch == c)
                {
                    if (isBetween)
                    {
                        list.Add(sb.ToString());
                        sb.Clear();
                        isBetween = false;
                    }
                    else
                    {
                        isBetween = true;
                    }


                    continue;
                }

                if (isBetween) sb.Append(ch);
            }

            return list;
        }

        public static List<string> GetStringBetween(string text, string delimiter)
        {
            var ret = new List<string>();
            if (string.IsNullOrEmpty(text)) return ret;
            var start = 0;

            while (true)
            {
                start = text.IndexOf(delimiter, start, StringComparison.Ordinal);
                if (start == -1) break;
                start += 1;
                var end = text.IndexOf(delimiter, start, StringComparison.Ordinal);
                if (end == -1) break;
                var sub = text.Substring(start, end - start);
                if (string.IsNullOrEmpty(sub)) break;
                ret.Add(sub);
                start = end + 1;
            }

            return ret;
        }

        public static List<string> GetStringBetween(string text, string startText, string endText)
        {
            var ret = new List<string>();
            if (string.IsNullOrEmpty(text)) return ret;
            var start = 0;

            while (true)
            {
                start = text.IndexOf(startText, start, StringComparison.Ordinal);
                if (start == -1) break;
                start += startText.Length;
                var end = text.IndexOf(endText, start, StringComparison.Ordinal);
                if (end == -1) break;
                var sub = text.Substring(start, end - start);
                if (string.IsNullOrEmpty(sub)) break;
                ret.Add(sub);
                start = end + 1;
            }

            return ret;
        }

        public static string GetMd5(byte[] bytes)
        {
            var last = bytes.Last();
            var len = bytes.Length;
            var result = new StringBuilder(bytes.Length * 2);
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                foreach (var t in hash) result.Append(t.ToString("x2"));
            }

            return result.ToString();
        }

        public static string GetSecondLevelHost(string host)
        {
            var arr = host.Split('.');
            if (arr.Length < 2)
                throw new ArgumentOutOfRangeException($"It is not possible generate second level domain from: {host}");
            return $"{arr[arr.Length - 2]}.{arr[arr.Length - 1]}";
        }

        public static string GetThirdLevelHost(string host, bool safe = false)
        {
            var arr = host.Split('.');
            if (arr.Length < 3 && safe == false)
                throw new ArgumentOutOfRangeException($"It is not possible generate second level domain from: {host}");

            if (arr.Length < 3) return host;
            return $"{arr[arr.Length - 3]}.{arr[arr.Length - 2]}.{arr[arr.Length - 1]}";
        }

        public static string GetVariable1(string text, string variableName,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            var pos1 = text.IndexOf(variableName, stringComparison);
            if (pos1 < 0) return string.Empty;
            var pos2 = text.IndexOf("=", pos1, stringComparison);
            if (pos2 < 0) return string.Empty;
            var pos3 = pos2 + 1;
            var pos4 = pos3;
            while (char.IsWhiteSpace(text[pos4])) pos4++;

            var sub = text.Substring(pos4, 20);

            switch (text[pos4])
            {
                case '\'':
                    var pos5 = text.IndexOf('\'', pos4 + 1);
                    var sub1 = text.Substring(pos4 + 1, pos5 - pos4 - 1)?.Trim();
                    return sub1;

                case '"':
                    var pos6 = text.IndexOf('"', pos4 + 1);
                    var sub2 = text.Substring(pos4 + 1, pos6 - pos4 - 1);
                    return sub2;
                default:
                    var pos7 = pos4 + 1;
                    while (char.IsWhiteSpace(text[pos7]) == false || text[pos7] != '\'' || text[pos7] != '\'') pos7++;
                    var sub3 = text.Substring(pos4 + 1, pos7 - pos4 - 1);
                    return sub3;
            }
        }

        public static string GetVariable2(string text, string variableName,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            var pos1 = text.IndexOf(variableName, stringComparison);
            if (pos1 < 0) return string.Empty;
            var pos2 = text.IndexOf("=", pos1, stringComparison);
            if (pos2 < 0) return string.Empty;
            var div = pos2 - pos1 - variableName.Length;
            if (div > 10) return string.Empty;
            var pos3 = pos2 + 1;
            var pos4 = pos3;
            while (char.IsWhiteSpace(text[pos4])) pos4++;

            var sub = text.Substring(pos4, 20);

            switch (text[pos4])
            {
                case '\'':
                    var pos5 = text.IndexOf('\'', pos4 + 1);
                    var sub1 = text.Substring(pos4 + 1, pos5 - pos4 - 1)?.Trim();
                    return sub1;

                case '"':
                    var pos6 = text.IndexOf('"', pos4 + 1);
                    var sub2 = text.Substring(pos4 + 1, pos6 - pos4 - 1);
                    return sub2;
                default:
                    var pos7 = pos4 + 1;
                    while (char.IsWhiteSpace(text[pos7]) == false || text[pos7] != '\'' || text[pos7] != '\'') pos7++;
                    var sub3 = text.Substring(pos4 + 1, pos7 - pos4 - 1);
                    return sub3;
            }
        }

        public static string GetStringFromHextJsLiteral(string text)
        {
            try
            {
                var sb = new StringBuilder();

                var arr2 = text.Split(new[] {"\\x"}, StringSplitOptions.RemoveEmptyEntries);

                foreach (var code in arr2)
                {
                    var c = Convert.ToChar(Convert.ToUInt32(code, 16));
                    sb.Append(c);
                }


                return sb.ToString();
            }
            catch (Exception)
            {
                //
            }

            return string.Empty;
        }


        public static string GetJsonKeyFirstValue(string sourceText, string key, bool strict = true)

        {
            var r = GetJsonKeyValues(sourceText, key, strict);
            return r.Count == 0 ? string.Empty : r[0];
        }

        public static List<string> GetJsonKeyValues(string sourceText, string key, bool strict = true)
        {
            var ret = new List<string>();
            if (string.IsNullOrEmpty(sourceText)) return ret;

            var qKey = strict ? $"\"{key}\"" : key;
            var z = 0;

            while (true)
            {
                var sb = new StringBuilder();
                var pos1 = sourceText.IndexOf(qKey, z, StringComparison.OrdinalIgnoreCase);
                if (pos1 < 0) return ret;
                var pos2 = sourceText.IndexOf(':', pos1);
                var pos3 = sourceText.IndexOf('"', pos2);
                z = pos3 + 1;
                var c1 = (char) 0;

                while (z <= sourceText.Length)
                {
                    var c = sourceText[z];
                    if (c != '"')
                    {
                        if (c != '\\') sb.Append(c);


                        c1 = c;
                    }
                    else
                    {
                        if (c1 == '\\')
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            ret.Add(sb.ToString());
                            sb.Clear();
                            break;
                        }
                    }

                    z++;
                }
            }
        }
    }
}