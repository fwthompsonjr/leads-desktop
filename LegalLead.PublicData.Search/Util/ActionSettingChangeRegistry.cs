using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionSettingChangeRegistry : Registry
    {
        public ActionSettingChangeRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<IRemoteDbHelper>().Add<RemoteDbHelper>().Singleton();

            For<ISettingChangeModel>().Add<ChangeBooleanModel>().Named("Bool");
            For<ISettingChangeModel>().Add<ChangeDateModel>().Named("DateTime");
            For<ISettingChangeModel>().Add<ChangeNumericModel>().Named("Numeric");
            For<ISettingChangeModel>().Add<ChangeTextModel>().Named("Text");
        }
    }
}