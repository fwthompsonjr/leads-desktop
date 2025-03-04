using HtmlAgilityPack;
using LegalLead.PublicData.Search.Extensions;
using Newtonsoft.Json;
using Polly;
using Polly.Timeout;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Util
{
    using DST = System.Net.Cookie;
    using Rx = Properties.Resources;
    using SRC = OpenQA.Selenium.Cookie;
    public class DalllasBulkCaseReader : BaseDallasSearchAction
    {
        public override int OrderId => 80;
        public List<CaseItemDto> Workload { get; } = [];

        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (Workload.Count == 0)
                throw new NullReferenceException(Rx.ERR_URI_MISSING);

            var cookies = Driver.Manage().Cookies.AllCookies;
            if (cookies.Count > 0)
            {
                Debug.WriteLine("Found {0:d} cookies", cookies.Count);
            }
            var count = Workload.Count;
            var items = new ConcurrentDictionary<int, CaseItemDtoMapper>();
            var consecutiveFailureCount = 0;
            Workload.ForEach(x =>
            {
                items[Workload.IndexOf(x)] = new() { Dto = x };
            });
            var retries = 0;
            var mxretries = 6;
            const int relaxInternal = 25;
            const int relaxThreshold = 4;
            while (retries < mxretries)
            {
                Workload.ForEach(c =>
                {
                    var id = Workload.IndexOf(c);
                    var w = IterateWorkLoad(id, c, cookies, count, items);
                    if (w == 0)
                    {
                        consecutiveFailureCount = 0;
}
                    else
                    {
                        consecutiveFailureCount++;
                        if (consecutiveFailureCount > relaxThreshold)
                        {
                            Console.WriteLine($"Detected website response failure.{Environment.NewLine}Waiting {relaxInternal:F2} seconds.");
                            Thread.Sleep(TimeSpan.FromSeconds(relaxInternal));
                            consecutiveFailureCount = 0;
                        }
                    }
                });
                var hasFailures = items.Any(x => !x.Value.IsMapped());
                if (!hasFailures) break;
                var unresloved = items.Count(x => !x.Value.IsMapped());
                Console.WriteLine($"Found {unresloved} items needing review.");
                retries++;
                var seconds = Math.Max(30, Math.Min(Math.Pow(2, retries) * 4, 75));
                Console.WriteLine($"Waiting {seconds:F2} seconds before retry.");
                var wait = TimeSpan.FromSeconds(seconds);
                Thread.Sleep(wait);
            }
            Workload.Clear();
            Workload.AddRange(items.Select(x => x.Value.Dto));
            // Cast ConcurrentDictionary to Dictionary before returning
            return Workload.ToJsonString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "<Pending>")]
        private int IterateWorkLoad(
            int idx,
            CaseItemDto c,
            ReadOnlyCollection<SRC> cookies,
            int count,
            ConcurrentDictionary<int, CaseItemDtoMapper> cases)
        {
            if (idx % 5 == 0)
            {
                ResetPageSession();
            }
            var instance = cases[idx];
            if (instance.IsMapped()) return 0;
            var ii = Math.Min(idx + 1, count);
            EchoIteration(c, ii, count);

            var content = GetContentWithPollyAsync(c.Href, cookies).GetAwaiter().GetResult();
            if (string.IsNullOrEmpty(content) || content.Equals("error")) return 1;
            var data = GetPageContent(content);
            instance.MappedContent = data;
            instance.Map();
            return 0;
        }
        private static async Task<string> GetContentWithPollyAsync(string href, ReadOnlyCollection<SRC> cookies)
        {
            var timeoutPolicy = Policy.TimeoutAsync(6, TimeoutStrategy.Pessimistic);
            var fallbackPolicy = Policy<string>
                .Handle<Exception>()
                .Or<TimeoutRejectedException>() // Handle timeout exceptions
                .FallbackAsync(async (cancellationToken) =>
                {
                    await Task.Run(() => { Thread.Sleep(TimeSpan.FromMinutes(1)); }, cancellationToken);
                    return string.Empty;
                });

            var policyWrap = timeoutPolicy.WrapAsync(fallbackPolicy);

            try
            {
                return await policyWrap.ExecuteAsync(async () =>
                {
                    return await GetPageAsync(href, cookies);
                });
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        private void ResetPageSession()
        {
            try
            {
                var uri = new Uri(Driver.Url);
                _ = Task.Run(() => Driver.NavigateWithRetry(TimeSpan.FromSeconds(5), uri));
                MaskUserName();
            }
            catch (Exception)
            {
                // no action on failure
            }
        }

        private void EchoIteration(CaseItemDto dto, int current, int max)
        {
            if (Interactive == null) return;
            var d1 = dto.FileDate;
            var dateformat = CultureInfo.CurrentCulture.DateTimeFormat;
            if (DateTime.TryParse(d1, dateformat, DateTimeStyles.AssumeLocal, out var dte1)) d1 = dte1.ToString("M/d", dateformat);
            Interactive.EchoProgess(0, max, current, $"{d1} - Reading item {current} of {max}.", true, dateNotification: d1);
        }
        private static async Task<string> GetPageAsync(string uri, IEnumerable<SRC> cookies)
        {
            var baseAddress = new Uri(uri);
            var cookieContainer = new CookieContainer();
            var cookieJar = cookies.ToList();
            var cookieCollection = new CookieCollection();
            foreach (var cookie in cookieJar)
            {
                if (string.IsNullOrEmpty(cookie.Value.Trim())) continue;
                cookieCollection.Add(new DST(cookie.Name, cookie.Value));
            }
            cookieContainer.Add(baseAddress, cookieCollection);
            var result = await GetRemotePageAsync(baseAddress, cookieContainer);
            return result;
        }

        private static async Task<string> GetRemotePageAsync(Uri baseAddress, CookieContainer cookieContainer)
        {
            try
            {
                using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromMilliseconds(3500) };
                var result = await client.GetAsync(baseAddress);
                if (result.IsSuccessStatusCode)
                {
                    var contents = await result.Content.ReadAsStringAsync();
                    return contents;
                }
                return "";
            }
            catch (Exception)
            {
                return "error";
            }
        }

        private static string GetPageContent(string html)
        {
            const string pipe = "|";
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var obj = new TheAddress();
            var node = doc.DocumentNode;
            var caseStyle = string.Empty;
            var paragraphs = node.SelectNodes(HtmlSelectors.ParagraghTextPrimary);
            if (paragraphs == null) return obj.ToJsonString();
            if (paragraphs.Count > 0)
            {
                var target = paragraphs.FirstOrDefault(s => s.InnerText.Contains(pipe));
                if (target != null)
                {
                    var a = target.InnerText.IndexOf(pipe);
                    caseStyle = target.InnerText[(a + 1)..].Trim();
                }
            }
            obj.CaseHeader = caseStyle;
            var dv = node.SelectSingleNode(HtmlSelectors.DivCaseInformationBody);
            if (dv == null) { return obj.ToJsonString(); }
            var dvs = dv.SelectNodes(HtmlSelectors.Div);
            if (dvs == null || dvs.Count < 2) { return obj.ToJsonString(); }
            dv = dvs[2];
            if (string.IsNullOrEmpty(dv.InnerText)) return string.Empty;
            var arr = dv.InnerText.Trim().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 2) return string.Empty;
            var cs = arr[1].Trim(); // this is actually the case number
            obj.CaseNumber = cs;
            /* get plaintiff */
            var list = node.SelectNodes(HtmlSelectors.Span).ToList();
            var find = list.FindAll(a =>
            {
                var attr = a.GetAttributeValue("class", "null");
                if (attr == "null") { return false; }
                return attr == "text-primary";
            }).FindAll(b => b.InnerText.Trim() == "PLAINTIFF");
            if (find == null || find.Count == 0) { return obj.ToJsonString(); }
            var pln = find[0];
            var paragraph = Closest(pln, "p");
            if (paragraph == null) { return obj.ToJsonString(); }
            Console.WriteLine($"     - Reading case: {cs}");
            obj.Plaintiff = paragraph.InnerText.Replace("PLAINTIFF", "").Trim();
            /* get defendant address */
            var dvparty = node.SelectSingleNode(HtmlSelectors.DivPartyInformationBody);
            var partytypes = new List<string> { "RESPONDENT", "DEFENDANT" };
            if (dvparty == null) return obj.ToJsonString();
            var children = dvparty.SelectNodes(HtmlSelectors.Paragragh)?.ToList();
            if (children == null || children.Count == 0) { return obj.ToJsonString(); }
            var parties = children.FindAll(f =>
            {
                var found = false;
                var dvp = Closest(f, "div");
                if (dvp == null) { return false; }
                var txt = dvp?.InnerText?.Trim() ?? string.Empty;
                partytypes.ForEach(p => { if (txt.Contains(p)) { found = true; } });
                if (!found) { return false; }
                return f.InnerHtml.IndexOf("<span") < 0;
            }).ToList();
            if (parties.Count == 0) return obj.ToJsonString();
            var addr = parties[0].InnerText.Trim();
            addr = addr.Replace(Environment.NewLine, "|").Trim();
            while (addr.Contains("||")) { addr = addr.Replace("||", "|").Trim(); }
            obj.Address = addr;
            return obj.ToJsonString();
        }
        private static class HtmlSelectors
        {
            public const string Div = "div";
            public const string DivCaseInformationBody = "//*[@id='divCaseInformation_body']";
            public const string DivPartyInformationBody = "//*[@id='divPartyInformation_body']";
            public const string Paragragh = "//p";
            public const string ParagraghTextPrimary = "//p[@class='text-primary']";
            public const string Span = "//span";
        }
        private sealed class TheAddress
        {
            [JsonProperty("addr")] public string Address { get; set; }
            [JsonProperty("plaintiff")] public string Plaintiff { get; set; }
            [JsonProperty("casenbr")] public string CaseNumber { get; set; }
            [JsonProperty("caseheader")] public string CaseHeader { get; set; }
            public bool IsValid
            {
                get
                {
                    if (string.IsNullOrEmpty(CaseNumber)) return false;
                    if (string.IsNullOrEmpty(CaseHeader)) return false;
                    return true;
                }
            }
        }
        private sealed class CaseItemDtoMapper
        {
            public string MappedContent { get; set; }
            public CaseItemDto Dto { get; set; }
            public void Map()
            {
                if (!IsMapped()) return;
                var src = MappedContent.ToInstance<TheAddress>();
                if (src == null || !src.IsValid) return;
                Dto.CaseNumber = src.CaseNumber;
                Dto.CaseStyle = src.CaseHeader;
                Dto.Plaintiff = src.Plaintiff;
                if (!string.IsNullOrWhiteSpace(src.Address)) Dto.Address = src.Address;
            }
            public bool IsMapped()
            {
                if (string.IsNullOrWhiteSpace(MappedContent)) return false;
                var src = MappedContent.ToInstance<TheAddress>();
                if (src == null || !src.IsValid) return false;
                return true;
            }
        }
        private static HtmlNode Closest(HtmlNode node, string elementName)
        {
            if (node == null) { return null; }
            var parent = node.ParentNode;
            while (parent != null && !parent.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase))
            {
                parent = parent.ParentNode;
            }
            return parent;
        }
    }
}