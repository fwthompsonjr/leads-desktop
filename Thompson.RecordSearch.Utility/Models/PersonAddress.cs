using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Thompson.RecordSearch.Utility.Parsing;

namespace Thompson.RecordSearch.Utility.Models
{
    public class PersonAddress
    {
        #region FieldList Helpers

        private const string FieldNames = @"Name,FirstName,LastName,Zip," +
            @"Address1,Address2,Address3," +
            @"Case Number,Date Filed,Court," +
            @"Case Type,case style,plantiff,FName,LName";
        private string _fieldNames;
        private List<string> _fieldList;

        protected string LoweredFieldNames
        {
            get
            {
                return _fieldNames ?? (_fieldNames = FieldNames.ToLower(CultureInfo.CurrentCulture));
            }
        }

        public List<string> FieldList
        {
            get
            {
                return _fieldList ?? (
                  _fieldList = LoweredFieldNames.Split(',').ToList());
            }
        }

        #endregion

        #region Parse Plantiff Helpers


        private List<ICaseDataParser> GetCaseDataParses()
        {
            var caseType = string.IsNullOrEmpty(CaseType) ?
                string.Empty : CaseType.Trim();
            var caseStyle = string.IsNullOrEmpty(CaseStyle) ?
                string.Empty : CaseStyle.Trim();
            return new List<ICaseDataParser>
            {
                new ParseCaseDataByVersusStrategy{ CaseData = caseStyle },
                new ParseCaseDataByVsStrategy{ CaseData = caseStyle },
                new ParseCaseDataByEstateStrategy{ CaseData = caseStyle },
                new ParseCaseDataByInTheEstateStrategy{ CaseData = caseStyle },
                new ParseCaseOrderForForeclosure { CaseData = caseStyle },
                new ParseCaseInTheMatterMarriage { CaseData = caseStyle },
                new ParseCaseByVDot { CaseData = caseStyle },
                new ParseCaseNameChange { CaseData = caseStyle },
                new ParseMatterOfNameChange { CaseData = caseStyle },
                new ParseCaseInInterestMatch { CaseData = caseStyle },
                new ParseCaseExParteMatch { CaseData = caseStyle },
                new ParseNameChangeByCaseType { CaseData =
                string.Format(CultureInfo.CurrentCulture, "{0}| {1}", caseType, caseStyle) },
                new ParseProtectiveOrderCaseType { CaseData =
                string.Format(CultureInfo.CurrentCulture, "{0}| {1}", caseType, caseStyle) },
                new ParseCaseDataByInGuardianshipStrategy{ CaseData = caseStyle}
            };
        }

        #endregion
        #region Properties

        public string Name { get; set; }
        public string Zip { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CaseNumber { get; set; }
        public string DateFiled { get; set; }
        public string Court { get; set; }
        public string CaseType { get; set; }
        public string CaseStyle { get; set; }
        public string FirstName
        {
            get {
                return ParseFromFullName(0);
            }
        }
        public string LastName
        {
            get
            {
                return ParseFromFullName(1);
            }
        }

        public string Plantiff
        {
            get
            {
                return ParseFromCaseStyle(0);
            }

        }
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return false;
                if (string.IsNullOrEmpty(Zip)) return false;
                if (string.IsNullOrEmpty(Address1)) return false;
                return true;
            }
        }

