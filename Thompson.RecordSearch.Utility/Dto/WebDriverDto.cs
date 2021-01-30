using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class WebDriverDto
    {
        public int SelectedIndex { get; set; }
        public IList<Driver> Drivers { get; set; }
    }


}
