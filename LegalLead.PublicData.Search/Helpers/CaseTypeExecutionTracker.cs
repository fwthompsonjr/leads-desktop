using LegalLead.PublicData.Search.Common;

namespace LegalLead.PublicData.Search.Helpers
{
    public class CaseTypeExecutionTracker
    {
        public int Id { get; set; }
        public bool IsExecuted { get; set; }
        public DallasJusticeOfficer Officer { get; set; }
    }
}
