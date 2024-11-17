using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    internal static class DebugAssistant
    {
        public static string GetBo()
        {
            var data = GetToken();
            var bo = ApiAuthenicationService.GetModel(data, out var _);
            return JsonConvert.SerializeObject(bo);
        }

        private static string GetToken()
        {
            var dto = GetDto();
            return string.Join("", dto.Token);
        }
        private static UserTmpDto GetDto()
        {
            if (Tmp != null) return Tmp;
            Tmp = userjs.ToInstance<UserTmpDto>() ?? new();
            return Tmp;
        }

        private static UserTmpDto Tmp = null;
        private sealed class UserTmpDto
        {
            public string UserName { get; set; } = string.Empty;
            public string[] Token { get; set; } = Array.Empty<string>();
        }

        private static readonly string userjs = "{" + Environment.NewLine +
            "'userName': 'lead.administrator'," + Environment.NewLine +
            "'token': [" + Environment.NewLine +
            "'MjAyNC0xMS0xN1QyMDoxNzo0N3siS2V5IjoiZWNiMTJjYTItYmFmMy00MWQ2LWJiYWQtZjJlMzJhNjQ1'," + Environment.NewLine +
            "'MDEyIiwiVXNlciI6eyJJZCI6ImZlZjI5NTMyLWE0ODctMTFlZi05OWNlLTBhZjdhMDFmNTJlOSIsIlVz'," + Environment.NewLine +
            "'ZXJOYW1lIjoibGVhZC5hZG1pbmlzdHJhdG9yIiwiVXNlckRhdGEiOiJ7XCJVc2VyTmFtZVwiOlwibGVh'," + Environment.NewLine +
            "'ZC5hZG1pbmlzdHJhdG9yXCIsXCJQaHJhc2VcIjpcInlvdS5jYW4ndC5yZWJvb3QudGhlLmFsYVwiLFwi'," + Environment.NewLine +
            "'VmVjdG9yXCI6XCJTNUlvZ3p0cWhNYmkxMmp1TjA1ckZnPT1cIixcIlRva2VuXCI6XCJpaitnTEJ4NWhu'," + Environment.NewLine +
            "'aElxenptZll6SlBWTTl6YVptTGtqRnVrN2cvYXY4b3Mvektja3kyQlh1R2JNMjJKSENZbEJvXCIsXCJD'," + Environment.NewLine +
            "'cmVhdGVEYXRlXCI6XCIyMDI0LTExLTE3VDAyOjAyOjI3XCIsXCJFbWFpbFwiOlwiZnJhbmsudGhvbXBz'," + Environment.NewLine +
            "'b24uanJAZ21haWwuY29tXCIsXCJJbnNlcnRGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNl'," + Environment.NewLine +
            "'XCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl0sXCJV'," + Environment.NewLine +
            "'cGRhdGVGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2Vu'," + Environment.NewLine +
            "'XCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl0sXCJJZFwiOlwiZmVmMjk1MzItYTQ4Ny0x'," + Environment.NewLine +
            "'MWVmLTk5Y2UtMGFmN2EwMWY1MmU5XCIsXCJUYWJsZU5hbWVcIjpcIkxFQURVU0VSXCIsXCJGaWVsZExp'," + Environment.NewLine +
            "'c3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVE'," + Environment.NewLine +
            "'YXRlXCIsXCJFbWFpbFwiLFwiSWRcIl19IiwiQ291bnR5RGF0YSI6Ilt7XCJMZWFkVXNlcklkXCI6XCJm'," + Environment.NewLine +
            "'ZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNvdW50eU5hbWVcIjpcImRhbGxh'," + Environment.NewLine +
            "'c1wiLFwiTW9kZWxcIjpcImtlcnJpcGhpbGxpcHNsYXdAZ21haWwuY29tfE1AcmkxMDA3XCJ9XSIsIklu'," + Environment.NewLine +
            "'ZGV4RGF0YSI6Ilt7XCJMZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAx'," + Environment.NewLine +
            "'ZjUyZTlcIixcIkNvdW50eUxpc3RcIjpcIi0xXCJ9XSJ9LCJSZWFzb24iOiJ1c2VyIGFjY291bnQgYWNj'," + Environment.NewLine +
            "'ZXNzIGNyZWRlbnRpYWwifQ=='" + Environment.NewLine +
            "]" + Environment.NewLine +
            "}";

    }
}
