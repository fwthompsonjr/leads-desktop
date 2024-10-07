using Thompson.RecordSearch.Utility.Dto;

namespace legallead.search.tests.classes
{

    public class CaseSelectionTests
    {
        [Theory]
        [InlineData("dallasCountyCaseOptions")]
        public void DtoCanGetDropDown(string text)
        {
            var dropdown = CaseTypeSelectionDto.GetDto(text);
            Assert.NotNull(dropdown);
        }
    }
}
