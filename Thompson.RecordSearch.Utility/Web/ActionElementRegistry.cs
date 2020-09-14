using StructureMap;
using Thompson.RecordSearch.Utility.Interfaces;

namespace Thompson.RecordSearch.Utility.Web
{
    public class ActionElementRegistry : Registry
    {
        public ActionElementRegistry()
        {
            For<IElementActionBase>().Use<DefaultAction>();

            For<IElementActionBase>().Add<BreakPointAction>().Named("break-point");
            For<IElementActionBase>().Add<ElementCountAction>().Named("count");
            For<IElementActionBase>().Add<ElementClickAction>().Named("click");
            For<IElementActionBase>().Add<ElementTextConfirmAction>().Named("text-confirm");
            For<IElementActionBase>().Add<ElementExistsAction>().Named("exists");
            For<IElementActionBase>().Add<ElementSetValueAction>().Named("set-text");
            For<IElementActionBase>().Add<WebNavAction>().Named("navigate");
            For<IElementActionBase>().Add<ElementSetComboBoxValue>().Named("set-select-value");
            For<IElementActionBase>().Add<ElementSendKeyAction>().Named("send-key");
            For<IElementActionBase>().Add<ElementGetHtmlAction>().Named("get-table-html");
            For<IElementActionBase>().Add<ElementSetPasswordAction>().Named("login");
            For<IElementActionBase>().Add<ElementSetDropDownValue>().Named("set-dropdown-value");
            For<IElementActionBase>().Add<GetRecordCountAction>().Named("get-record-count"); 
            For<IElementActionBase>().Add<JquerySetTextBox>().Named("jquery-set-text");
            For<IElementActionBase>().Add<JquerySetSelectedIndex>().Named("jquery-set-selected-index");
        }
    }
}
