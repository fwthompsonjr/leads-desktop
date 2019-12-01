using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
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
        public string Content
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
        public List<WebNavigationParameter> GetNavigation()
        {
            var data = Content;
            var doc = new XmlDocument();
            doc.LoadXml(data);
            if (doc.DocumentElement == null) return null;
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
        public List<ExcelColumnLayout> GetColumnLayouts(int id, string sectionName)
        {
            var data = Content;
            var doc = new XmlDocument();
            doc.LoadXml(data);
            if (doc.DocumentElement == null) return null;
            var parent = doc.DocumentElement.SelectSingleNode(
                string.Format("layouts/layout[@id='{0}' and @name='{1}']", 
                id,
                sectionName));
            if (parent == null) return null;
            var columnNode = parent.FirstChild;
            if (columnNode == null) return null;
            if (!columnNode.HasChildNodes) return null;
            var layoutList = new List<ExcelColumnLayout>();
            var columnList = columnNode.ChildNodes.Cast<XmlNode>().ToList();
            foreach (var column in columnList)
            {
                layoutList.Add(new ExcelColumnLayout
                {
                    Name = column.Attributes.GetNamedItem("name").InnerText,
                    ColumnWidth = Convert.ToInt32(column.Attributes.GetNamedItem("columnWidth").InnerText)
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
            const string dfmt = "MMddyyyy";
            var fileName = GetFileName(settingFile);
            var targetFile = fileName;
            var idx = 0;
            while (File.Exists(targetFile))
            {
                idx = idx + 1;
                var cleaned = Path.GetFileNameWithoutExtension(fileName);
                cleaned = string.Format("{0}_{1}.xml", cleaned, idx.ToString("D9"));
                targetFile = string.Format("{0}/{1}", Path.GetDirectoryName(fileName), cleaned);
            }
            fileName = targetFile;
            using (var sw = new StreamWriter(fileName))
            {
                sw.Write(Layout);
                sw.Close();
            }
            var doc = new XmlDocument();
            doc.Load(fileName);
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
                        item.InnerText = settingFile.StartDate.ToString(dfmt);
                        break;
                    case "EndDate":
                        item.InnerText = settingFile.EndingDate.ToString(dfmt);
                        break;
                    case "SearchDate":
                        item.InnerText = DateTime.Now.ToString(dfmt);
                        break;
                    default:
                        break;
                }
            }
            
            
            
            
            doc.Save(fileName);
            
            // check for null
            return new XmlContentHolder {
                Id = settingFile.Parameters.Id,
                FileName = fileName,
                Document = doc};
        }

        private static WebNavigationParameter MapFrom(XmlNode node)
        {
            var parameter = new WebNavigationParameter
            {
                Id = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value),
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
            var qpath = string.Format("directions/instructions[@id={0}]", parameter.Id);
            
            var instructions = node.OwnerDocument
                .DocumentElement.SelectSingleNode(qpath);
            if (instructions == null) return parameter;

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

            qpath = string.Format("directions/caseInspection[@id={0}]", parameter.Id);
            instructions = node.OwnerDocument
                .DocumentElement.SelectSingleNode(qpath);
            if (instructions == null) return parameter;

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
            var targetFile = new Uri(string.Format(@"{0}\xml\{1}", execName, fileName)).AbsolutePath;
            return !File.Exists(targetFile) ? string.Empty : File.ReadAllText(targetFile);
        }



        /// <summary>
        /// Generates a unique file name for reasrch results file.
        /// </summary>
        /// <param name="settingFile">The setting file.</param>
        /// <returns></returns>
        private string GetFileName(WebInteractive settingFile)
        {
            // settingFile.StartDate.ToString("MMddyyyy")
            const string dfmt = "MMddyyyy";
            var dteStart = settingFile.StartDate.ToString(dfmt);
            var dteEnding = settingFile.EndingDate.ToString(dfmt);
            var fileName = string.Format("data_rqst_{2}_{0}_{1}.xml",
                dteStart,
                dteEnding,
                settingFile.Parameters.Name.Replace(" ","").ToLowerInvariant());
            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            var targetPath = new Uri(string.Format(@"{0}\xml\", execName)).AbsolutePath;
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            targetPath = string.Format(@"{0}\data\", targetPath);
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

            var targetFile = new Uri(string.Format(@"{0}\xml\data\{1}", execName, fileName)).AbsolutePath;
            return targetFile;
        }



        /// <summary>
        /// Gets the name of the excel file.
        /// </summary>
        /// <returns></returns>
        private string GetExcelFileName()
        {
            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            var targetFile = new Uri(string.Format(@"{0}\Utilities\CourtRecordSearch.xlsm", execName)).AbsolutePath;
            if (!File.Exists(targetFile)) return string.Empty;
            return targetFile;
        }

        public static List<WebNavInstruction> GetInstructions(int siteId)
        {
            var instructions = new List<WebNavInstruction>();
            var content = new SettingsManager().Content;
            var doc = new XmlDocument();
            doc.LoadXml(content);

            var qpath = string.Format("directions/instructions[@id={0}]", siteId);
            var data = doc.DocumentElement.SelectSingleNode(qpath);
            if (data == null) return instructions;

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
