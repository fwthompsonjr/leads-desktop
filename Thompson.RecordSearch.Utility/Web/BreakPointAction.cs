using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class BreakPointAction : ElementActionBase
    {
        const string actionName = "break-point";

        public override string ActionName => actionName;

        public override void Act(Step item)
        {
            System.Diagnostics.Debugger.Break();
        }
    }
}
