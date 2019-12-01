using System;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class PersonAddressTests
    {
        [TestMethod]
        [TestCategory("Person.Data.Mapping")]
        public void CanSerializeXmlString()
        {

            var personData = "<person>" +
            "<name>Holland, Stephen</name>" +
            "<address><![CDATA[  387 West FRK<br/>  APT #2838<br/>  Irving, TX 75039]]>" +
            "<addressA><![CDATA[387 WEST FRK]]></addressA>" +
            "<addressB><![CDATA[APT #2838]]></addressB>" +
            "<addressC><![CDATA[IRVING, TX 75039]]></addressC>" +
            "<zip><![CDATA[75039]]></zip>" +
            "</address>" +
            "<case><![CDATA[CV-2019-01210-OL]]></case>" +
            "<dateFiled><![CDATA[04/12/2019]]></dateFiled>" +
            "<court><![CDATA[County Court At Law #2]]></court>" +
            "<caseType><![CDATA[Occupational  License]]></caseType>" +
            "</person>";

            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);
            XmlNode rootNode = doc.CreateElement("personList");
            doc.AppendChild(rootNode);

            //Create a document fragment and load the xml into it
            XmlDocumentFragment fragment = doc.CreateDocumentFragment();
            fragment.InnerXml = personData;
            rootNode.AppendChild(fragment);
            // what do i do with these line-breaks?
            // how is that going to translate into Excel?
            var dto = PersonAddress.ConvertFrom(rootNode.FirstChild);

            Assert.IsNotNull(dto);
        }

        [TestMethod]
        [TestCategory("Person.Data.Mapping")]
        public void CanReadFileAndGetCases()
        {
            const string testFile = @"C:/Code/SandBox/RecordSearch/Thompson.RecordSearch.Utility.Tests/bin/Debug/xml/data/data_rqst_dentoncounty_04152019_04172019.xml";
            var settings = new SettingsManager().GetNavigation();
            var sttg = settings.First();
            var startDate = DateTime.Now.Date.AddDays(-2);
            var endingDate = DateTime.Now.Date.AddDays(0);
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            webactive.ReadFromFile(testFile);
        }
    }
}
