namespace Thompson.RecordSearch.Utility.Interfaces
{
    public interface IAuthenicationService
    {
        int RetryCount { get; }

        bool Login(string username, string password);
    }
}
