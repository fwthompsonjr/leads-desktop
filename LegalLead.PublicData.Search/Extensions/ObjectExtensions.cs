using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    }
}
