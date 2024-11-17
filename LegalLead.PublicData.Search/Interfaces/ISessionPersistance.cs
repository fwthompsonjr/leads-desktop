namespace LegalLead.PublicData.Search.Interfaces
{
    public interface ISessionPersistance
    {
        void Initialize();
        string GetAccountCredential(string county = "");
        string Read();
        bool Write(string content);
        string GetAccountPermissions();
    }
}
