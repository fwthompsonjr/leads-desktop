using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Models;

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
        public static T ChangeType<T>(this object source, T defaultValue = default)
        {
            var culture = CultureInfo.CurrentCulture;
            if (DBNull.Value == source || source == null) return defaultValue;
            Type t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (t == typeof(string))
            {
                var temp = Convert.ToString(source, culture);
                if (temp == null) return defaultValue;
                return (T)Convert.ChangeType(temp, t, culture);
            }
            return (T)Convert.ChangeType(source, t, culture) ?? defaultValue;
        }

        public static List<QueryDbResponse> ConvertFrom(this List<PersonAddress> addresses)
        {
            var list = new List<QueryDbResponse>();
            if (addresses == null) return list;
            addresses.ForEach(a => list.Add(new QueryDbResponse
            {
                Name = a.Name ?? string.Empty,
                Zip = a.Zip ?? string.Empty,
                Address1 = a.Address1 ?? string.Empty,
                Address2 = a.Address2 ?? string.Empty,
                Address3 = a.Address3 ?? string.Empty,
                CaseNumber = a.CaseNumber ?? string.Empty,
                DateFiled = a.DateFiled ?? string.Empty,
                Court = a.Court ?? string.Empty,
                CaseType = a.CaseType ?? string.Empty,
                CaseStyle = a.CaseStyle ?? string.Empty,
                Plaintiff = a.Plantiff ?? string.Empty,
            }));
            return list;

        }
        public static void BindGrid(this List<QueryDbResponse> source, DataGridView view)
        {
            var columnNames = new List<string>()
            {
                "DateFiled|File Date",
                "CaseNumber|Case Number",
                "Name",
                "FirstName|First Name",
                "LastName|Last Name",
                "Zip",
                "Address1",
                "Address2",
                "Address3",
                "Court",
                "CaseType|Case Type",
                "CaseStyle|Case Style",
                "Plaintiff",
                "County",
                "CourtAddress|Court Address",
            };

            // Clear existing columns
            view.Columns.Clear();

            // Add columns with specified headers and data property names
            for (int i = 0; i < columnNames.Count; i++)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    HeaderText = columnNames[i].Split('|')[^1],
                    DataPropertyName = columnNames[i].Split('|')[0],
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular),
                        ForeColor = System.Drawing.Color.Black,
                        BackColor = System.Drawing.Color.White,
                        SelectionForeColor = System.Drawing.Color.White,
                        SelectionBackColor = System.Drawing.Color.Blue
                    }
                };

                // Set display mode for the first two columns
                if (i < 2)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                }
                else
                {
                    // Set display mode for the other columns
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                view.Columns.Add(column);
            }

            // Set the DataSource
            view.DataSource = source;
            view.Refresh();
        }
    }
}
