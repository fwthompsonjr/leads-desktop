using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db = Harris.Criminal.Db;

namespace LegalLead.PublicData.Search.Classes
{
    public class HccDetailItem
    {
		
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
		
		public static string GetValue(int indexId){
			var culture = CultureInfo.CurrentCulture;
			var dataValue = string.Empty;

			var datelist = Db.Startup.CaseStyles.DataList?
				.Select(m =>
				{
					DateTime? resolvedDate = default;
					if (DateTime.TryParseExact(m.FileDate, "m/d/yyyy", culture, DateTimeStyles.None, out DateTime filingDate))
					{
						resolvedDate = filingDate;
					}
					return resolvedDate;
				})
				.Where(d => d.HasValue)
				.Distinct()
				.Select(a => a.GetValueOrDefault());
			switch (indexId)
			{
				case 100:
					// HCC Database - Latest Header
					var lastHeader = Db.Startup.Downloads.DataList?.Max(f => f.CreateDate);
					if (lastHeader.HasValue) dataValue = lastHeader.Value.ToString("g", culture);
					break;
				case 200:
					// HCC Database - Latest Detail
					var mxDate = datelist?.Max();
					if (mxDate.HasValue) dataValue = mxDate.Value.ToString("g", culture);
					break;
				// HCC Database - Detail Date Range
				case 300:
					var mxxDate = datelist?.Max();
					var mnDate = datelist?.Min();
					var startDate = mnDate.HasValue ? mnDate.Value.ToString("g", culture) : "n/a";
					var endDate = mxxDate.HasValue ? mxxDate.Value.ToString("g", culture) : "n/a";
					dataValue = $"{startDate} to {endDate}";
					break;
			}
			return dataValue;
		}
    }
}
