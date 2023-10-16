using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Harris.Criminal.Db.Tables
{
    public class CaseStyleDb
    {
        public static readonly List<string> FieldNames = ("CaseNumber,Style,FileDate,Court,Status,TypeOfActionOrOffense").Split(',').ToList();

        [JsonProperty("cnbr")]
        public string CaseNumber { get; set; }
        [JsonProperty("style")]
        public string Style { get; set; }
        [JsonProperty("fdt")]
        public string FileDate { get; set; }
        [JsonProperty("court")]
        public string Court { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("toa")]
        public string TypeOfActionOrOffense { get; set; }


        public string this[int index]
        {
            get
            {
                if (index < 0 || index > FieldNames.Count - 1)
                {
                    return null;
                }
                switch (index)
                {
                    case 0: return CaseNumber;
                    case 1: return Style;
                    case 2: return FileDate;
                    case 3: return Court;
                    case 4: return Status;
                    case 5: return TypeOfActionOrOffense;
                    default:
                        return null;
                }
            }
            set
            {
                if (index < 0 || index > FieldNames.Count)
                {
                    return;
                }
                switch (index)
                {
                    case 0: CaseNumber = value; return;
                    case 1: Style = value; return;
                    case 2: FileDate = value; return;
                    case 3: Court = value; return;
                    case 4: Status = value; return;
                    case 5: TypeOfActionOrOffense = value; return;

                    default: return;
                }
            }
        }

        public string this[string fieldName]
        {
            get
            {
                var index =
                    FieldNames
                    .FindIndex(x => x.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                if (index < 0 || index > FieldNames.Count - 1)
                {
                    return null;
                }
                return this[index];
            }
            set
            {
                var index =
                    FieldNames
                    .FindIndex(x => x.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                if (index < 0 || index > FieldNames.Count)
                {
                    return;
                }
                this[index] = value;
            }
        }
    }
}
