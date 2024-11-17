namespace LegalLead.PublicData.Search.Interfaces
{
    internal interface ISessionPersistance
    {
        void Initialize();
        string GetAccountCredential(string county = "");
        string Read();
        bool Write(string content);
    }
}
