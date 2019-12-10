using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
    public class XmlContentHolder
    {
        public string FileName { get; set; }

        public XmlDocument Document { get; set; }

        public XmlNode Data
        {
            get
            {
                if (Document == null) return null;
                return Document.DocumentElement.SelectSingleNode(@"//results/result[@name='casedata']");
            }
        }

        public XmlNode PeopleData
        {
            get
            {
                if (Document == null) return null;
                return Document.DocumentElement.SelectSingleNode(@"//results/result[@name='peopledata']");
            }
        }


        public XmlNode People
        {
            get
            {
                if (Document == null) return null;
                return Document.DocumentElement.SelectSingleNode(@"//results/result[@name='person']/people");
            }
        }

        public List<PersonAddress> GetPersonAddresses()
        {
            var people = People;
            if (people == null) return null;
            if (!people.HasChildNodes) return new List<PersonAddress>();
            var addressList = new List<PersonAddress>();
            foreach (var personNode in people.ChildNodes.Cast<XmlNode>())
            {
                var addressPerson = PersonAddress.ConvertFrom(personNode);
                if (addressPerson == null) continue;
                if (addressPerson.IsValid)
                {
                    string caseSytle = addressPerson["CaseStyle"];
                    addressList.Add(addressPerson);
                }
                else if(!string.IsNullOrEmpty(personNode.ChildNodes[0].InnerText))
                {
                    addressPerson = new PersonAddress
                    {
                        Name = personNode.ChildNodes[0].InnerText,
                        Address1 = PersonAddress.ConvertFrom("address1", personNode),
                        Address2 = PersonAddress.ConvertFrom("address2", personNode),
                        Address3 = PersonAddress.ConvertFrom("address3", personNode),
                        Zip = PersonAddress.ConvertFrom("zip", personNode),
                        CaseNumber = PersonAddress.ConvertFrom("case", personNode),
                        CaseStyle = PersonAddress.ConvertFrom("caseStyle", personNode),
                        CaseType = PersonAddress.ConvertFrom("caseType", personNode),
                        Court = PersonAddress.ConvertFrom("court", personNode),
                        DateFiled = PersonAddress.ConvertFrom("dateFiled", personNode)
                    };
                    addressList.Add(addressPerson);
                }
                if (!addressPerson.IsValid && !string.IsNullOrEmpty(addressPerson.Name))
                {
                    addressPerson.Address1 = "No Address Found";
                    addressPerson.Address3 = "Not, Available 00000";
                    addressPerson.Zip = string.Empty;
                    addressPerson.CaseNumber = PersonAddress.ConvertFrom("case", personNode);
                    addressPerson.CaseStyle = PersonAddress.ConvertFrom("caseStyle", personNode);
                    addressPerson.CaseType = PersonAddress.ConvertFrom("caseType", personNode);
                    addressPerson.Court = PersonAddress.ConvertFrom("court", personNode);
                    addressPerson.DateFiled = PersonAddress.ConvertFrom("dateFiled", personNode);
                }
            }
            return addressList;
        }

        protected StringBuilder CharacterData { get; set; }
        protected StringBuilder CharacterPeople { get; set; }
        public int Id { get; internal set; }

        public void Append(Models.HLinkDataRow dta)
        {
            if (dta == null) throw new ArgumentNullException(nameof(dta));
            if (CharacterData == null) CharacterData = new StringBuilder("");
            if (CharacterPeople == null) CharacterPeople = new StringBuilder("");
            var openTable = @"<table style='border-collapse: collapse; border: 1px solid black;'>";
            var opnTable = openTable + @"<tr style='border: 1px solid black;'>";
            opnTable = opnTable + Environment.NewLine;
            opnTable = opnTable + @"<td colspan='2' style='border: 1px solid black;'>Case Results</td></tr>";
            opnTable = opnTable + Environment.NewLine;
            const string closeTable = @"</table>";
            const string fmttbl = @"{0}{1}{2}";
            var tableRow = CleanHtml(dta.Data);
            CharacterData.AppendLine(dta.Data);
            var cnode = (XmlCDataSection)Data.FirstChild;
            cnode.Data = string.Format(fmttbl, 
                openTable,
                CharacterData.ToString(),
                closeTable);
            var person = People.FirstChild.CloneNode(true);
            if(dta.IsCriminal & dta.IsMapped)
            {
                // System.Diagnostics.Debugger.Break();
            }
            person.ChildNodes[0].InnerText = dta.Defendant;
            var addressNode = person.ChildNodes[1];
            ((XmlCDataSection)(addressNode.FirstChild)).Data = dta.Address;
            addressNode = ParseAddressInformation(dta.Address, addressNode);
            person = MapExtraData(dta, person);
            People.AppendChild(person);

            var personHtml = PersonHtml(dta);
            if (!string.IsNullOrEmpty(personHtml))
            {
                personHtml = personHtml.Replace(@"<tr>", @"<tr style='border: 1px solid black;'>");
                CharacterPeople.AppendLine(personHtml);
                cnode = (XmlCDataSection)PeopleData.FirstChild;
                cnode.Data = string.Format(fmttbl,
                    opnTable,
                    CharacterPeople.ToString(),
                    closeTable);
                
            }
            Document.Save(FileName);
        }

        private static XmlNode ParseAddressInformation(string address, XmlNode addressNode)
        {
            const string lineBreak = @"<br/>";
            if (addressNode == null) return addressNode;
            if (string.IsNullOrEmpty(address)) return addressNode;
            address = address.Trim();
            if (string.IsNullOrEmpty(address)) return addressNode;

            var addresses = address.Split(new string[] { lineBreak }, StringSplitOptions.None).ToList();
            if (addresses.Count <= 1 || addresses.Count > 4) return addressNode;

            var zipNode = addressNode.SelectSingleNode("zip");
            var addressA = addressNode.SelectSingleNode("addressA");
            var addressB = addressNode.SelectSingleNode("addressB");
            var addressC = addressNode.SelectSingleNode("addressC");
            // append a cdata section
            var ndes = new List<XmlNode>{ zipNode , addressA , addressB , addressC };
            foreach (var item in ndes)
            {
                var cdta = item.OwnerDocument.CreateCDataSection(string.Empty);
                item.AppendChild(cdta);
            }
            addresses = RemoveBlank(addresses);
            var firstItem = addresses.First().Trim().ToUpper(CultureInfo.CurrentCulture);
            var lastItem = addresses.Last().Trim().ToUpper(CultureInfo.CurrentCulture);
            var middleItem = GetMiddleAddress(addresses).Trim().ToUpper(CultureInfo.CurrentCulture);
            ((XmlCDataSection)(zipNode.FirstChild)).Data = ParseZipCode(lastItem);
            ((XmlCDataSection)(addressC.FirstChild)).Data = lastItem;
            ((XmlCDataSection)(addressA.FirstChild)).Data = firstItem;
            ((XmlCDataSection)(addressB.FirstChild)).Data = middleItem;
            return addressNode;

        }

        private static string ParseZipCode(string lastItem)
        {
            var pieces = lastItem.Split(' ').ToList();
            return pieces.Last();
        }

        private static XmlNode MapExtraData(HLinkDataRow dta, XmlNode person)
        {
            if (!dta.IsMapped) return person;
            var fields = "case,dateFiled,court,caseType,caseStyle".Split(',').ToList();
            foreach (var fieldName in fields)
            {
                var targetNode = person.SelectSingleNode(fieldName);
                if (targetNode == null) continue;
                ((XmlCDataSection)targetNode.FirstChild).Data = dta[fieldName]; 
            }
            return person;
        }

        private static string CleanHtml(string rawHtml)
        {
            if (string.IsNullOrEmpty(rawHtml)) return rawHtml;
            if (!rawHtml.ToLower(CultureInfo.CurrentCulture).Contains("href=")) return rawHtml;
            try
            {
                var doc = XmlDocProvider.GetDoc(rawHtml);
                var firstChild = doc.DocumentElement.FirstChild;
                var hlink = firstChild.SelectSingleNode("a");
                if (hlink == null) return rawHtml;
                firstChild.InnerXml = string.Format("<span>{0}</span>", hlink.InnerText);
                return doc.OuterXml;
            }
            catch (Exception)
            {
                return rawHtml;
            }
        }

        private static string PersonHtml(Models.HLinkDataRow dta)
        {
            if (string.IsNullOrEmpty(dta.Defendant)) return string.Empty;
            if (string.IsNullOrEmpty(dta.Address)) return string.Empty;
            const string rfmt = @"<tr><td caseNumber='{2}'>{0}</td><td>{1}</tr>";
            return string.Format(rfmt, dta.Defendant, dta.Address, dta.Case ?? "");
        }


        private static List<string> RemoveBlank(List<string> data)
        {
            var result = new List<string>();
            foreach (var addr in data)
            {
                if (string.IsNullOrEmpty(addr)) continue;
                if (string.IsNullOrEmpty(addr.Trim())) continue;
                result.Add(addr.Trim());
            }
            return result;
        }


        private static string GetMiddleAddress(List<string> middle)
        {
            if (middle.Count < 3) return string.Empty;
            var addr = string.Empty;
            for (int i = 1; i < middle.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(addr))
                {
                    addr = addr + ", ";
                }
                addr = addr + middle[i];
            }
            return addr;
        }
    }
}
