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
            For<IUserManager>().Add<UserManagerGetPricing>().Named("GetPricing");
            For<IUserManager>().Add<UserManagerGetCounty>().Named("GetCounty");
            For<IUserManager>().Add<UserManagerGetProfile>().Named("GetProfile");
            For<IUserManager>().Add<UserManagerGetInvoices>().Named("GetInvoice");
            For<IUserManager>().Add<UserManagerGetSearch>().Named("GetSearch");
            For<IUserManager>().Add<UserManagerGetBillTypeHistory>().Named("GetBillCode");
            For<IUserManager>().Add<UserManagerNonActive>().Named("UpdateProfile");
            For<IUserManager>().Add<UserManagerNonActive>().Named("UpdateUsageLimit");
            For<IUserManager>().Add<UserManagerNonActive>().Named("None");
        }
    }
}