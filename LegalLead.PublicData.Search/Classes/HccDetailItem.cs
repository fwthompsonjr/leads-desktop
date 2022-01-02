using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
					if (DateTime.TryParseExact(m.FileDate, "M/d/yyyy", culture, DateTimeStyles.None, out DateTime filingDate))
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
					var lastHeader = Db.Startup.Downloads.DataList?.Max(f => f.MaxFilingDate);
					if (lastHeader.HasValue) dataValue = lastHeader.Value.ToString("D", culture);
					break;
				case 125:
					// HCC Database - Earliest Header
					var eheader = Db.Startup.Downloads.DataList?.Min(f => f.MinFilingDate);
					if (eheader.HasValue) dataValue = eheader.Value.ToString("D", culture);
					break;
				case 200:
					// HCC Database - Earliest Detail
					var mnnDate = datelist?.Min();
					if (mnnDate.HasValue) dataValue = mnnDate.Value.ToString("D", culture);
					break;
				case 300:
					// HCC Database - Latest Detail
					var mxxDate = datelist?.Max();
					if (mxxDate.HasValue) dataValue = mxxDate.Value.ToString("D", culture);
					break;
				case 400:
					// HCC Database - Headers
					var headers = Db.Startup.Downloads.FileNames.Select(a => Path.GetFileNameWithoutExtension(a));
					dataValue = string.Join(Environment.NewLine, headers);
					break;
				case 500:
					// HCC Database - Details
					var dbdetails = Db.Startup.CaseStyles.FileNames.Select(a => Path.GetFileNameWithoutExtension(a));
					dataValue = string.Join(Environment.NewLine, dbdetails);
					break;
			}
			return dataValue;
		}
    }
}
