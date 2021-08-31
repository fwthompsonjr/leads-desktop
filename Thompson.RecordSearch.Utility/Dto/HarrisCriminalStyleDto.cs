using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class HarrisCriminalStyleDto
    {

        public static readonly List<string> FieldNames = ("Index," +
            "CaseNumber,Style,FileDate,Court,Status,TypeOfActionOrOffense,Defendant,Plantiff").Split(',').ToList();
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }
        /// <summary>
        /// Gets or sets the case number.
        /// </summary>
        /// <value>
        /// The case number.
        /// </value>
        [JsonProperty("cnbr")]
        public string CaseNumber { get; set; }
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        [JsonProperty("style")]
        public string Style { get; set; }
        /// <summary>
        /// Gets or sets the file date.
        /// </summary>
        /// <value>
        /// The file date.
        /// </value>
        [JsonProperty("fdt")]
        public string FileDate { get; set; }
        /// <summary>
        /// Gets or sets the court.
        /// </summary>
        /// <value>
        /// The court.
        /// </value>
        [JsonProperty("court")]
        public string Court { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the type of action or offense.
        /// </summary>
        /// <value>
        /// The type of action or offense.
        /// </value>
        [JsonProperty("toa")]
        public string TypeOfActionOrOffense { get; set; }

        /// <summary>
        /// Gets or sets the defendant.
        /// </summary>
        /// <value>
        /// The defendant.
        /// </value>
        public string Defendant { get; set; }

        /// <summary>
        /// Gets or sets the plantiff.
        /// </summary>
        /// <value>
        /// The plantiff.
        /// </value>
        public string Plantiff { get; set; }

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
                    case 0: return Index.ToString("D", CultureInfo.InvariantCulture);
                    case 1: return CaseNumber;
                    case 2: return Style;
                    case 3: return FileDate;
                    case 4: return Court;
                    case 5: return Status;
                    case 6: return TypeOfActionOrOffense;
                    case 7: return Defendant;
                    case 8: return Plantiff;
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
                    case 0: Index = Convert.ToInt32(value, CultureInfo.InvariantCulture); return;
                    case 1: CaseNumber = value; return;
                    case 2: Style = value; return;
                    case 3: FileDate = value; return;
                    case 4: Court = value; return;
                    case 5: Status = value; return;
                    case 6: TypeOfActionOrOffense = value; return;
                    case 7: Defendant = value; return;
                    case 8: Plantiff = value; return;

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
