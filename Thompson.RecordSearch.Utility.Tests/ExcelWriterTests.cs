using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Tests
{
    [TestClass]
    public class ExcelWriterTests
    {
        private string SampleTable()
        {
            return "<table style='border-collapse: collapse; border: 1px solid black;'>" +
                "<tr><th class='ssSearchResultHeader' nowrap='true'><b>Case Number</b></th>" +
                "<th class='ssSearchResultHeader' nowrap='true'><b>Style</b></th>" +
                "<th class='ssSearchResultHeader'><b>Filed/Location/Judicial Officer</b></th>" +
                "<th class='ssSearchResultHeader' nowrap='true'><b>Type</b><b>/Status</b></th>" +
                "</tr>" +
                "<tr>" +
                "<td nowrap='true' valign='top'><a href='CaseDetail.aspx?CaseID=2642893' " +
                "style='color: blue'>BF-2019-00309</a></td>" +
                "<td valign='top'>State of Texas VS. John Vincent Olaes Rualo, Principal</td>" +
                "<td valign='top' nowrap='true'><div>04/12/2019</div><div>County Court At Law #2</div>" +
                "<div>Ramirez, Robert C.</div></td><td valign='top' nowrap='true'>" +
                "<div> Bond Forfeiture - Principal </div><div> Active </div></td></tr> " +
                "<tr bgcolor='#EEEEEE'>" +
                "<td nowrap='true' valign='top'>" +
                "<a href='CaseDetail.aspx?CaseID=2642900' style='color: blue'>BF-2019-00310</a></td>" +
                "<td valign='top'>State of Texas VS. Darin Jay Hoogendoorn, " +
                "Principal and International Fidelity Insurance Co. " +
                "and Doing Business As Angela Burgher and Doing Business " +
                "As Central Bail Bonds II-Denton, Surety</td>" +
                "<td valign='top' nowrap='true'><div>04/12/2019</div><div>County Court At Law #2</div>" +
                "<div>Ramirez, Robert C.</div></td><td valign='top' nowrap='true'>" +
                "<div>Bond Forfeiture-Surety</div><div>Active</div></td>" +
                "</tr>" +
                "<tr>" +
                "<td nowrap='true' valign='top'><a href='CaseDetail.aspx?CaseID=2642969' " +
                "style='color: blue'>C19-353J4</a></td><td valign='top'>FEELEY RENTALS, LLC " +
                "VS " +
                "KAGHOB LEE, KWANGMI YE, AND ALL OTHER OCCUPANTS</td>" +
                "<td valign='top' nowrap='true'><div>04/12/2019</div>" +
                "<div>Justice of the Peace Pct #4</div><div>Hughey, Harris</div>" +
                "</td>" +
                "<td valign='top' nowrap='true'><div>Evictions</div><div>Filed</div></td>" +
                "</tr>" +
                "</table>";
        }
        private List<PersonAddress> SamplePeople()
        {
            int nbr = random.Next(5, 12);
            var data = new List<PersonAddress>();
            for (int i = 0; i < nbr; i++)
            {
                var person = new PersonAddress();
                foreach (var item in person.FieldList)
                {
                    person[item] = RandomString(15);
                }
                data.Add(person);
            }
            return data;
        }
        private static readonly Random random = new Random(DateTime.Now.Millisecond);
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [TestMethod]
        [TestCategory("Excel.Automation.Tests")]
        public void CanWriteTableToExcelLocal()
        {
            var writer = new ExcelWriter();
            var extn = CommonKeyIndexes.ExtensionXlsx;
            var tmpFileName = string.Format(@"{0}{1}{2}",
                Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(Path.GetRandomFileName()),
                extn);

            writer.ConvertToDataTable(
                htmlTable: SampleTable(),
                worksheetName: "CaseData",
                saveFile: true,
                outputFileName: tmpFileName);

            Assert.IsTrue(File.Exists(tmpFileName), "Expected Excel file was not found");
            Console.WriteLine("Excel file created at: {0}", tmpFileName);

        }


        [TestMethod]
        [TestCategory("Excel.Automation.Tests")]
        public void CanWritePeopleToExcelLocal()
        {
            var writer = new ExcelWriter();
            var extn = CommonKeyIndexes.ExtensionXlsx;
            var tmpFileName = string.Format(@"{0}{1}{2}",
                Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(Path.GetRandomFileName()),
                extn);

            writer.ConvertToPersonTable(
                addressList: SamplePeople(),
                worksheetName: "Addresses",
                saveFile: true,
                outputFileName: tmpFileName);

            Assert.IsTrue(File.Exists(tmpFileName), "Expected Excel file was not found");
            Console.WriteLine("Excel file created at: {0}", tmpFileName);

        }

        [TestMethod]
        [TestCategory("Excel.Automation.Tests")]
        public void CanWritePeopleAndTableLocal()
        {

            var writer = new ExcelWriter();
            var extn = CommonKeyIndexes.ExtensionXlsx;
            var tmpFileName = string.Format(@"{0}{1}{2}",
                Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(Path.GetRandomFileName()),
                extn);

            var workBook = writer.ConvertToPersonTable(
                addressList: SamplePeople(),
                worksheetName: "Addresses",
                saveFile: false,
                outputFileName: tmpFileName);

            writer.ConvertToDataTable(
                excelPackage: workBook,
                htmlTable: SampleTable(),
                worksheetName: "CaseData",
                saveFile: true,
                outputFileName: tmpFileName);

            Assert.IsTrue(File.Exists(tmpFileName), "Expected Excel file was not found");
            Console.WriteLine("Excel file created at: {0}", tmpFileName);
        }

        [TestMethod]
        [TestCategory("Excel.Automation.Tests")]
        public void CanCreateTestFile()
        {
            const string fileName = "tarrantSample.json";
            var dir = SettingsManager.GetAppFolderName();
            dir = new DirectoryInfo(dir).Parent.FullName;
            dir = new DirectoryInfo(dir).Parent.FullName;
            dir = new DirectoryInfo(dir).Parent.FullName;
            dir = Path.Combine(dir, fileName);
            if (File.Exists(dir)) return;

            var people = SamplePeople().Take(2);
            using (var writer = new StreamWriter(dir))
            {
                writer.WriteLine(Newtonsoft.Json.JsonConvert
                    .SerializeObject(people, Newtonsoft.Json.Formatting.Indented));
            }

        }
    }
}
