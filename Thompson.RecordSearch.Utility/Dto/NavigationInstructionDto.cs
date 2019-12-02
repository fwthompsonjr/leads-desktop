using System.Collections.Generic;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class Locator
    {
        public string Find { get; set; }
        public string Query { get; set; }
    }

    public class Step
    {
        public string ActionName { get; set; }
        public string DisplayName { get; set; }
        public Locator Locator { get; set; }
        public string ExpectedValue { get; set; }
        public int Wait { get; set; }
    }

    public class NavigationInstructionDto
    {
        public List<Step> Steps { get; set; }
    }
}
