using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using StructureMap;

namespace LegalLead.PublicData.Search.Util
{
    public class UserManagerRegistry : Registry
    {
        public UserManagerRegistry()
        {
            For<IUserManager>().Add<UserManagerGetAccounts>().Named("GetAccounts");
            For<IUserManager>().Add<UserManagerNonActive>().Named("GetPricing");
            For<IUserManager>().Add<UserManagerNonActive>().Named("GetCounty");
            For<IUserManager>().Add<UserManagerNonActive>().Named("GetProfile");
            For<IUserManager>().Add<UserManagerNonActive>().Named("GetInvoice");
            For<IUserManager>().Add<UserManagerNonActive>().Named("GetSearch");
            For<IUserManager>().Add<UserManagerNonActive>().Named("UpdateProfile");
            For<IUserManager>().Add<UserManagerNonActive>().Named("UpdateUsageLimit");
            For<IUserManager>().Add<UserManagerNonActive>().Named("None");
        }
    }
}