<#

        FileVersionInfo.GetVersionInfo(Path.Combine(Environment.SystemDirectory, "Notepad.exe"));

#>

$exfile = "C:\ProgramData\ll\DALLAS_COUNTY_241202_241208_0020.XLSX";
$vi = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($exfile);
$vi

$vi.FileVersion = "1.0.0"