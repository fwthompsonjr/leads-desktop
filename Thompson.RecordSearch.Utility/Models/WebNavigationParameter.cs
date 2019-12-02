using System.Collections.Generic;
using System.Xml;

namespace Thompson.RecordSearch.Utility.Models
{
    public class WebNavigationParameter
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<WebNavigationKey> Keys { get; set; }

        public List<WebNavInstruction> Instructions { get; set; }

        public List<WebNavInstruction> CaseInstructions { get; set; }
    }

    public class WebNavigationKey
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }


    public class WebNavInstruction
    {
        public string Name { get; set; }

        public string FriendlyName { get; set; }

        public string By { get; set; }

        public string Value { get; set; }

        public string CommandType { get; set; }
    }

    public class HLinkDataRow
    {
        public int WebsiteId { get; set; }
        public string Data { get; set; }
        
        public string Uri { get; set; }

        public string Defendant { get; set; }

        public string Address { get; set; }

        public bool IsMapped { get; set; }
	    public string Case { get; set; }
	    public string DateFiled { get; set; }
	    public string Court { get; set; }
	    public string CaseType { get; set; }
        public string CaseStyle { get; set; }
        public string PageHtml { get; internal set; }

        public string this[string fieldName]
        {
            get
            {
                if (fieldName.ToLower().Equals("case"))
                {
                    return Case;
                }
                if (fieldName.ToLower().Equals("datefiled"))
                {
                    return DateFiled;
                }
                if (fieldName.ToLower().Equals("court"))
                {
                    return Court;
                }
                if (fieldName.ToLower().Equals("casetype"))
                {
                    return CaseType;
                }
                if (fieldName.ToLower().Equals("casestyle"))
                {
                    return CaseStyle ?? (CaseStyle = GetFromData());
                }
                return string.Empty;
            }
            set {
                if (fieldName.ToLower().Equals("case"))
                {
                    Case = value;
                    return;
                }
                if (fieldName.ToLower().Equals("datefiled"))
                {
                    DateFiled = value;
                    return;
                }
                if (fieldName.ToLower().Equals("court"))
                {
                    Court = value;
                    return;
                }
                if (fieldName.ToLower().Equals("casetype"))
                {
                    CaseType = value;
                    return;
                }
                if (fieldName.ToLower().Equals("casestyle"))
                {
                    CaseStyle = value;
                    return;
                }
                /* set the specified index to value here */
            }
        }

        private string GetFromData()
        {
            if (string.IsNullOrEmpty(Data)) return null;
            var doc = new XmlDocument();
            doc.LoadXml(Data);
            var node = doc.FirstChild.ChildNodes[1];
            if (node == null) return string.Empty;
            return node.InnerText;
        }
    }
}
