using System;
using System.Collections.Generic;
using System.Linq;

namespace LegalLead.PublicData.Search.Helpers
{
    public static class ElPasoCourtSelectionBuilder
    {
        public static string GetSelection(bool isJustice, DateTime startDate, int? parameterId = null)
        {
            const char comma = ',';
            const string prefixes = "1,2,3,4,5,6.1,6.2,7";
            if (!isJustice)
            {
                return $"{startDate:yyyy}DCV*";
            }
            var suffix = $"{startDate:yy}-0*";
            var arr = prefixes.Split(comma).Select(s => $"{s}{suffix}").ToList();
            if (parameterId.HasValue) return GetParameter(arr, parameterId.Value);
            return string.Join(comma, arr);
        }

        private static string GetParameter(List<string> parameterList, int id)
        {
            if (parameterList.Count == 0 || id < 0) return string.Empty;
            if (id > parameterList.Count - 1) return parameterList[0];
            return parameterList[id];
        }
    }
}
