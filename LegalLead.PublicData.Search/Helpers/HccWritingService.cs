using LegalLead.PublicData.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class HccWritingService : IHccWritingService
    {
        public HccWritingService(IHttpService http)
        {
            httpService = http;
        }
        private readonly IHttpService httpService;
        public void Write(string csvdata)
        {
            var revised = GetRevisedList(csvdata);
            if (string.IsNullOrEmpty(revised)) return;
            var dat = Encoding.UTF8.GetBytes(revised);
            var conversion = Convert.ToBase64String(dat);
            var payload = new { Content = conversion };
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(90) };
            _ = httpService.PostAsJson<object, object>(client, RemoteAddress, payload);
        }

        private static string GetRevisedList(string csvdata)
        {
            var tab = '\t';
            string newline = Environment.NewLine;
            var lines = csvdata.Split(newline).ToList();
            if (lines.Count < 2) { return string.Empty; }
            var body = lines.GetRange(1, lines.Count - 1);
            var original = new
            {
                header = lines[0].Split(tab).ToList(),
                content = body.Select(s => s.Split(tab).ToList()).ToList(),
            };
            var isvalid = original.header.Exists(x => FieldList.Contains(x, StringComparer.OrdinalIgnoreCase));
            if (!isvalid) { return string.Empty; }
            var final = new
            {
                header = FieldList,
                content = new List<List<string>>()
            };
            original.content.ForEach(x =>
            {
                var line = new List<string>();
                for (int i = 0; i < final.header.Count; i++)
                {
                    var field = final.header[i];
                    var sourceId = original.header.FindIndex(x => x.Equals(field, StringComparison.OrdinalIgnoreCase));
                    line.Add(TryGetValue(x, sourceId));
                }
                final.content.Add(line);
            });
            final.content.RemoveAll(CheckDateRange);
            var dataset = new StringBuilder();
            dataset.AppendLine(string.Join(tab, final.header));
            final.content.ForEach(x => dataset.AppendLine(string.Join(tab, x)));
            return dataset.ToString();
        }

        private static bool CheckDateRange(List<string> obj)
        {
            var id = FieldList.FindIndex(x => x.Equals("fda"));
            if (id == -1) return false; // allow record
            var nw = DateTime.Now;
            var prefixes = new[]
            {
                nw.AddMonths(-3).ToString("yyyyMM"),
                nw.AddMonths(-2).ToString("yyyyMM"),
                nw.AddMonths(-1).ToString("yyyyMM"),
                DateTime.Now.AddMonths(0).ToString("yyyyMM"),
                DateTime.Now.AddMonths(1).ToString("yyyyMM"),
            };
            var current = TryGetValue(obj, id);
            if (string.IsNullOrEmpty(current)) return true;
            var ismatched = false;
            foreach (var prefix in prefixes)
            {
                if (current.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    ismatched = true;
                    break;
                }
            }
            return !ismatched;
        }

        private static string TryGetValue(List<string> list, int index)
        {
            if (index < 0) return string.Empty;
            if (index > list.Count - 1) return string.Empty;
            return list[index];
        }
        private static string RemoteAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(_remoteAddress)) return _remoteAddress;
                _remoteAddress = GetRemoteAddress();
                return _remoteAddress;
            }
        }
        private static string _remoteAddress = string.Empty;
        private static string GetRemoteAddress()
        {
            var model = HccConfigurationModel.GetModel().RemoteModel;
            if (string.IsNullOrEmpty(model.Url) ||
                string.IsNullOrEmpty(model.PostUrl))
                throw new NullReferenceException();
            return string.Concat(model.Url, model.PostUrl);
        }
        private static List<string> FieldList => _fields ??= definedFields.Split(',').ToList();
        private static List<string> _fields = null;

        private const string definedFields = "cdi,cas,fda,ins,cad,crt,cst,dst,bam,curr_off,curr_off_lit,curr_l_d," +
            "com_off,com_off_lit,com_l_d,gj_off,gj_off_lit,gj_l_d,nda,cnc,rea," +
            "def_nam,def_spn,def_rac,def_sex,def_dob,def_stnum,def_stnam,def_apt," +
            "def_cty,def_st,def_zip,aty_nam,aty_spn,aty_coc,aty_coc_lit,comp_nam," +
            "comp_agency,off_rpt_num,dispdt,disposition,CasCDI,sentence," +
            "def_citizen,bamexp,gj_dt,gj_crt,gj_cdp,def_pob";
    }
}