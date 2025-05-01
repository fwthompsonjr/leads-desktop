using System;

namespace LegalLead.PublicData.Search.Classes
{
    public class GetAccountsResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsAdministrator { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
