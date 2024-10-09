namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface ICountyCodeReader
    {
        string GetCountyCode(int id);
        string GetCountyCode(string code);
    }
}
