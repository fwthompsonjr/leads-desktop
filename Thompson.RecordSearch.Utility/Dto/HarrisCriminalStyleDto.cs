using System;
using System.Collections.Generic;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class HarrisCriminalStyleDto
    {

        public static readonly List<string> FieldNames = ("Index," +
            "CaseNumber,Style,FileDate,Court,Status,TypeOfActionOrOffense").Split(',').ToList();
        public int Index { get; set; }
        public string CaseNumber { get; set; }
        public string Style { get; set; }
        public string FileDate { get; set; }
        public string Court { get; set; }
        public string Status { get; set; }
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
                    case 0: return Index.ToString("D");
                    case 1: return CaseNumber;
                    case 2: return Style;
                    case 3: return FileDate;
                    case 4: return Court;
                    case 5: return Status;
                    case 6: return TypeOfActionOrOffense;
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
                    case 0: Index = Convert.ToInt32(value); return;
                    case 1: CaseNumber = value; return;
                    case 2: Style = value; return;
                    case 3: FileDate = value; return;
                    case 4: Court = value; return;
                    case 5: Status = value; return;
                    case 6: TypeOfActionOrOffense = value; return;

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
