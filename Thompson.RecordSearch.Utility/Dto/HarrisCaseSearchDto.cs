using System;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class HarrisCaseSearchDto
    {
        private string _uniqueIndex;
        public string CaseNumber { get; set; }
        public string DateFiled { get; set; }
        public string Court { get; set; }
        public string UniqueIndex => GetUniqueIndex();

        private string GetUniqueIndex()
        {
            if (_uniqueIndex != null)
            {
                return _uniqueIndex;
            }
            string dateFiled = GetDate(DateFiled);
            string caseNumber = GetText(CaseNumber, "0000");
            string court = GetText(Court, "0000");

            _uniqueIndex = $"{dateFiled}~{caseNumber}~{court}";
            return _uniqueIndex;
        }

        private static string GetText(string text, string textDefault)
        {
            if (string.IsNullOrEmpty(text))
            {
                return textDefault;
            }
            return text;
        }

        private static string GetDate(string dateFiled)
        {
            var currentDate = DateTime.Now.ToString("s");
            if (string.IsNullOrEmpty(dateFiled))
            {
                dateFiled = currentDate;
            }
            if (DateTime.TryParse(dateFiled, out DateTime date))
            {
                return date.ToString("s");
            }
            return currentDate;
        }
    }
}
