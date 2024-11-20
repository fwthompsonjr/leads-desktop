using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace LegalLead.PublicData.Search.Extensions
{
    internal static class ObjectExtensions
    {
        public static List<ValidationResult> Validate<T>(this T source, out bool isValid) where T : class
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(source, context, validationResults, true);
            return validationResults;
        }
        public static T ChangeType<T>(object source)
        {
            var culture = CultureInfo.CurrentCulture;
            if (DBNull.Value == source || source == null) return default;
            Type t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (t == typeof(string))
            {
                var temp = Convert.ToString(source, culture);
                if (temp == null) return default;
                return (T)Convert.ChangeType(temp, t, culture);
            }
            return (T)Convert.ChangeType(source, t, culture);
        }
    }
}
