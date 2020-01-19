using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LegalLead.Changed.Classes
{
    public class CommandSetInstallVersion : BuildCommandBase
    {
        public override int Index => 100000;

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(SourceFile))
                throw new InvalidOperationException();

            if (Log == null)
                throw new InvalidOperationException();

            if (LatestVersion == null)
            {
                return true;
            }

            var sourceDir = Path.GetDirectoryName(SourceFile);
            var installDir = Path.Combine(sourceDir, "Legal Lead Install");
            var installFile = Path.Combine(installDir, "Legal Lead Install.aip");
            
            return SetInstallVersion(installFile);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", 
            "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private bool SetInstallVersion(string installFile)
        {
            try
            {
                var doc = GetDoc(installFile);
                // Property="ProductVersion"
                var node = doc.DocumentElement.ChildNodes[1].ChildNodes.Cast<XmlNode>()
                    .ToList().Find(x =>
                    x.Attributes.GetNamedItem("Property").InnerText.Equals("ProductVersion"));
                if (node.Attributes.GetNamedItem("Value").InnerText.Equals(LatestVersion.Number))
                {
                    return true;
                }
                node.Attributes.GetNamedItem("Value").InnerText = LatestVersion.Number;
                doc.Save(installFile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", 
            "CA2000:Dispose objects before losing scope", Justification = "Object is being passed to caller and must not be disposed.")]
        private static XmlDocument GetDoc(string fileName)
        {
            string content = string.Empty;
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            using (var streamReader = new StreamReader(fileName))
            {
                content = streamReader.ReadToEnd();
            }
            StringReader sreader = new StringReader(content);
            XmlReader reader = XmlReader.Create(sreader, new XmlReaderSettings() { XmlResolver = null });
            doc.Load(reader);
            return doc;
        }
    }
}
