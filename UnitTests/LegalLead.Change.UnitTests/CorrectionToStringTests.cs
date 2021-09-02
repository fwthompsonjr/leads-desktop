using LegalLead.Changed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLipsum.Core;
using System;

namespace LegalLead.Changed.UnitTests
{
    [TestClass]
    public class CorrectionToStringTests
    {
        [TestMethod]
        public void CorrectionWithNoIdTest()
        {
            var test = new Correction();
            Assert.IsNotNull(test);
            Assert.AreEqual(0, test.Id);
            var actual = test.ToString();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void CorrectionWithNoIssueTest()
        {
            string description = GetRandomIpsum();
            const double testId = 2.5;
            var test = new Correction
            {
                Id = testId,
                CorrectionDate = DateTime.Now,
                Description = description
            };
            Assert.AreEqual(testId, test.Id);
            var actual = test.ToString();
            Assert.IsNotNull(actual);
            Console.Write(actual);
        }

        [TestMethod]
        public void CorrectionWithChangeTest()
        {
            string description = GetRandomIpsum();
            const double testId = 2.5;
            var test = new Correction
            {
                Id = testId,
                CorrectionDate = DateTime.Now,
                Description = description
            };
            var change = new Change
            {
                ReportedDate = DateTime.Now.AddDays(-7)
            };
            Assert.AreEqual(testId, test.Id);
            var expected = test.ToString();
            var actual = test.ToLogEntry(change);
            Assert.IsNotNull(actual);
            Assert.AreNotEqual(expected, actual);
            Assert.AreEqual(expected.Length, actual.Length);
            Console.Write(actual);
        }

        private static string GetRandomIpsum(int mxLength = 200)
        {
            string rawText = Lipsums.LoremIpsum;
            LipsumGenerator lipsum = new LipsumGenerator(rawText, false);
            int desiredParagraphCount = 5;
            string[] generatedParagraphs = lipsum.
                GenerateParagraphs(desiredParagraphCount, Paragraph.Medium);
            var description = string.Join(" ", generatedParagraphs)
                .Substring(0, mxLength); // only need 200 characters
            return description;
        }
    }
}
