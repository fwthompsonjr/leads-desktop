using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Harris.Criminal.Db.Downloads
{
    public class HarrisCriminalDto
    {

        public static List<string> AliasNames = ("index," +
            "rundate,cdi,cas,fda,ins,cad,crt,cst,dst," +
            "bam,curr_off,curr_off_lit,curr_l_d," +
            "nda,cnc,rea,def_nam,def_spn,def_rac," +
            "def_sex,def_dob,def_stnum,def_stnam," +
            "def_cty,def_st,def_zip," +
            "aty_nam,aty_spn,aty_coc,aty_coc_lit," +
            "def_birthplace,def_uscitizen").Split(',').ToList();

        public static List<string> FieldNames = ("Index," +
            "DateDatasetProduced,CourtDivisionIndicator,CaseNumber," +
            "FilingDate,InstrumentType,CaseDisposition,Court," +
            "CaseStatus,DefendantStatus,BondAmount,CurrentOffense," +
            "CurrentOffenseLiteral,CurrentLevelAndDegree,NextAppearanceDate," +
            "DocketCalendarNameCode,CalendarReason," +
            "DefendantName,DefendantSPN,DefendantRace,DefendantSex,DefendantDateOfBirth," +
            "DefendantStreetNumber,DefendantStreetName,DefendantCity,DefendantState,DefendantZip," +
            "AttorneyName,AttorneySPN,AttorneyConnectionCode,AttorneyConnectionLiteral," +
            "DefendantPlaceOfBirth,DefUSCitizenFlag").Split(',').ToList();

        public int Index { get; set; }
        public string DateDatasetProduced { get; set; }
        public string CourtDivisionIndicator { get; set; }
        public string CaseNumber { get; set; }
        public string FilingDate { get; set; }
        public string InstrumentType { get; set; }
        public string CaseDisposition { get; set; }
        public string Court { get; set; }
        public string CaseStatus { get; set; }
        public string DefendantStatus { get; set; }
        public string BondAmount { get; set; }
        public string CurrentOffense { get; set; }
        public string CurrentOffenseLiteral { get; set; }
        public string CurrentLevelAndDegree { get; set; }
        public string NextAppearanceDate { get; set; }
        public string DocketCalendarNameCode { get; set; }
        public string CalendarReason { get; set; }
        public string DefendantName { get; set; }
        public string DefendantSPN { get; set; }
        public string DefendantRace { get; set; }
        public string DefendantSex { get; set; }
        public string DefendantDateOfBirth { get; set; }
        public string DefendantStreetNumber { get; set; }
        public string DefendantStreetName { get; set; }
        public string DefendantCity { get; set; }
        public string DefendantState { get; set; }
        public string DefendantZip { get; set; }
        public string AttorneyName { get; set; }
        public string AttorneySPN { get; set; }
        public string AttorneyConnectionCode { get; set; }
        public string AttorneyConnectionLiteral { get; set; }
        public string DefendantPlaceOfBirth { get; set; }
        public string DefUSCitizenFlag { get; set; }

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
                    case 1: return DateDatasetProduced;
                    case 2: return CourtDivisionIndicator;
                    case 3: return CaseNumber;
                    case 4: return FilingDate;
                    case 5: return InstrumentType;
                    case 6: return CaseDisposition;
                    case 7: return Court;
                    case 8: return CaseStatus;
                    case 9: return DefendantStatus;
                    case 10: return BondAmount;
                    case 11: return CurrentOffense;
                    case 12: return CurrentOffenseLiteral;
                    case 13: return CurrentLevelAndDegree;
                    case 14: return NextAppearanceDate;
                    case 15: return DocketCalendarNameCode;
                    case 16: return CalendarReason;
                    case 17: return DefendantName;
                    case 18: return DefendantSPN;
                    case 19: return DefendantRace;
                    case 20: return DefendantSex;
                    case 21: return DefendantDateOfBirth;
                    case 22: return DefendantStreetNumber;
                    case 23: return DefendantStreetName;
                    case 24: return DefendantCity;
                    case 25: return DefendantState;
                    case 26: return DefendantZip;
                    case 27: return AttorneyName;
                    case 28: return AttorneySPN;
                    case 29: return AttorneyConnectionCode;
                    case 30: return AttorneyConnectionLiteral;
                    case 31: return DefendantPlaceOfBirth;
                    case 32: return DefUSCitizenFlag;
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
                    case 1: DateDatasetProduced = value; return;
                    case 2: CourtDivisionIndicator = value; return;
                    case 3: CaseNumber = value; return;
                    case 4: FilingDate = value; return;
                    case 5: InstrumentType = value; return;
                    case 6: CaseDisposition = value; return;
                    case 7: Court = value; return;
                    case 8: CaseStatus = value; return;
                    case 9: DefendantStatus = value; return;
                    case 10: BondAmount = value; return;
                    case 11: CurrentOffense = value; return;
                    case 12: CurrentOffenseLiteral = value; return;
                    case 13: CurrentLevelAndDegree = value; return;
                    case 14: NextAppearanceDate = value; return;
                    case 15: DocketCalendarNameCode = value; return;
                    case 16: CalendarReason = value; return;
                    case 17: DefendantName = value; return;
                    case 18: DefendantSPN = value; return;
                    case 19: DefendantRace = value; return;
                    case 20: DefendantSex = value; return;
                    case 21: DefendantDateOfBirth = value; return;
                    case 22: DefendantStreetNumber = value; return;
                    case 23: DefendantStreetName = value; return;
                    case 24: DefendantCity = value; return;
                    case 25: DefendantState = value; return;
                    case 26: DefendantZip = value; return;
                    case 27: AttorneyName = value; return;
                    case 28: AttorneySPN = value; return;
                    case 29: AttorneyConnectionCode = value; return;
                    case 30: AttorneyConnectionLiteral = value; return;
                    case 31: DefendantPlaceOfBirth = value; return;
                    case 32: DefUSCitizenFlag = value; return;

                    default: return;
                }
            }
        }

        public static List<HarrisCriminalDto> Map(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return default;
            if (!File.Exists(fileName)) return default;
            var result = new List<HarrisCriminalDto>();
            using (var sreader = new StreamReader(fileName))
            {
                var index = 0;
                var line = sreader.ReadLine();
                while (line != null)
                {
                    if (index <= 0)
                    {
                        index++;
                        continue;
                    }
                    line = sreader.ReadLine();
                    if (line != null)
                    {
                        result.Add(Parse(index, line));
                    }
                    index++;
                }
            }
            return result;
        }

        private static HarrisCriminalDto Parse(int index, string line)
        {
            const string delimiter = "\t";
            var record = new HarrisCriminalDto
            {
                Index = index
            };
            var fields = line.Split(delimiter.ToCharArray());
            for (int i = 0; i < fields.Length; i++)
            {
                var data = fields[i];
                record[i + 1] = data;
            }
            return record;
        }
    }
}
