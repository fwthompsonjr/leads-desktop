namespace LegalLead.PublicData.Search.Helpers
{
    internal class SessionApiFilePersistence : SessionFilePersistence
    {

        protected override string SetupFile
        {
            get
            {
                if (setupFileName != null) return setupFileName;
                setupFileName = SessionUtil.GetFullFileName(datFileName);
                return setupFileName;
            }
        }
        private static string setupFileName = null;
        private const string datFileName = "session.dtx";
    }
}
