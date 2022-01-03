using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataActionAttribute : Attribute
    {
        public int ProcessId { get; set; }
        public string Name { get; set; }
    }
}
