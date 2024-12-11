using System;
using System.ComponentModel.DataAnnotations;

namespace LegalLead.PublicData.Search.Attr
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NumericRequestAttribute : ValidationAttribute
    {
        public string Text { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (value is not string name) return false;
            if (string.IsNullOrWhiteSpace(name)) return false;
            return int.TryParse(name,
                out var _);
        }
    }
}
