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

        public override async Task ExecuteAsync(IProgress<HccProcess> progress, HccProcess process)
        {
            Start();
            WebDriver = await GetDriverAsync(true).ConfigureAwait(false);
            End();
        }

        private async Task<IWebDriver> GetDriverAsync(bool headless)
        {
            var drvr = await Task.Run(() =>
            {
                var wdriver = (new WebDriverDto().Get()).WebDrivers;
                var driver = wdriver.Drivers.Where(d => d.Id == wdriver.SelectedIndex).FirstOrDefault();
                var container = WebDriverContainer.GetContainer;
                var provider = container.GetInstance<IWebDriverProvider>(driver.Name);
                return provider.GetWebDriver(headless);
            }).ConfigureAwait(false);
            return drvr;
        }
    }
}
