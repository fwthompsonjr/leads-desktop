using legallead.permissions.api.Entity;
using legallead.permissions.api.Model;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api
{
    public static class ObjectExtensions
    {    public static T? GetObjectFromHeader<T>(this HttpRequest request, string headerName) where T : class
        {
            try
            {

                if (request == null || string.IsNullOrEmpty(headerName)) return default;
                if (!request.Headers.TryGetValue(headerName, out var strings)) return default;
                var source = strings.ToString();
                if (string.IsNullOrEmpty(source)) return default;
                var response = JsonConvert.DeserializeObject<T>(source);
                return response;
            }
            catch
            {
                return default;
            }
        }

        public static KeyValuePair<bool, string> Validate(this HttpRequest request, DataProvider db, string response)
        {
            var pair = new KeyValuePair<bool, string>(true, "application is valid");

            var application = request.GetObjectFromHeader<ApplicationRequestModel>("APP_IDENTITY");
            if (application == null)
            {
                return new KeyValuePair<bool, string>(false, response); ;
            }
            var apperrors = application.Validate(out bool isAppValid);
            if (!application.Id.HasValue || !isAppValid)
            {
                response = string.Join(';', apperrors.Select(m => m.ErrorMessage));
                return new KeyValuePair<bool, string>(false, response);
            }
            var targetApplication = new ApplicationEntity { Id = application.Id.Value.ToString("D") };
            var matched = db.Get(targetApplication, t => { return t.Id == targetApplication.Id; });
            if (matched == null || !(matched.Name ?? "").Equals(application.Name))
            {
                response = "Target application is not found or mismatched.";
                return new KeyValuePair<bool, string>(false, response);
            }
            return pair;
        }

        public static List<ValidationResult> Validate<T>(this T source, out bool isValid) where T : class
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(source, context, validationResults, true);
            return validationResults;
        }
    }
}
