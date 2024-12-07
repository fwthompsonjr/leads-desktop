using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LegalLead.PublicData.Search.Attr
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BooleanRequestAttribute : ValidationAttribute
    {
        private static readonly List<string> types = "True,False".Split(',').ToList();
        public string Text { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (value is not string name) return false;
            if (string.IsNullOrWhiteSpace(name)) return false;
            return types.Exists(t => t.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
