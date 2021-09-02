using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Tests.Data
{
    public class PersonAddressDto
    {

        #region FieldList Helpers

        private const string FieldNames = @"Name,FirstName,LastName,Zip," +
            @"Address1,Address2,Address3," +
            @"Case Number,Date Filed,Court," +
            @"Case Type,case style,plantiff";
        private string _fieldNames;
        private List<string> _fieldList;

        protected string LoweredFieldNames
        {
            get
            {
                return _fieldNames ?? (_fieldNames = FieldNames.ToLower(CultureInfo.CurrentCulture));
            }
        }

        public List<string> FieldList
        {
            get
            {
                return _fieldList ?? (
                  _fieldList = LoweredFieldNames.Split(',').ToList());
            }
        }

        #endregion

        #region Properties

        public string Name { get; set; }
        public string Zip { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CaseNumber { get; set; }
        public string DateFiled { get; set; }
        public string Court { get; set; }
        public string CaseType { get; set; }
        public string CaseStyle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Plantiff { get; set; }
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return false;
                if (string.IsNullOrEmpty(Zip)) return false;
                if (string.IsNullOrEmpty(Address1)) return false;
                return true;
            }
        }

        #endregion

    }
}
