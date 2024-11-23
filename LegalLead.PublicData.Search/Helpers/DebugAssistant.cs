using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    [ExcludeFromCodeCoverage(Justification = "This class only used in debug mode.")]
    internal static class DebugAssistant
    {
        public static string GetBo()
        {
            if (!Debugger.IsAttached) return string.Empty;
            var data = GetToken();
            var bo = ApiAuthenicationService.GetModel(data, out var _);
            var counties = bo.User.CountyData.ToInstance<List<LeadCountyTokenModel>>();
            counties.ForEach(c => c.MonthlyLimit = -1);
            bo.User.CountyData = counties.ToJsonString();
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

        private static readonly string accountToken = 
            "'MjAyNC0xMS0yM1QxNzo1MjowNHsiS2V5IjoiNmI5M2M2MjQtYTNmZC00ZWQ5',"
             + Environment.NewLine + "'LWFmZjktOTRkYzM1MzdjNWIyIiwiVXNlciI6eyJJZCI6ImZlZjI5NTMyLWE0',"
             + Environment.NewLine + "'ODctMTFlZi05OWNlLTBhZjdhMDFmNTJlOSIsIlVzZXJOYW1lIjoibGVhZC5h',"
             + Environment.NewLine + "'ZG1pbmlzdHJhdG9yIiwiVXNlckRhdGEiOiJ7XCJVc2VyTmFtZVwiOlwibGVh',"
             + Environment.NewLine + "'ZC5hZG1pbmlzdHJhdG9yXCIsXCJQaHJhc2VcIjpcInlvdS5jYW4ndC5yZWJv',"
             + Environment.NewLine + "'b3QudGhlLmFsYVwiLFwiVmVjdG9yXCI6XCJTNUlvZ3p0cWhNYmkxMmp1TjA1',"
             + Environment.NewLine + "'ckZnPT1cIixcIlRva2VuXCI6XCJpaitnTEJ4NWhuaElxenptZll6SlBWTTl6',"
             + Environment.NewLine + "'YVptTGtqRnVrN2cvYXY4b3Mvektja3kyQlh1R2JNMjJKSENZbEJvXCIsXCJD',"
             + Environment.NewLine + "'cmVhdGVEYXRlXCI6XCIyMDI0LTExLTE3VDAyOjAyOjI3XCIsXCJFbWFpbFwi',"
             + Environment.NewLine + "'OlwiZnJhbmsudGhvbXBzb24uanJAZ21haWwuY29tXCIsXCJJbnNlcnRGaWVs',"
             + Environment.NewLine + "'ZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixc',"
             + Environment.NewLine + "'IlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl0sXCJV',"
             + Environment.NewLine + "'cGRhdGVGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJW',"
             + Environment.NewLine + "'ZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwi',"
             + Environment.NewLine + "'SWRcIl0sXCJJZFwiOlwiZmVmMjk1MzItYTQ4Ny0xMWVmLTk5Y2UtMGFmN2Ew',"
             + Environment.NewLine + "'MWY1MmU5XCIsXCJUYWJsZU5hbWVcIjpcIkxFQURVU0VSXCIsXCJGaWVsZExp',"
             + Environment.NewLine + "'c3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRv',"
             + Environment.NewLine + "'a2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl19IiwiQ291',"
             + Environment.NewLine + "'bnR5RGF0YSI6Ilt7XCJMZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTEx',"
             + Environment.NewLine + "'ZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNvdW50eU5hbWVcIjpcIkRhbGxh',"
             + Environment.NewLine + "'c1wiLFwiTW9udGhseUxpbWl0XCI6MCxcIk1vZGVsXCI6XCJrZXJyaXBoaWxs',"
             + Environment.NewLine + "'aXBzbGF3QGdtYWlsLmNvbXxNQHJpMTAwN1wifSx7XCJMZWFkVXNlcklkXCI6',"
             + Environment.NewLine + "'XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNv',"
             + Environment.NewLine + "'dW50eU5hbWVcIjpcImRhbGxhc1wiLFwiTW9udGhseUxpbWl0XCI6LTEsXCJN',"
             + Environment.NewLine + "'b2RlbFwiOlwia2VycmlwaGlsbGlwc2xhd0BnbWFpbC5jb218TUByaTEwMDdc',"
             + Environment.NewLine + "'In0se1wiTGVhZFVzZXJJZFwiOlwiZmVmMjk1MzItYTQ4Ny0xMWVmLTk5Y2Ut',"
             + Environment.NewLine + "'MGFmN2EwMWY1MmU5XCIsXCJDb3VudHlOYW1lXCI6XCJCZXhhclwiLFwiTW9u',"
             + Environment.NewLine + "'dGhseUxpbWl0XCI6LTEsXCJNb2RlbFwiOlwia2VycmlwaGlsbGlwc2xhd0Bn',"
             + Environment.NewLine + "'bWFpbC5jb218TUByaTEwMDdcIn0se1wiTGVhZFVzZXJJZFwiOlwiZmVmMjk1',"
             + Environment.NewLine + "'MzItYTQ4Ny0xMWVmLTk5Y2UtMGFmN2EwMWY1MmU5XCIsXCJDb3VudHlOYW1l',"
             + Environment.NewLine + "'XCI6XCJIaWRhbGdvXCIsXCJNb250aGx5TGltaXRcIjotMSxcIk1vZGVsXCI6',"
             + Environment.NewLine + "'XCJrZXJyaXBoaWxsaXBzbGF3QGdtYWlsLmNvbXxNQHJpMTAwN1wifSx7XCJM',"
             + Environment.NewLine + "'ZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAx',"
             + Environment.NewLine + "'ZjUyZTlcIixcIkNvdW50eU5hbWVcIjpcIkVsIFBhc29cIixcIk1vbnRobHlM',"
             + Environment.NewLine + "'aW1pdFwiOi0xLFwiTW9kZWxcIjpcImtlcnJpcGhpbGxpcHNsYXdAZ21haWwu',"
             + Environment.NewLine + "'Y29tfE1AcmkxMDA3XCJ9LHtcIkxlYWRVc2VySWRcIjpcImZlZjI5NTMyLWE0',"
             + Environment.NewLine + "'ODctMTFlZi05OWNlLTBhZjdhMDFmNTJlOVwiLFwiQ291bnR5TmFtZVwiOlwi',"
             + Environment.NewLine + "'Rm9ydCBCZW5kXCIsXCJNb250aGx5TGltaXRcIjotMSxcIk1vZGVsXCI6XCJr',"
             + Environment.NewLine + "'ZXJyaXBoaWxsaXBzbGF3QGdtYWlsLmNvbXxNQHJpMTAwN1wifSx7XCJMZWFk',"
             + Environment.NewLine + "'VXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUy',"
             + Environment.NewLine + "'ZTlcIixcIkNvdW50eU5hbWVcIjpcIldpbGxpYW1zb25cIixcIk1vbnRobHlM',"
             + Environment.NewLine + "'aW1pdFwiOi0xLFwiTW9kZWxcIjpcImtlcnJpcGhpbGxpcHNsYXdAZ21haWwu',"
             + Environment.NewLine + "'Y29tfE1AcmkxMDA3XCJ9LHtcIkxlYWRVc2VySWRcIjpcImZlZjI5NTMyLWE0',"
             + Environment.NewLine + "'ODctMTFlZi05OWNlLTBhZjdhMDFmNTJlOVwiLFwiQ291bnR5TmFtZVwiOlwi',"
             + Environment.NewLine + "'R3JheXNvblwiLFwiTW9udGhseUxpbWl0XCI6LTEsXCJNb2RlbFwiOlwia2Vy',"
             + Environment.NewLine + "'cmlwaGlsbGlwc2xhd0BnbWFpbC5jb218TUByaTEwMDdcIn0se1wiTGVhZFVz',"
             + Environment.NewLine + "'ZXJJZFwiOlwiZmVmMjk1MzItYTQ4Ny0xMWVmLTk5Y2UtMGFmN2EwMWY1MmU5',"
             + Environment.NewLine + "'XCIsXCJDb3VudHlOYW1lXCI6XCJEZW50b25cIixcIk1vbnRobHlMaW1pdFwi',"
             + Environment.NewLine + "'Oi0xLFwiTW9kZWxcIjpcImtlcnJpcGhpbGxpcHNsYXdAZ21haWwuY29tfE1A',"
             + Environment.NewLine + "'cmkxMDA3XCJ9LHtcIkxlYWRVc2VySWRcIjpcImZlZjI5NTMyLWE0ODctMTFl',"
             + Environment.NewLine + "'Zi05OWNlLTBhZjdhMDFmNTJlOVwiLFwiQ291bnR5TmFtZVwiOlwiQ29sbGlu',"
             + Environment.NewLine + "'XCIsXCJNb250aGx5TGltaXRcIjotMSxcIk1vZGVsXCI6XCJwaGlsbGlwc2t8',"
             + Environment.NewLine + "'cGhpbGxpcHNrXCJ9LHtcIkxlYWRVc2VySWRcIjpcImZlZjI5NTMyLWE0ODct',"
             + Environment.NewLine + "'MTFlZi05OWNlLTBhZjdhMDFmNTJlOVwiLFwiQ291bnR5TmFtZVwiOlwiVGFy',"
             + Environment.NewLine + "'cmFudFwiLFwiTW9udGhseUxpbWl0XCI6LTEsXCJNb2RlbFwiOlwia2Vycmlw',"
             + Environment.NewLine + "'aGlsbGlwc2xhd0BnbWFpbC5jb218TUByaTEwMDdcIn0se1wiTGVhZFVzZXJJ',"
             + Environment.NewLine + "'ZFwiOlwiZmVmMjk1MzItYTQ4Ny0xMWVmLTk5Y2UtMGFmN2EwMWY1MmU5XCIs',"
             + Environment.NewLine + "'XCJDb3VudHlOYW1lXCI6XCJIYXJyaXNcIixcIk1vbnRobHlMaW1pdFwiOi0x',"
             + Environment.NewLine + "'LFwiTW9kZWxcIjpcImtlcnJpcGhpbGxpcHNsYXdAZ21haWwuY29tfE1Acmkx',"
             + Environment.NewLine + "'MDA3XCJ9LHtcIkxlYWRVc2VySWRcIjpcImZlZjI5NTMyLWE0ODctMTFlZi05',"
             + Environment.NewLine + "'OWNlLTBhZjdhMDFmNTJlOVwiLFwiQ291bnR5TmFtZVwiOlwiVHJhdmlzXCIs',"
             + Environment.NewLine + "'XCJNb250aGx5TGltaXRcIjotMSxcIk1vZGVsXCI6XCJrZXJyaXBoaWxsaXBz',"
             + Environment.NewLine + "'bGF3QGdtYWlsLmNvbXxNQHJpMTAwN1wifSx7XCJMZWFkVXNlcklkXCI6XCJm',"
             + Environment.NewLine + "'ZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNvdW50',"
             + Environment.NewLine + "'eU5hbWVcIjpcImNvbGxpblwiLFwiTW9udGhseUxpbWl0XCI6MCxcIk1vZGVs',"
             + Environment.NewLine + "'XCI6XCJwaGlsbGlwc2t8cGhpbGxpcHNrXCJ9XSIsIkluZGV4RGF0YSI6Ilt7',"
             + Environment.NewLine + "'XCJMZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3',"
             + Environment.NewLine + "'YTAxZjUyZTlcIixcIkNvdW50eUxpc3RcIjpcIi0xXCJ9XSJ9LCJSZWFzb24i',"
             + Environment.NewLine + "'OiJ1c2VyIGFjY291bnQgYWNjZXNzIGNyZWRlbnRpYWwifQ=='";

        private static readonly string userjs = "{" + Environment.NewLine +
            "'userName': 'lead.administrator'," + Environment.NewLine +
            "'token': [" + Environment.NewLine + accountToken + Environment.NewLine + "]" + Environment.NewLine +
            "}";

    }
}
