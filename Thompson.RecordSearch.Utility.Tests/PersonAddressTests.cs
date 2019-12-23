using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using AutoMapper;
using CsvHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;
using Thompson.RecordSearch.Utility.Tests.Data;

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
            var settings = SettingsManager.GetNavigation();
            var sttg = settings.First();
            var startDate = DateTime.Now.Date.AddDays(-2);
            var endingDate = DateTime.Now.Date.AddDays(0);
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            webactive.ReadFromFile(testFile);
        }



        [TestMethod]
        public void CanMapPersonNonCriminalCaseInfo()
        {
            var caseInstructions = SearchSettingDto.GetNonCriminalMapping();
            var expectedList = ExpectedNonCriminalValues();

            var doc = new XmlDocument();
            doc.LoadXml(NonCriminalRow());
            var indx = 0;
            foreach (var item in caseInstructions.NavInstructions)
            {
                var node = TryFindNode(doc, item.Value);
                Assert.IsNotNull(node, $"{item.FriendlyName} is null");
                Assert.IsFalse(string.IsNullOrEmpty(node.InnerText), $"{item.FriendlyName} is blank or empty");
                // if (item.Name.Equals("CaseStyle")) continue;
                Assert.AreEqual(expectedList[indx++].CommandType, node.InnerText,
                    $"{item.FriendlyName} not matched. ");
            }
        }

        [TestMethod]
        public void CanMapPersonCriminalCaseInfo()
        {
            var caseInstructions = SearchSettingDto.GetCriminalMapping();
            var expectedList = ExpectedCriminalValues();

            var doc = new XmlDocument();
            doc.LoadXml(CriminalRow());
            var indx = 0;
            foreach (var item in caseInstructions.NavInstructions)
            {
                var node = TryFindNode(doc, item.Value);
                Assert.IsNotNull(node, $"{item.FriendlyName} is null");
                Assert.IsFalse(string.IsNullOrEmpty(node.InnerText), $"{item.FriendlyName} is blank or empty");
                if (item.Name.Equals("CaseStyle")) continue;
                Assert.AreEqual(expectedList[indx++].CommandType, node.InnerText,
                    $"{item.FriendlyName} not matched. ");
            }
        }

        [TestMethod]
        public void CanGetCourtLookUpList()
        {
            var list = SearchSettingDto.GetCourtLookupList;
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void CanGetSampleDtoList()
        {
            var list = SamplePersonAddress();
            Assert.IsNotNull(list);
            Assert.AreEqual(6, list.Count);
            var noPlantiffList = list.FindAll(x => string.IsNullOrEmpty(x.Plantiff));
            Assert.AreEqual(0, noPlantiffList.Count);
        }

        [TestMethod]
        public void CanFindASampleRecordWithoutPlantiff()
        {
            var list = SamplePersonAddress();
            Assert.IsNotNull(list);
            var noPlantiffList = list.FindAll(x => string.IsNullOrEmpty(x.Plantiff));
            if (!noPlantiffList.Any()) return;
            var failing = noPlantiffList.First();
            var actual = failing.Plantiff;

        }


        private XmlNode TryFindNode(XmlDocument doc, string xpath)
        {
            try
            {
                var node = doc.FirstChild.SelectSingleNode(xpath);
                return node;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string CriminalRow()
        {

            return " <tr> " + Environment.NewLine +
            " <td index='1' nowrap='true' valign='top'> " + Environment.NewLine +
            " <a href='CaseDetail.aspx?CaseID=2702640' style='color: blue'>19-1647J4</a> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='2' nowrap='true' valign='top'> " + Environment.NewLine +
            " <div style='overflow:hidden'>304097</div> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='3' nowrap='true' valign='top'> " + Environment.NewLine +
            " <div>Morton, Mason R</div> " + Environment.NewLine +
            " <div>08/29/2001</div> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='4' valign='top' nowrap='true'> " + Environment.NewLine +
            " <div>12/03/2019</div> " + Environment.NewLine +
            " <div>Justice of the Peace Pct #4</div> " + Environment.NewLine +
            " <div>Hughey, Harris</div> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='5' valign='top' nowrap='true'> " + Environment.NewLine +
            " <div>Adult Traffic Citation</div> " + Environment.NewLine +
            " <div>Filed</div> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='6' nowrap='true' valign='top'> " + Environment.NewLine +
            " <table border='0' cellpadding='0' cellspacing='0'  " + Environment.NewLine +
            "  width='100%' style='table-layout: fixed; font-size: 8pt; font-family: arial'> " + Environment.NewLine +
            " <tbody> " + Environment.NewLine +
            " <tr> " + Environment.NewLine +
            " <td style='vertical-align:top;'>SPEEDING &gt;10% ABOVE POSTED LIMIT </td> " + Environment.NewLine +
            " </tr> " + Environment.NewLine +
            " </tbody> " + Environment.NewLine +
            " </table> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " </tr>";
        }

        private string NonCriminalRow()
        {
            return "<tr> " + Environment.NewLine +
            "<td index='1' nowrap='true' valign='top'>" + Environment.NewLine +
            "<a href='CaseDetail.aspx?CaseID=2701630' " + Environment.NewLine +
            "style='color: blue'>BF-2019-01279</a>" + Environment.NewLine +
            "</td>" + Environment.NewLine +
            "<td index='2' valign='top'>State of Texas VS. Jennifer Dansheal Lee</td>" + Environment.NewLine +
            "<td index='3' valign='top' nowrap='true'>" + Environment.NewLine +
            "<div>11/26/2019</div><div>County Court At Law #2</div>" + Environment.NewLine +
            "<div>Ramirez, Robert C.</div></td><td valign='top' nowrap='true'>" + Environment.NewLine +
            "<div>Bond Forfeiture-Surety</div><div>Active</div>" + Environment.NewLine +
            "</td>" + Environment.NewLine +
            "</tr>";
        }
        private IList<WebNavInstruction> ExpectedCriminalValues()
        {
            var caseInstructions = SearchSettingDto.GetCriminalMapping();
            foreach (var item in caseInstructions.NavInstructions)
            {
                switch (item.Name)
                {
                    case "DateFiled":
                        item.CommandType = "12/03/2019";
                        break;
                    case "Case":
                        item.CommandType = "19-1647J4";
                        break;
                    case "Court":
                        item.CommandType = "Justice of the Peace Pct #4";
                        break;
                    case "CaseType":
                        item.CommandType = "Adult Traffic Citation";
                        break;
                    case "CaseStyle":
                        item.CommandType = "SPEEDING &gt;10% ABOVE POSTED LIMIT";
                        break;
                    default:
                        break;
                }
            }
            return caseInstructions.NavInstructions;
        }


        private IList<WebNavInstruction> ExpectedNonCriminalValues()
        {
            var caseInstructions = SearchSettingDto.GetCriminalMapping();
            foreach (var item in caseInstructions.NavInstructions)
            {
                switch (item.Name)
                {
                    case "DateFiled":
                        item.CommandType = "11/26/2019";
                        break;
                    case "Case":
                        item.CommandType = "BF-2019-01279";
                        break;
                    case "Court":
                        item.CommandType = "County Court At Law #2";
                        break;
                    case "CaseType":
                        item.CommandType = "Bond Forfeiture-Surety";
                        break;
                    case "CaseStyle":
                        item.CommandType = "State of Texas VS. Jennifer Dansheal Lee";
                        break;
                    default:
                        break;
                }
            }
            return caseInstructions.NavInstructions;
        }



        private List<PersonAddress> SamplePersonAddress()
        {
            const string jsFile = @"Json\collincounty_probate.csv";
            var appFile = GetAppDirectoryName();
            appFile = Path.Combine(appFile, jsFile); 
            using (var reader = new StreamReader(appFile))
            using (var csv = new CsvReader(reader))
            {
                var dto = csv.GetRecords<PersonAddressDto>().ToList();
                var config = new MapperConfiguration(cfg => cfg.CreateMap<PersonAddressDto, PersonAddress>());
                var mapper = config.CreateMapper();
                var people = new List<PersonAddress>();
                dto.ForEach(x => people.Add(mapper.Map<PersonAddress>(x)));
                return people;
            }
            // return null;
        }


        private static string GetAppDirectoryName()
        {

            var navigation = new SettingsManager();
            var navFile = navigation.ExcelFormatFile;
            var folder = Path.GetDirectoryName(navFile);
            while (new DirectoryInfo(folder).Name != "bin")
            {
                folder = new DirectoryInfo(folder).Parent.FullName;
            }
            return new DirectoryInfo(folder).Parent.FullName;
        }
    }
}
