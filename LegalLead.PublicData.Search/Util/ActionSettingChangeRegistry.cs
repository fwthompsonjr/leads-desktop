using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using StructureMap;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionSettingChangeRegistry : Registry
    {
        public ActionSettingChangeRegistry()
        {
            For<ISettingChangeModel>().Add<ChangeBooleanModel>().Named("Boolean");
            For<ISettingChangeModel>().Add<ChangeDateModel>().Named("DateTime");
            For<ISettingChangeModel>().Add<ChangeNumericModel>().Named("Numeric");
            For<ISettingChangeModel>().Add<ChangeTextModel>().Named("Text");
        }
    }
}