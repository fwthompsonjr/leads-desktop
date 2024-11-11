namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface ICountyCodeReader
    {
        string GetCountyCode(int id, string userId = "");
        string GetCountyCode(string code, string userId = "");
    }
}
