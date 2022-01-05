using Harris.Criminal.Db.Entities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Db
{
    [DataAction(Name = "header", ProcessId = 1)]
    public class GetBrowserAction : BaseAction
    {
        public GetBrowserAction(HccProcess process) : base(process)
        {
        }
        public override TimeSpan EstimatedDuration => TimeSpan.FromSeconds(15);

        public override void Execute(IProgress<HccProcess> progress)
        {
            ReportProgress = progress;
            Start();
            WebDriver = GetDriver(false);
            End();
        }

        private IWebDriver GetDriver(bool headless)
        {
            var wdriver = (new WebDriverDto().Get()).WebDrivers;
            var driver = wdriver.Drivers.Where(d => d.Id == wdriver.SelectedIndex).FirstOrDefault();
            var container = WebDriverContainer.GetContainer;
            var provider = container.GetInstance<IWebDriverProvider>(driver.Name);
            return provider.GetWebDriver(headless);
        }
    }
}
