using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface ICountyCodeService
    {
        CountyCodeMapDto Map { get; }
        CountyCodeDto Find(int id);
        CountyCodeDto Find(string name);
        string GetWebAddress(int id);
    }
}
