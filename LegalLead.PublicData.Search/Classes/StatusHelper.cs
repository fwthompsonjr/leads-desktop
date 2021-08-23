using System;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search
{

    public enum StatusType
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
    public static class StatusHelper
    {
        public static StatusState GetStatus(StatusType status)
        {

            var v = (int)status - 1;
            var collectionName = Enum.GetName(typeof(StatusType), status);
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
