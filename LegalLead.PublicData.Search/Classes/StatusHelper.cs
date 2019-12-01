using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.PublicData.Search
{

    public enum StatusTypes
    {
        Ready = 1,
        Running = 2,
        Finished = 3,
        Error = 4
    }
    public class StatusState
    {
        public string Name { get; set; }
        public System.Drawing.Color Color { get; set; }
    }
    public class StatusHelper
    {
        public static StatusState GetStatus(StatusTypes status)
        {

            var v = (int)status - 1;
            var collectionName = Enum.GetName(typeof(StatusTypes), status);
            var statuses = "Ready,Running,Finished,Error".Split(',').ToList();
            var colors = new List<System.Drawing.Color>()
            {
                { System.Drawing.Color.Black },
                { System.Drawing.Color.Green },
                { System.Drawing.Color.Blue },
                { System.Drawing.Color.Red }
            };
            return new StatusState
            {
                Name = collectionName,
                Color = colors[v]
            };
        }
    }
}
