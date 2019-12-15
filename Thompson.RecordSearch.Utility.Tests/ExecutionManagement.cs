using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Tests
{
    public static class ExecutionManagement
    {


        public static bool CanExecuteFetch()
        {
            var settingCanExecute = 
                ConfigurationManager.AppSettings["allow.web.integration"];
            if (settingCanExecute == null) return true;
            if (!bool.TryParse(settingCanExecute, out bool canExec)) return true;
            return canExec;

        }
    }
}
