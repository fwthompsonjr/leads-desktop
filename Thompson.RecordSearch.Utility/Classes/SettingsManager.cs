using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{

    public static class XmlDocProvider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is being passed to caller and must not be disposed.")]
        public static XmlDocument GetDoc(string xml)
        {
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            System.IO.StringReader sreader = new System.IO.StringReader(xml);
            XmlReader reader = XmlReader.Create(sreader, new XmlReaderSettings() { XmlResolver = null });
            doc.Load(reader);
            return doc;
        }

        public static XmlDocument Load(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                var content = reader.ReadToEnd();
                return GetDoc(content);
            }
        }
    }
    /// <summary>Class definition for settings reader utility which reads the application settings xml file to map parameters to the search process.</summary>
    public class SettingsManager
    {

        #region Fields

        // private string _fileContent;
        private string _layoutContent;
        private string _excelFileName;

        #endregion

        #region Properties

        /// <summary>Gets the file name for the excel format file.</summary>
        /// <value>The excel format file.</value>
        public string ExcelFormatFile
        {
            get { return _excelFileName ?? (_excelFileName = GetExcelFileName()); }
        }


        /// <summary>
        /// Gets the content of the xml source file.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public static string Content
        {
            get { return LoadFile("settings.xml"); }
        }

        /// <summary>
        /// Gets the layout of the xml source file.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Layout
        {
            get { return _layoutContent ?? (_layoutContent = LoadFile("caselayout.xml")); }
        }


        #endregion



        /// <summary>
        /// Reads the settings file to gets the navigation 
        /// settings for record search processes.
        /// </summary>
        /// <returns></returns>
        public static List<WebNavigationParameter> GetNavigation()
        {
            var data = Content;

            var doc = XmlDocProvider.GetDoc(data);
            if (doc.DocumentElement == null)
            {
                return null;
            }

            var parent = doc.DocumentElement.SelectSingleNode("setting[@name='Websites']");
            var response = new List<WebNavigationParameter>();
            foreach (var node in parent.ChildNodes)
            {
                response.Add(MapFrom((XmlNode)node));
            }
            return response;

        }

        /// <summary>
        /// Gets the column layouts from settings file.
        /// </summary>
        /// <param name="id">The website identifier.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        public static List<ExcelColumnLayout> GetColumnLayouts(int id, string sectionName)
        {
            var data = Content;
            var doc = XmlDocProvider.GetDoc(data);
            if (doc.DocumentElement == null)
            {
                return null;
            }

            var parent = doc.DocumentElement.SelectSingleNode(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "layouts/layout[@id='{0}' and @name='{1}']",
                id != 1 ? 1 : id,
                sectionName));
            if (id == 30)
            {
                parent = doc.DocumentElement.SelectSingleNode(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "layouts/layout[@id='{0}' and @name='{1}']",
                id,
                sectionName));
            }
            if (parent == null)
            {
                return null;
            }

            var columnNode = parent.FirstChild;
            if (columnNode == null)
            {
                return null;
            }

            if (!columnNode.HasChildNodes)
            {
                return null;
            }

            var layoutList = new List<ExcelColumnLayout>();
            var columnList = columnNode.ChildNodes.Cast<XmlNode>().ToList();
            foreach (var column in columnList)
            {
                layoutList.Add(new ExcelColumnLayout
                {
                    Name = column.Attributes.GetNamedItem("name").InnerText,
                    ColumnWidth = Convert.ToInt32(
                        column.Attributes.GetNamedItem("columnWidth").InnerText,
                        CultureInfo.CurrentCulture.NumberFormat)
                });
            }
            return layoutList;
        }


        /// <summary>
        /// Generates the xml output file to hold record search results.
        /// </summary>
        /// <param name="settingFile">The setting file.</param>
        /// <returns></returns>
        public XmlContentHolder GetOutput(WebInteractive settingFile)
        {
            if (settingFile == null)
            {
                throw new ArgumentNullException(nameof(settingFile));
            }

            const string dfmt = "MMddyyyy";
            var fileName = GetFileName(settingFile);
            var targetFile = fileName;
            var idx = 0;
            var cultureInfo = CultureInfo.CurrentCulture;
            var numberInfo = cultureInfo.NumberFormat;
            while (File.Exists(targetFile))
            {
                idx += 1;
                var cleaned = Path.GetFileNameWithoutExtension(fileName);
                cleaned = string.Format(cultureInfo, "{0}_{1}.xml", cleaned, idx.ToString("D9", numberInfo));
                targetFile = string.Format(cultureInfo, "{0}/{1}", Path.GetDirectoryName(fileName), cleaned);
            }
            fileName = targetFile;
            using (var sw = new StreamWriter(fileName))
            {
                sw.Write(Layout);
                sw.Close();
            }
            var content = string.Empty;
            using (var reader = new StreamReader(fileName))
            {
                content = reader.ReadToEnd();
            }
            var doc = XmlDocProvider.GetDoc(content);
            var nde = doc.DocumentElement.SelectSingleNode(@"parameters");
            var nds = new List<XmlNode>(nde.ChildNodes.Cast<XmlNode>());
            foreach (var item in nds)
            {
                var attrName = item.Attributes.GetNamedItem("name").Value;
                switch (attrName)
                {
                    case "Website":
                        item.InnerText = settingFile.Parameters.Name;
                        break;
                    case "StartDate":
                        item.InnerText = settingFile.StartDate.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
                        break;
                    case "EndDate":
                        item.InnerText = settingFile.EndingDate.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
                        break;
                    case "SearchDate":
                        item.InnerText = DateTime.Now.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
                        break;
                    default:
                        break;
                }
            }




            doc.Save(fileName);

            // check for null
            return new XmlContentHolder
            {
                Id = settingFile.Parameters.Id,
                FileName = fileName,
                Document = doc
            };
        }

        private static WebNavigationParameter MapFrom(XmlNode node)
        {
            var parameter = new WebNavigationParameter
            {
                Id = Convert.ToInt32(
                    node.Attributes.GetNamedItem("id").Value,
                    CultureInfo.CurrentCulture.NumberFormat),
                Name = node.Attributes.GetNamedItem("name").Value,
                Keys = new List<WebNavigationKey>(),
                Instructions = new List<WebNavInstruction>(),
                CaseInstructions = new List<WebNavInstruction>()
            };
            foreach (var item in node.ChildNodes)
            {
                var nde = (XmlNode)item;
                parameter.Keys.Add(new WebNavigationKey
                {
                    Name = nde.Attributes.GetNamedItem("name").Value,
                    Value = ((XmlCDataSection)nde.FirstChild).Data
                });
            }
            var qpath = string.Format(
                CultureInfo.CurrentCulture,
                "directions/instructions[@id={0}]", parameter.Id);

            var instructions = node.OwnerDocument
                .DocumentElement.SelectSingleNode(qpath);
            if (instructions == null)
            {
                return parameter;
            }

            foreach (var item in instructions.ChildNodes)
            {
                var nde = (XmlNode)item;
                parameter.Instructions.Add(new WebNavInstruction
                {
                    Name = nde.Attributes.GetNamedItem("name").Value,
                    By = nde.Attributes.GetNamedItem("By").Value,
                    CommandType = nde.Attributes.GetNamedItem("type").Value,
                    FriendlyName = nde.Attributes.GetNamedItem("FriendlyName").Value,
                    Value = ((XmlCDataSection)nde.FirstChild).Data
                });
            }

            qpath = string.Format(
                CultureInfo.CurrentCulture,
                "directions/caseInspection[@id={0}]", parameter.Id);
            instructions = node.OwnerDocument
                .DocumentElement.SelectSingleNode(qpath);
            if (instructions == null)
            {
                return parameter;
            }

            foreach (var item in instructions.ChildNodes)
            {
                var nde = (XmlNode)item;
                parameter.CaseInstructions.Add(new WebNavInstruction
                {
                    Name = nde.Attributes.GetNamedItem("name").Value,
                    By = nde.Attributes.GetNamedItem("By").Value,
                    CommandType = nde.Attributes.GetNamedItem("type").Value,
                    FriendlyName = nde.Attributes.GetNamedItem("FriendlyName").Value,
                    Value = ((XmlCDataSection)nde.FirstChild).Data
                });
            }

            return parameter;
        }
        /// <summary>
        /// Loads the resource xml file from data folder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>the contents of specified file as string</returns>
        private static string LoadFile(string fileName)
        {
            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            var targetFile = new Uri(string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\xml\{1}", execName, fileName)).AbsolutePath;
            return !File.Exists(targetFile) ?
                GetXmlContent(fileName) :
                File.ReadAllText(targetFile);
        }

        private static string GetXmlContent(string fileName)
        {
            const string tilde = "~";
            var quote = '"'.ToString();

            switch (fileName)
            {
                case "settings.xml":
                    var sb = new StringBuilder();
                    sb.AppendLine("<?xml version=~1.0~ encoding=~utf-8~ ?>".Replace(tilde, quote));
                    sb.AppendLine("<settings>".Replace(tilde, quote));
                    sb.AppendLine("<setting name=~Websites~>".Replace(tilde, quote));
                    sb.AppendLine("<keys id=~1~ name=~Denton County~>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~baseUri~><![CDATA[http://justice1.dentoncounty.com/PublicAccess/Search.aspx]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=------%20All%20District%20Courts%20------]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~hlinkUri--inactive~><![CDATA[http://justice1.dentoncounty.com/PublicAccessDC/]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~dateRangeMaxDays~><![CDATA[5]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~startDate~><![CDATA[]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~endDate~><![CDATA[]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~isCriminalSearch~><![CDATA[0]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~criminalCaseInclusion~><![CDATA[0]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~criminalLinkQuery~><![CDATA[//a[text()='JP & County Court: Criminal Case Records']]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~nonCriminalLinkQuery~><![CDATA[//a[text()='JP & County Court: Civil, Family & Probate Case Records']]]></key>".Replace(tilde, quote));
                    sb.AppendLine("</keys>".Replace(tilde, quote));
                    sb.AppendLine("<keys id=~10~ name=~Tarrant County~>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~baseUri~><![CDATA[https://odyssey.tarrantcounty.com/PublicAccess/default.aspx]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=------%20All%20District%20Courts%20------]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~dateRangeMaxDays~><![CDATA[5]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~startDate~><![CDATA[]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~endDate~><![CDATA[]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~navigation.control.file~><![CDATA[tarrantCountyMapping_1]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~navigation.control.alternate.file~><![CDATA[tarrantCountyMapping_2]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~hlinkUri~><![CDATA[https://odyssey.tarrantcounty.com/PublicAccess/{0}]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~caseTypeSelectedIndex~><![CDATA[0]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~criminalCaseInclusion~><![CDATA[1]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~personNodeXpath~><![CDATA[//*[@id='PIr11']]]></key>".Replace(tilde, quote));
                    sb.AppendLine("</keys>".Replace(tilde, quote));
                    sb.AppendLine("<keys id=~20~ name=~Collin County~>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~baseUri~><![CDATA[http://cijspub.co.collin.tx.us/secure/login.aspx]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=------%20All%20District%20Courts%20------]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~dateRangeMaxDays~><![CDATA[5]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~startDate~><![CDATA[]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~endDate~><![CDATA[]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~navigation.control.file~><![CDATA[collinCountyMapping_1]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~hlinkUri~><![CDATA[http://cijspub.co.collin.tx.us/Secure/{0}]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~personNodeXpath~><![CDATA[//*[@id='PIr11']]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~personParentXath~><![CDATA[./..]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~childTdXath~><![CDATA[/html/body/table[4]/tbody/tr[{0}]/td]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~caseTypeSelectedIndex~><![CDATA[0]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~searchTypeSelectedIndex~><![CDATA[0]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~criminalCaseInclusion~><![CDATA[1]]></key>".Replace(tilde, quote));
                    sb.AppendLine("</keys>".Replace(tilde, quote));
                    sb.AppendLine("</setting>".Replace(tilde, quote));
                    sb.AppendLine("<directions>".Replace(tilde, quote));
                    sb.AppendLine("<instructions id=~1~>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~WaitForElementExist~ By=~Id~ FriendlyName=~Court-Type-Selection~><![CDATA[sbxControlID2]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~SetComboIndex~ By=~Id~ FriendlyName=~Court-Type-Selection~><![CDATA[sbxControlID2,?SetComboIndex]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~WaitForElementExist~ By=~XPath~ FriendlyName=~Search-Hyperlink~><![CDATA[//a[@class='ssSearchHyperlink'][contains(text(),'County Court: Civil, Family')]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~Click~ By=~XPath~ FriendlyName=~Search-Hyperlink~><![CDATA[//a[@class='ssSearchHyperlink'][contains(text(),'County Court: Civil, Family')]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~WaitForNavigation~ By=~~ FriendlyName=~~><![CDATA[ ]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~WaitForElementExist~ By=~Id~ FriendlyName=~District-Type-Selection~><![CDATA[sbxControlID2]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~SetComboIndex~ By=~Id~ FriendlyName=~District-Type-Selection~><![CDATA[sbxControlID2,?SetComboIndex]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~WaitForElementExist~ By=~XPath~ FriendlyName=~District-Hyperlink~><![CDATA[//a[@class='ssSearchHyperlink'][contains(text(),'District Clerk Civil')]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~Click~ By=~XPath~ FriendlyName=~District-Hyperlink~><![CDATA[//a[@class='ssSearchHyperlink'][contains(text(),'District Clerk Civil')]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~Click~ By=~Id~ FriendlyName=~Date-Filed-Radio-Button~><![CDATA[DateFiled]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~SetControlValue~ By=~Id~ FriendlyName=~Date-Filed-On-TextBox~><![CDATA[DateFiledOnAfter,?StartDate]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~SetControlValue~ By=~Id~ FriendlyName=~Date-Filed-Before-TextBox~><![CDATA[DateFiledOnBefore,?EndingDate]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~ClickElement~ By=~Id~ FriendlyName=~Search-Records-Submit~><![CDATA[SearchSubmit]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~WaitForNavigation~ By=~~ FriendlyName=~~><![CDATA[ ]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~GetCases~ name=~GetElement~ By=~XPath~ FriendlyName=~Search-Records-Result-Table~><![CDATA[/html/body/table[4]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("</instructions>".Replace(tilde, quote));
                    sb.AppendLine("<caseInspection id=~1~ type=~normal~>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[3]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]/div[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("</caseInspection>".Replace(tilde, quote));
                    sb.AppendLine("<caseInspection id=~10~ type=~normal~>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[4]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[4]/div[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[5]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Style-Selection~><![CDATA[td[3]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("</caseInspection>".Replace(tilde, quote));
                    sb.AppendLine("<caseInspection id=~10~ type=~probate~>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[3]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]/div[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("</caseInspection>".Replace(tilde, quote));
                    sb.AppendLine("<caseInspection id=~20~ type=~normal~>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[4]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[4]/div[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[5]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Style-Selection~><![CDATA[td[3]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("</caseInspection>".Replace(tilde, quote));
                    sb.AppendLine("<caseInspection id=~20~ type=~probate~>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[3]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]/div[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("</caseInspection>".Replace(tilde, quote));
                    sb.AppendLine("<caseInspection id=~20~ type=~justice~>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[3]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]/div[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]/div[1]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("<command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[2]]]></command>".Replace(tilde, quote));
                    sb.AppendLine("</caseInspection>".Replace(tilde, quote));
                    sb.AppendLine("</directions>".Replace(tilde, quote));
                    sb.AppendLine("<layouts>".Replace(tilde, quote));
                    sb.AppendLine("<layout id=~1~ name=~caselayout~>".Replace(tilde, quote));
                    sb.AppendLine("<columns>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Number~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Name~ columnWidth=~80~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Filed/Location/Judical Officer~ columnWidth=~40~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Type/Status~ columnWidth=~40~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Style~ columnWidth=~80~/>".Replace(tilde, quote));
                    sb.AppendLine("</columns>".Replace(tilde, quote));
                    sb.AppendLine("</layout>".Replace(tilde, quote));
                    sb.AppendLine("<layout id=~1~ name=~people~>".Replace(tilde, quote));
                    sb.AppendLine("<columns>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Name~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~FirstName~ columnWidth=~20~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~LastName~ columnWidth=~20~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Zip~ columnWidth=~20~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Address1~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Address2~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Address3~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~CaseNumber~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Date Filed~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Court~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Type~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Style~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Plantiff~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("</columns>".Replace(tilde, quote));
                    sb.AppendLine("</layout>".Replace(tilde, quote));
                    sb.AppendLine("<layout id=~10~ name=~caselayout~>".Replace(tilde, quote));
                    sb.AppendLine("<columns>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Number~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Name~ columnWidth=~80~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Filed/Location/Judical Officer~ columnWidth=~40~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Type/Status~ columnWidth=~40~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Style~ columnWidth=~80~/>".Replace(tilde, quote));
                    sb.AppendLine("</columns>".Replace(tilde, quote));
                    sb.AppendLine("</layout>".Replace(tilde, quote));
                    sb.AppendLine("<layout id=~10~ name=~people~>".Replace(tilde, quote));
                    sb.AppendLine("<columns>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Name~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~FirstName~ columnWidth=~20~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~LastName~ columnWidth=~20~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Zip~ columnWidth=~20~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Address1~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Address2~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Address3~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~CaseNumber~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Style~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Date Filed~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Court~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Type~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Style~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Plantiff~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("</columns>".Replace(tilde, quote));
                    sb.AppendLine("</layout>".Replace(tilde, quote));
                    sb.AppendLine("<layout id=~20~ name=~caselayout~>".Replace(tilde, quote));
                    sb.AppendLine("<columns>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Number~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Name~ columnWidth=~80~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Filed/Location/Judical Officer~ columnWidth=~40~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Type/Status~ columnWidth=~40~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Style~ columnWidth=~80~/>".Replace(tilde, quote));
                    sb.AppendLine("</columns>".Replace(tilde, quote));
                    sb.AppendLine("</layout>".Replace(tilde, quote));
                    sb.AppendLine("<layout id=~20~ name=~people~>".Replace(tilde, quote));
                    sb.AppendLine("<columns>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Name~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~FirstName~ columnWidth=~20~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~LastName~ columnWidth=~20~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Zip~ columnWidth=~20~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Address1~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Address2~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Address3~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~CaseNumber~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Date Filed~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Court~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Type~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Case Style~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("<column name=~Plantiff~ columnWidth=~30~/>".Replace(tilde, quote));
                    sb.AppendLine("</columns>".Replace(tilde, quote));
                    sb.AppendLine("</layout>".Replace(tilde, quote));
                    sb.AppendLine("</layouts>".Replace(tilde, quote));
                    sb.AppendLine("<inactive>".Replace(tilde, quote));
                    sb.AppendLine("<!-- ".Replace(tilde, quote));
                    sb.AppendLine("<keys id=~30~ name=~Dallas County~>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~baseUri~><![CDATA[https://courtsportal.dallascounty.org/DALLASPROD/Home/Dashboard/26]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=%20All%20District%20Courts%20]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~hlinkUri-inactive~><![CDATA[http://justice1.dentoncounty.com/PublicAccessDC/]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~dateRangeMaxDays~><![CDATA[31]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~startDate~><![CDATA[]]></key>".Replace(tilde, quote));
                    sb.AppendLine("<key name=~endDate~><![CDATA[]]></key>".Replace(tilde, quote));
                    sb.AppendLine("</keys>".Replace(tilde, quote));
                    sb.AppendLine("-->".Replace(tilde, quote));
                    sb.AppendLine("</inactive>".Replace(tilde, quote));
                    sb.AppendLine("</settings>".Replace(tilde, quote));

                    return sb.ToString();
                case "caselayout.xml":
                    var sbb = new StringBuilder();
                    sbb.AppendLine("<?xml version=~1.0~ encoding=~utf-8~?>".Replace(tilde, quote));
                    sbb.AppendLine("<search>".Replace(tilde, quote));
                    sbb.AppendLine("<parameters>".Replace(tilde, quote));
                    sbb.AppendLine("<parameter name=~Website~/>".Replace(tilde, quote));
                    sbb.AppendLine("<parameter name=~StartDate~/>".Replace(tilde, quote));
                    sbb.AppendLine("<parameter name=~EndDate~/>".Replace(tilde, quote));
                    sbb.AppendLine("<parameter name=~SearchDate~/>".Replace(tilde, quote));
                    sbb.AppendLine("<parameter name=~SearchComplete~>false</parameter>".Replace(tilde, quote));
                    sbb.AppendLine("</parameters>".Replace(tilde, quote));
                    sbb.AppendLine("<!-- Name".Replace(tilde, quote));
                    sbb.AppendLine("<results>".Replace(tilde, quote));
                    sbb.AppendLine("<result name=~casedata~>".Replace(tilde, quote));
                    sbb.AppendLine("<![CDATA[".Replace(tilde, quote));
                    sbb.AppendLine("]]>".Replace(tilde, quote));
                    sbb.AppendLine("</result>".Replace(tilde, quote));
                    sbb.AppendLine("<result name=~peopledata~>".Replace(tilde, quote));
                    sbb.AppendLine("<![CDATA[".Replace(tilde, quote));
                    sbb.AppendLine("]]>".Replace(tilde, quote));
                    sbb.AppendLine("</result>".Replace(tilde, quote));
                    sbb.AppendLine("<result name=~person~>".Replace(tilde, quote));
                    sbb.AppendLine("<people>".Replace(tilde, quote));
                    sbb.AppendLine("<person>".Replace(tilde, quote));
                    sbb.AppendLine("<name/>".Replace(tilde, quote));
                    sbb.AppendLine("<address>".Replace(tilde, quote));
                    sbb.AppendLine("<![CDATA[]]>".Replace(tilde, quote));
                    sbb.AppendLine("".Replace(tilde, quote));
                    sbb.AppendLine("<addressA/>".Replace(tilde, quote));
                    sbb.AppendLine("<addressB/>".Replace(tilde, quote));
                    sbb.AppendLine("<addressC/>".Replace(tilde, quote));
                    sbb.AppendLine("<zip/>".Replace(tilde, quote));
                    sbb.AppendLine("            ".Replace(tilde, quote));
                    sbb.AppendLine("</address>".Replace(tilde, quote));
                    sbb.AppendLine("<case>".Replace(tilde, quote));
                    sbb.AppendLine("<![CDATA[]]>".Replace(tilde, quote));
                    sbb.AppendLine("</case>".Replace(tilde, quote));
                    sbb.AppendLine("<dateFiled>".Replace(tilde, quote));
                    sbb.AppendLine("<![CDATA[]]>".Replace(tilde, quote));
                    sbb.AppendLine("</dateFiled>".Replace(tilde, quote));
                    sbb.AppendLine("<court>".Replace(tilde, quote));
                    sbb.AppendLine("<![CDATA[]]>".Replace(tilde, quote));
                    sbb.AppendLine("</court>".Replace(tilde, quote));
                    sbb.AppendLine("<caseType>".Replace(tilde, quote));
                    sbb.AppendLine("<![CDATA[]]>".Replace(tilde, quote));
                    sbb.AppendLine("</caseType>".Replace(tilde, quote));
                    sbb.AppendLine("<caseStyle>".Replace(tilde, quote));
                    sbb.AppendLine("<![CDATA[]]>".Replace(tilde, quote));
                    sbb.AppendLine("</caseStyle>".Replace(tilde, quote));
                    sbb.AppendLine("</person>".Replace(tilde, quote));
                    sbb.AppendLine("</people>".Replace(tilde, quote));
                    sbb.AppendLine("</result>".Replace(tilde, quote));
                    sbb.AppendLine("</results>".Replace(tilde, quote));
                    sbb.AppendLine("</search>".Replace(tilde, quote));

                    return sbb.ToString();
                default:
                    return string.Empty;
            }
        }



        /// <summary>
        /// Generates a unique file name for reasrch results file.
        /// </summary>
        /// <param name="settingFile">The setting file.</param>
        /// <returns></returns>
        private static string GetFileName(WebInteractive settingFile)
        {
            // settingFile.StartDate.ToString("MMddyyyy")
            const string dfmt = "MMddyyyy";
            var dteStart = settingFile.StartDate.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
            var dteEnding = settingFile.EndingDate.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
            var fileName = string.Format(
                CultureInfo.CurrentCulture,
                "data_rqst_{2}_{0}_{1}.xml",
                dteStart,
                dteEnding,
                settingFile.Parameters.Name.Replace(" ", "").ToLower(CultureInfo.CurrentCulture));
            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            var targetPath = new Uri(string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\xml\", execName)).AbsolutePath;
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            targetPath = string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\data\", targetPath);
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            var targetFile = new Uri(string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\xml\data\{1}", execName, fileName)).AbsolutePath;
            return targetFile;
        }



        /// <summary>
        /// Gets the name of the excel file.
        /// </summary>
        /// <returns></returns>
        private static string GetExcelFileName()
        {
            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            var targetFile = new Uri(string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\Utilities\CourtRecordSearch.xlsm", execName)).AbsolutePath;
            if (!File.Exists(targetFile))
            {
                return string.Empty;
            }

            return targetFile;
        }


        /// <summary>
        /// Gets the name of the application directory.
        /// </summary>
        /// <returns></returns>
        public static string GetAppFolderName()
        {
            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            return Path.GetDirectoryName(execName);
        }

        public static List<WebNavInstruction> GetInstructions(int siteId)
        {
            var instructions = new List<WebNavInstruction>();
            var content = Content;
            var doc = XmlDocProvider.GetDoc(content);

            var qpath = string.Format(
                CultureInfo.CurrentCulture,
                "directions/instructions[@id={0}]", siteId);
            var data = doc.DocumentElement.SelectSingleNode(qpath);
            if (data == null)
            {
                return instructions;
            }

            foreach (var item in data.ChildNodes)
            {
                var nde = (XmlNode)item;
                instructions.Add(new WebNavInstruction
                {
                    Name = nde.Attributes.GetNamedItem("name").Value,
                    By = nde.Attributes.GetNamedItem("By").Value,
                    CommandType = nde.Attributes.GetNamedItem("type").Value,
                    FriendlyName = nde.Attributes.GetNamedItem("FriendlyName").Value,
                    Value = ((XmlCDataSection)nde.FirstChild).Data
                });
            }
            return instructions;
        }
    }
}
