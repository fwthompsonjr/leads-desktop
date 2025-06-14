using StructureMap;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FsMyProfile
    {
        protected bool ValidateUserInput()
        {
            txErrorMessages.Clear();
            txErrorMessages.ForeColor = Color.Black;
            SetControlColor(false);
            var groupName = cboProfileGroup.SelectedItem as string;
            if (string.IsNullOrEmpty(groupName)) return false;
            var validationName = (groupName.StartsWith("Address")) ? "Address" : groupName;
            var validator = ValidationContainer.GetContainer.GetInstance<ICanValidate>(validationName);
            if (validator == null) return false;
            var db = changes.FindAll(x => x.ProfileGroup == groupName);
            if (db == null || db.Count == 0)
            {
                txErrorMessages.Text = "No changes found.";
                return false;
            }
            var values = new List<string>()
            {
                tbxFieldValue01.Text,
                tbxFieldValue02.Text,
                tbxFieldValue03.Text,
            };
            var names = new List<string>()
            {
                tbxFieldName01.Text,
                tbxFieldName02.Text,
                tbxFieldName03.Text,
            };
            if (validationName == "Address")
            {
                var address = string.Join(" ", values.Where(s => !string.IsNullOrEmpty(s)));
                var response = validator.Validate(new() { Text = address });
                if (!response.IsValid)
                {
                    SetControlColor(true);
                    var message = string.Join(Environment.NewLine, response.Messages);
                    var arr = new List<string>()
                    {
                        "Please correct errors in your submission.",
                        message
                    };
                    txErrorMessages.Text = string.Join(Environment.NewLine, arr);
                    txErrorMessages.ForeColor = Color.Red;
                }
                return response.IsValid;
            }
            var sb = new StringBuilder();
            var responses = new List<ValidationModel>();
            values.ForEach(v =>
            {
                if (string.IsNullOrEmpty(v))
                {
                    responses.Add(new ValidationModel { IsValid = true });
                }
                else
                {
                    responses.Add(validator.Validate(new() { Text = v }));
                }
            });
            var isFailed = false;
            names.ForEach(n =>
            {
                var id = names.IndexOf(n);
                var item = responses[id];
                if (!item.IsValid && !isFailed)
                {
                    isFailed = true;
                    sb.AppendLine("Please correct errors in your submission.");

                }
                if (!item.IsValid)
                {
                    sb.AppendLine($"Please check input for {n.Replace(':',' ').Trim()} :" + string.Join(Environment.NewLine, item.Messages));
                }
            });
            txErrorMessages.ForeColor = isFailed ? Color.Red : Color.Black;
            if (isFailed)
            {
                txErrorMessages.Text = sb.ToString();
                SetControlColor(true);
            }
            return !isFailed;
        }

        private void SetControlColor(bool hasError)
        {
            var color = hasError ? Color.Red : Color.Black;
            var textboxes = new List<TextBox>() { tbxFieldValue01, tbxFieldValue02, tbxFieldValue03 };
            textboxes.ForEach(t => t.ForeColor = color);
        }

        #region Attributes
        [AttributeUsage(AttributeTargets.Property)]
        private class ValidUSAddressAttribute : ValidationAttribute
        {
            private static readonly Regex[] AddressPatterns = new[]
            {
                    // Standard street address: 123 Main St, Springfield, IL 62704
                    new Regex(@"^\d+\s+[\w\s]+\s+(Street|St|Avenue|Ave|Boulevard|Blvd|Road|Rd|Lane|Ln|Drive|Dr|Court|Ct|Circle|Cir|Way|Terrace|Ter|Place|Pl)\,?\s+[\w\s]+\,?\s+[A-Z]{2}\s+\d{5}(-\d{4})?$", RegexOptions.IgnoreCase),

                    // PO Box: PO Box 123, Springfield, IL 62704
                    new Regex(@"^(P\.?O\.?|Post\sOffice)\sBox\s\d+\,?\s+[\w\s]+\,?\s+[A-Z]{2}\s+\d{5}(-\d{4})?$", RegexOptions.IgnoreCase),

                    // Military address: PSC 123 Box 4567, APO AE 09012
                    new Regex(@"^(PSC|Unit)\s\d+\s(Box|BOX)\s\d+\,?\s+(APO|FPO|DPO)\s+(AA|AE|AP)\s+\d{5}(-\d{4})?$", RegexOptions.IgnoreCase)
            };

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var address = value as string;
                if (string.IsNullOrWhiteSpace(address))
                {
                    return new ValidationResult("Address is required.");
                }

                foreach (var pattern in AddressPatterns)
                {
                    if (pattern.IsMatch(address))
                    {
                        return ValidationResult.Success;
                    }
                }

                return new ValidationResult("Invalid US postal address format.");
            }
        }

        [AttributeUsage(AttributeTargets.Property)]
        private class ValidPhoneNumberAttribute : ValidationAttribute
        {
            private static readonly Regex[] PhonePatterns = new[]
            {
                    // Standard phone xxx-xxx-xxxx
                    new Regex(@"^[0-9]{3}-[0-9]{3}-[0-9]{4}$", RegexOptions.IgnoreCase),

                    // Standard phone (xxx)-xxx-xxxx
                    new Regex(@"^\([0-9]{3}\)[0-9]{3}-[0-9]{4}$", RegexOptions.IgnoreCase),
                    new Regex(@"^(?:\+1)?\s?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$", RegexOptions.IgnoreCase),
                    //7 or 10-digit phone number, extension allowed
                    new Regex(@"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$", RegexOptions.IgnoreCase)
            };

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var phone = value as string;
                if (string.IsNullOrWhiteSpace(phone))
                {
                    return new ValidationResult("Phone number is required.");
                }


                foreach (var pattern in PhonePatterns)
                {
                    if (pattern.IsMatch(phone))
                    {
                        return ValidationResult.Success;
                    }
                }
                return new ValidationResult("Invalid phone number format.");
            }
        }
        #endregion

        #region Models        

        private class ValidationModel
        {
            public string Text { get; set; } = string.Empty;
            public bool IsValid { get; set; }
            public List<string> Messages { get; set; } = [];
        }
        private class PhoneNumberModel
        {
            [ValidPhoneNumber]
            public string Number { get; set; }
        }
        private class AddressModel
        {
            [Required]
            [ValidUSAddress]
            public string AddressBlock { get; set; }
        }
        private class EmailModel
        {
            [Required]
            [EmailAddress(ErrorMessage = "Invalid email address format.")]
            [StringLength(256, ErrorMessage = "Email address cannot exceed 256 characters.")]
            public string Email { get; set; }
        }

        #endregion

        #region Validators
        private interface ICanValidate
        {
            ValidationModel Validate(ValidationModel model);
        }
        private class AddressValidator : ICanValidate
        {
            public ValidationModel Validate(ValidationModel model)
            {
                var response = new ValidationModel { Text = model.Text };
                var address = new AddressModel { AddressBlock = model.Text };
                var context = new ValidationContext(address);
                var results = new List<ValidationResult>();

                response.IsValid = Validator.TryValidateObject(address, context, results, true);
                if (response.IsValid) return response;
                foreach (var validationResult in results)
                {
                    response.Messages.Add(validationResult.ErrorMessage);
                }

                return response;
            }
        }

        private class EmailValidator : ICanValidate
        {
            public ValidationModel Validate(ValidationModel model)
            {
                var response = new ValidationModel { Text = model.Text };
                var address = new EmailModel { Email = model.Text };
                var context = new ValidationContext(address);
                var results = new List<ValidationResult>();

                response.IsValid = Validator.TryValidateObject(address, context, results, true);
                if (response.IsValid) return response;
                foreach (var validationResult in results)
                {
                    response.Messages.Add(validationResult.ErrorMessage);
                }

                return response;
            }

        }

        private class PhoneValidator : ICanValidate
        {
            public ValidationModel Validate(ValidationModel model)
            {
                var response = new ValidationModel { Text = model.Text };
                var address = new PhoneNumberModel { Number = model.Text };
                var context = new ValidationContext(address);
                var results = new List<ValidationResult>();

                response.IsValid = Validator.TryValidateObject(address, context, results, true);
                if (response.IsValid) return response;
                foreach (var validationResult in results)
                {
                    response.Messages.Add(validationResult.ErrorMessage);
                }

                return response;
            }
        }
        private class NameValidator : ICanValidate
        {
            public ValidationModel Validate(ValidationModel model)
            {
                var response = new ValidationModel { Text = model.Text, IsValid = true };
                return response;
            }
        }
        private static class ValidationContainer
        {
            private static Container _container;

            /// <summary>
            /// Gets the container.
            /// </summary>
            /// <value>
            /// The container of registered interfaces.
            /// </value>
            public static Container GetContainer
            {
                get
                {
                    return _container ??= new Container(new ValidationRegistry());
                }
            }
        }
        private class ValidationRegistry : Registry
        {
            public ValidationRegistry()
            {
                For<ICanValidate>().Add<NameValidator>().Named("Name");
                For<ICanValidate>().Add<PhoneValidator>().Named("Phone");
                For<ICanValidate>().Add<EmailValidator>().Named("Email");
                For<ICanValidate>().Add<AddressValidator>().Named("Address");
            }
        }
        #endregion
    }
}