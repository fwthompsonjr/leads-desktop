using OfficeOpenXml;
using System;
using System.IO;
using System.Text;
using Thompson.RecordSearch.Utility.Interfaces;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ExcelFileWriter : IExcelFileWriter
    {

        public void SaveAs(ExcelPackage pck, string outputFileName)
        {
            if (pck == null)
            {
                throw new System.ArgumentNullException(nameof(pck));
            }

            if (string.IsNullOrEmpty(outputFileName))
            {
                throw new System.ArgumentNullException(nameof(outputFileName));
            }

            FileInfo fileInfo = new FileInfo(outputFileName);
            var macro = new ExcelMacroBase { FileName = outputFileName, Package = pck };
            macro.AppendModule();
            pck.SaveAs(fileInfo);
        }

        private static string _macroCode;
        private static string GetMacroCode()
        {
            if (!string.IsNullOrEmpty(_macroCode))
            {
                return _macroCode;
            }
            var sbb = new StringBuilder();
            sbb.AppendLine("Option Explicit");
            sbb.AppendLine("");
            sbb.AppendLine("Public Sub RemoveHardReturnFromCurrentColumn()");
            sbb.AppendLine("On Error GoTo ErrHandler");
            sbb.AppendLine("");
            sbb.AppendLine("Dim rsp");
            sbb.AppendLine("Dim wb As Workbook");
            sbb.AppendLine("Dim wk As Worksheet");
            sbb.AppendLine("Dim rg As Range");
            sbb.AppendLine("Dim columnIndex As Long");
            sbb.AppendLine("Dim rowCount As Long");
            sbb.AppendLine("Dim rr As Long");
            sbb.AppendLine("Dim content As String");
            sbb.AppendLine("Dim columnName As String");
            sbb.AppendLine("");
            sbb.AppendLine("Set wb = ActiveWorkbook");
            sbb.AppendLine("Set wk = ActiveSheet");
            sbb.AppendLine("Set rg = ActiveCell");
            sbb.AppendLine("");
            sbb.AppendLine("columnIndex = rg.Column");
            sbb.AppendLine("columnName = wk.Cells(1, columnIndex)");
            sbb.AppendLine("rowCount = rg.CurrentRegion.Rows.Count - 1");
            sbb.AppendLine("");
            sbb.AppendLine("");
            sbb.AppendLine("rsp = MsgBox(~Would you like to remove carriage returns from column [~ & columnName & ~]~, vbQuestion + vbYesNo, ~Column Carriage Return~)");
            sbb.AppendLine("If rsp <> vbYes Then Exit Sub");
            sbb.AppendLine("");
            sbb.AppendLine("For rr = 2 To rowCount");
            sbb.AppendLine("    content = wk.Cells(rr, columnIndex)");
            sbb.AppendLine("    If Not IsNull(content) Then");
            sbb.AppendLine("        content = Trim(content)");
            sbb.AppendLine("        content = Replace(content, vbCrLf, ~ ~)");
            sbb.AppendLine("        content = Replace(content, vbCr, ~ ~)");
            sbb.AppendLine("        If 0 < InStr(content, Chr(10)) Then");
            sbb.AppendLine("            content = Replace(content, Chr(10), ~ ~)");
            sbb.AppendLine("        End If");
            sbb.AppendLine("        content = Replace(content, ~  ~, ~ ~)");
            sbb.AppendLine("        content = Trim(content)");
            sbb.AppendLine("        wk.Cells(rr, columnIndex) = content");
            sbb.AppendLine("    End If");
            sbb.AppendLine("Next");
            sbb.AppendLine("");
            sbb.AppendLine("MsgBox ~Process completed.~, vbInformation, ~Column Formatting~");
            sbb.AppendLine("");
            sbb.AppendLine("ErrExit:");
            sbb.AppendLine("Exit Sub");
            sbb.AppendLine("");
            sbb.AppendLine("ErrHandler:");
            sbb.AppendLine("MsgBox ~An error occurring during formatting.~ & vbCrLf _");
            sbb.AppendLine("& ~ Err: ~ & Err.Number & vbCrLf _");
            sbb.AppendLine("& Err.Description, vbInformation + vbOKOnly, ~Column Formatting Error~");
            sbb.AppendLine("Resume ErrExit");
            sbb.AppendLine("    ");
            sbb.AppendLine("End Sub");
            sbb.Replace("~", '"'.ToString(System.Globalization.CultureInfo.CurrentCulture));
            _macroCode = sbb.ToString();
            return _macroCode;
        }

        private class ExcelMacroBase
        {
            public string FileName { get; set; }

            public ExcelPackage Package { get; set; }

            public void AppendModule()
            {
                if (string.IsNullOrEmpty(FileName))
                {
                    return;
                }

                if (Package == null)
                {
                    return;
                }

                var ccic = StringComparison.CurrentCultureIgnoreCase;
                var extension = Path.GetExtension(FileName);
                if (!extension.Equals(CommonKeyIndexes.ExtensionXlsm, ccic))
                {
                    return;
                }

                var pck = Package;
                // Add VBA Code
                pck.Workbook.CreateVBAProject();         // Can not append VBA Projects.Multiple "Code" can be added. See "https://github.com/pruiz/EPPlus/blob/master/SampleApp/Sample15.cs".
                pck.Workbook.CodeModule.Name = "Module1";
                pck.Workbook.CodeModule.Code = GetMacroCode();
            }
        }

    }
}
