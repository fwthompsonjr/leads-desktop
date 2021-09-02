using LegalLead.Changed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace LegalLead.Changed.UnitTests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void FixedWidthVariableLengthTest()
        {
            var test = string.Empty;
            var length = new Random(DateTime.Now.Millisecond)
                .Next(7, 700);
            var actual = test.ToFixedWidth(length);
            Assert.AreEqual(length, actual.Length);
        }

        [TestMethod]
        public void FixedWidthExtendStringTest()
        {
            var test = "abcdefg";
            var length = new Random(DateTime.Now.Millisecond)
                .Next(7, 700);

            var actual = test.ToFixedWidth(length);
            Assert.AreEqual(length, actual.Length);
            Assert.IsTrue(actual.StartsWith(test, StringComparison.CurrentCulture));

        }

        [TestMethod]
        public void FixedWidthZeroLengthTest()
        {
            var test = "abcdefg";
            var length = new Random(DateTime.Now.Millisecond)
                .Next(0, 700);

            var actual = test.ToFixedWidth(-length);
            Assert.IsNotNull(actual);
            Assert.IsTrue(string.IsNullOrEmpty(actual));
        }

        [TestMethod]
        public void SplitByLengthTest()
        {
            var test = "abcdefg ";
            var length = new Random(DateTime.Now.Millisecond)
                .Next(1, test.Length);
            var sb = new StringBuilder(test);
            for (int i = 0; i < length; i++)
            {
                sb.Append(test);
            }
            var actual = sb.ToString().SplitByLength(length).ToList();
            Assert.IsNotNull(actual);
            for (int i = 0; i < actual.Count - 1; i++)
            {
                // each element in collection should be same length
                Assert.IsTrue(actual[i].Length == length);
            }
            Assert.IsTrue(actual.Last().Length <= length);
        }

        [TestMethod]
        public void SplitByLengthTruncateLastIfEmptyTest()
        {
            var test = "abcdefg ";
            const int length = 7;
            var sb = new StringBuilder(test);
            for (int i = 0; i < length; i++)
            {
                sb.Append(test);
            }
            var normal = sb.ToString().SplitByLength(length).ToList();
            var actual = sb.ToString().SplitByLength(length, true).ToList();
            Assert.IsNotNull(actual);
            var expectedCount =
                string.IsNullOrEmpty(normal.Last().Trim()) ?
                normal.Count - 1 : normal.Count;
            Assert.AreEqual(expectedCount, actual.Count);
        }
    }
}
