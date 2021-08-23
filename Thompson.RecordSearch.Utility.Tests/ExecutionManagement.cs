using System.Configuration;

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