        public string Status { get; set; }
        public string CalcFirstName { get; set; }
        public string CalcLastName { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Parses from full name.
        /// </summary>
        /// <param name="postionId">The postion identifier.</param>
        /// <returns></returns>
        private string ParseFromFullName(int postionId)
        {
            if (string.IsNullOrEmpty(Name)) return string.Empty;
            var fullName = Name;
            if (!fullName.Contains(',')) return postionId == 0 ? fullName : string.Empty;
            var nameParts = fullName.Split(',');
            var lastName = nameParts[0];
            var findIt = string.Format(CultureInfo.CurrentCulture, "{0}{1}", lastName, ',');
            var firstName = fullName.Remove(0, findIt.Length).Trim();
            if(firstName.Contains(' '))
            {
                nameParts = firstName.Split(' ');
                firstName = nameParts[0];
            }
            return postionId == 0 ? firstName : lastName;
        }

        /// <summary>
        /// Parses from case stlye.
        /// </summary>
        /// <param name="postionId">The postion identifier.</param>
        /// <returns></returns>
        private string ParseFromCaseStyle(int postionId)
        {
            if (postionId > 100) return string.Empty;
            if (string.IsNullOrEmpty(CaseStyle)) return string.Empty;
            var providers = GetCaseDataParses().FindAll(x => x.CanParse());
            if (!providers.Any()) return string.Empty;
            var response = string.Empty;
            foreach (var parser in providers)
            {
                response = parser.Parse().Plantiff;
                if (!string.IsNullOrEmpty(response))
                {
                    return response;
                }
            }
            return providers.First().Parse().Plantiff;
        }
        #endregion
        #region Indexer

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified index name.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="indexName">Name of the index.</param>
        /// <returns></returns>
        public string this[string indexName]
        {
            get
            {
                if (string.IsNullOrEmpty(indexName)) return string.Empty;
                var keyName = indexName.ToLower(CultureInfo.CurrentCulture);
                if (!FieldList.Contains(keyName)) return string.Empty;

                switch (keyName)
                {
                    case "name":
                        return Name;
                    case "zip":
                        return Zip;
                    case "address1":
                        return Address1;
                    case "address2":
                        return Address2;
                    case "address3":
                        return Address3;
                    case "case number":
                        return CaseNumber;
                    case "date filed":
                        return DateFiled;
                    case "court":
                        return Court;
                    case "case type":
                        return CaseType;
                    case "case style":
                        return CaseStyle;
                    case "firstname":
                        return FirstName;
                    case "lastname":
                        return LastName;
                    case "plantiff":
                        return Plantiff;
                    case "fname":
                        return CalcFirstName ?? "";
                    case "lname":
                        return CalcLastName ?? "";
                    default:
                        return string.Empty;
                }
            }
            set
            {

                if (string.IsNullOrEmpty(indexName)) return;
                var keyName = indexName.ToLower(CultureInfo.CurrentCulture);
                if (!FieldList.Contains(keyName)) return;

                switch (keyName)
                {
                    case "name":
                        Name = value;
                        return;
                    case "zip":
                        Zip = value;
                        return;
                    case "address1":
                        Address1 = value;
                        return;
                    case "address2":
                        Address2 = value;
                        return;
                    case "address3":
                        Address3 = value;
                        return;
                    case "case number":
                        CaseNumber = value;
                        return;
                    case "date filed":
                        DateFiled = value;
                        return;
                    case "court":
                        Court = value;
                        return;
                    case "case type":
                        CaseType = value;
                        return;
                    case "case style":
                        CaseStyle = value;
                        return;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                if (index < 0) return string.Empty;
                if (index > FieldList.Count - 1) return string.Empty;
                return this[FieldList[index]];
            }
            set
            {

                if (index < 0) return;
                if (index > FieldList.Count - 1) return;
                this[FieldList[index]] = value;
            }
        }

        #endregion
        #region Static Methods

        public static string ConvertFrom(string nodeName, XmlNode personNode)
        {
            if (string.IsNullOrEmpty(nodeName)) return string.Empty;
            if (personNode == null) return string.Empty;
            if (personNode.ChildNodes.Count < 2) return string.Empty;
            var lowerNodeName = nodeName.ToLower(CultureInfo.CurrentCulture);
            var addressNode = personNode.ChildNodes[1].InnerText.Trim();
            var addressFields = "address1,address2,address3,zip".Split(',').ToList(); 
            if (addressFields.Contains(lowerNodeName) && string.IsNullOrEmpty(addressNode)) return string.Empty;
            switch (lowerNodeName)
            {
                case "address1":
                    var first = GetAddressList(addressNode);
                    return first.First();
                case "address2":
                    var middle = GetAddressList(addressNode);
                    return GetMiddleAddress(middle);
                case "address3":
                    var second = GetAddressList(addressNode); 
                    if (second.Count < 2) return string.Empty;
                    return second.Last();
                case "zip":
                    var parts = addressNode
                        .Replace("<br/>", "~")
                        .Replace("<br />", "~").Split('~').ToList().Last();
                    return parts.Split(' ').Last();
                case "case":
                    var caseNode = personNode.SelectSingleNode("case");
                    if (caseNode == null)
                    {
                        return string.Empty;
                    }
                    return ((XmlCDataSection)caseNode.ChildNodes[0]).Data;
                case "court":
                    var courtNode = personNode.SelectSingleNode("court");
                    if (courtNode == null) return string.Empty;
                    return ((XmlCDataSection)courtNode.ChildNodes[0]).Data;
                case "datefiled":
                    var filedNode = personNode.SelectSingleNode("dateFiled");
                    if (filedNode == null) return string.Empty;
                    return ((XmlCDataSection)filedNode.ChildNodes[0]).Data;
                case "casetype":
                    var caseTypeNode = personNode.SelectSingleNode("caseType");
                    if (caseTypeNode == null) return string.Empty;
                    return ((XmlCDataSection)caseTypeNode.ChildNodes[0]).Data;
                case "casestyle":
                    var caseStyleNode = personNode.SelectSingleNode("caseStyle");
                    if (caseStyleNode == null) return string.Empty;
                    return ((XmlCDataSection)caseStyleNode.ChildNodes[0]).Data;
                default:
                    break;
            }
            return string.Empty;
        }

        private static string GetMiddleAddress(List<string> middle)
        {
            if (middle.Count < 3) return string.Empty;
            var addr = string.Empty;
            for (int i = 1; i < middle.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(addr))
                {
                    addr += ", ";
                }
                addr += middle[i];
            }
            return addr;
        }

        public static PersonAddress ConvertFrom(XmlNode personNode)
        {
            if (personNode == null) return null;
            var addr = new PersonAddress()
            {
                Name = TryGet(personNode, "name"),
                Zip = TryGet(personNode, "address", "zip"),
                Address1 = TryGet(personNode, "address", "addressA"),
                Address2 = TryGet(personNode, "address", "addressB"),
                Address3 = TryGet(personNode, "address", "addressC"),
                CaseNumber = TryGet(personNode, "case", ""),
                DateFiled = TryGet(personNode, "dateFiled", ""),
                Court = TryGet(personNode, "court", ""),
                CaseType = TryGet(personNode, "caseType", "")
            };

            return addr;
        }

        private static string TryGet(XmlNode personNode, string nodeName)
        {
            var node = personNode.SelectSingleNode(nodeName);
            if (node == null) return string.Empty;
            return node.InnerText;
        }

        private static string TryGet(XmlNode personNode, string nodeName, string childNodeName)
        {
            try
            {
                var node = personNode.SelectSingleNode(nodeName);
                if (node == null) return string.Empty;
                if (string.IsNullOrEmpty(childNodeName))
                {
                    return ((XmlCDataSection)(node.ChildNodes[0])).Data;
                }
                else
                {
                    if (childNodeName.StartsWith("address", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var fullAddress = ((XmlCDataSection)(node.ChildNodes[0])).Data;
                        var addressList = GetAddressList(fullAddress);
                        switch (childNodeName)
                        {
                            case "addressA":
                                return addressList.First();
                            case "addressC":
                                return addressList.Last();
                            case "addressB":
                                return GetMiddleAddress(addressList);
                            default:
                                return string.Empty;
                        }
                    }
                }
                var childNode = node.SelectSingleNode(childNodeName);
                if (childNode == null) return string.Empty;
                return ((XmlCDataSection)(childNode.ChildNodes[0])).Data;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return string.Empty;
            }
        }

        private static List<string> GetAddressList(string address)
        {
            var result = new List<string>();
            var data = address
                .Replace("<br>", "~")
                .Replace("<br/>", "~")
                .Replace("<br />", "~").Split('~').ToList();
            foreach (var addr in data)
            {
                if (string.IsNullOrEmpty(addr)) continue;
                if (string.IsNullOrEmpty(addr.Trim())) continue;
                result.Add(addr.Trim());
            }
            return result;
        }
        #endregion
    }
}
