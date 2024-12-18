﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace LegalLead.PublicData.Search.Attr
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeRequestAttribute : ValidationAttribute
    {
        public string Text { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (value is not string name) return false;
            if (string.IsNullOrWhiteSpace(name)) return false;
            return DateTime.TryParse(name,
                CultureInfo.CurrentCulture.DateTimeFormat,
                DateTimeStyles.AssumeLocal,
                out var _);
        }
    }
}
