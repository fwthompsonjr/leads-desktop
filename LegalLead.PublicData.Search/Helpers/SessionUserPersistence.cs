using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionUserPersistence : SessionFilePersistence
    {
        public string GetUserName()
        {
            var content = Read();
            var dto = content.ToInstance<LoginAccountDto>();
            if (dto == null || string.IsNullOrEmpty(dto.UserName)) return string.Empty;
            return dto.UserName;
        }
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
        private const string datFileName = "session.dtu";
    }
}
