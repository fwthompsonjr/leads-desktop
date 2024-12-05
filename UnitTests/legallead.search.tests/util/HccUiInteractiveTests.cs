using LegalLead.PublicData.Search.Util;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.util
{
    public class HccUiInteractiveTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(HccUiInteractive));
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        [InlineData(true, false)]
        public void ServiceCanBeConstructed(bool allowDownload, bool isTest)
        {
            var parameter = GetParameter();
            var error = Record.Exception(() => _ = new HccUiInteractive(parameter, allowDownload, isTest));
            Assert.Null(error);
        }
        private static WebNavigationParameter GetParameter()
        {
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = "2024-05-25"},
                new() { Name = "EndDate", Value = "2024-05-25"},
                new() { Name = "CourtType", Value = "JUSTICE"}
            };
            return new WebNavigationParameter { Keys = keys };
        }
    }
}