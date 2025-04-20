using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Extensions
{
    public static class PackageExtensions
    {
        public static Dictionary<string, int> GetColumns(this ExcelPackage package)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            var columns = new Dictionary<string, int>();
            var worksheet = package.Workbook.Worksheets.First();
            var columnCount = worksheet.Dimension.End.Column;
            for (int i = 0; i < columnCount; i++)
            {
                var idx = i + 1;
                var name = Convert.ToString(worksheet.GetValue(1, idx), culture);
                if (string.IsNullOrEmpty(name)) continue;
                name = GetUniqueName(name, columns);
                columns.Add(name, idx);
            }
            return columns;
        }

        public static void PopulateColumn(
            this ExcelPackage package,
            string destination,
            List<string> values)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            if (values == null) throw new ArgumentNullException(nameof(values));
            var columns = package.GetColumns();
            var hasDestination = columns.TryGetValue(destination, out var destinationId);
            if (!hasDestination) return;
            var worksheet = package.Workbook.Worksheets.First();
            var rowCount = worksheet.Dimension.End.Row;
            for (int i = 0; i < rowCount; i++)
            {
                if (i > values.Count - 1) continue;
                var idx = i + 2;
                UpdateValue(worksheet, idx, destinationId, values[i]);
            }
        }
        public static void TransferColumn(this ExcelPackage package, string source, string destination)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            var columns = package.GetColumns();
            var hasSource = columns.TryGetValue(source, out var sourceId);
            var hasDestination = columns.TryGetValue(destination, out var destinationId);
            if (!hasDestination || !hasSource) return;
            var worksheet = package.Workbook.Worksheets.First();
            var rowCount = worksheet.Dimension.End.Row;
            for (int i = 0; i < rowCount; i++)
            {
                var idx = i + 1;
                var src = Convert.ToString(worksheet.GetValue(idx, sourceId), culture);
                worksheet.Cells[idx, destinationId].Value = src;
                worksheet.Cells[idx, sourceId].Value = null;
            }
        }



        private static string GetUniqueName(string name, Dictionary<string, int> columns)
        {
            if (!IsDuplicateName(name, columns)) return name;
            var id = 1;
            var originalName = name;
            while (IsDuplicateName(name, columns))
            {
                name = $"{originalName}_{id}";
                id++;
            }
            return name;
        }

        private static bool IsDuplicateName(string name, Dictionary<string, int> columns)
        {
            return columns.Count(x => x.Key == name) > 1;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private static void UpdateValue(ExcelWorksheet worksheet, int rowNumber, int columnNumber, string data)
        {
            try
            {
                worksheet.Cells[rowNumber, columnNumber].Value = data;
            }
            catch (Exception)
            {
                // ignoring errors from this method
            }
        }

        private static readonly CultureInfo culture = new CultureInfo("en-US");
    }
}
