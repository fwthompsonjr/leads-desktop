' ../generate-shortcut.vbs
Option Explicit
Dim Title,sh,ProcessPath,ProcessName,VersionName,VersionNumber,EnvLocation,VersionTag
Title = "Generate application shortcut using Vb Script"
VersionName = "2.8"
VersionNumber = VersionName & ".0"
VersionTag = "Legal Lead Search - v " & VersionNumber
EnvLocation = "%LOCALAPPDATA%\LegalLead\" & VersionName & "\LegalLead.PublicData.Search.exe"
Set sh = CreateObject("WScript.Shell")
ProcessPath = sh.ExpandEnvironmentStrings(EnvLocation)
CreateShortcut sh, ProcessPath, VersionTag


Set sh = Nothing

'****************************************************
Function DblQuote(Str)
    DblQuote = Chr(34) & Str & Chr(34)
End Function
'****************************************************

' -- create desktop shortcut
Sub CreateShortcut(shell, exeLocation, applicationTagLine) 

Dim fso, DesktopPath, DesktopLink
Dim shortName, iconLocation, workingDirectory
Dim link

    Set fso = CreateObject("Scripting.FileSystemObject")
    DesktopPath = shell.SpecialFolders("Desktop")
    DesktopLink = DesktopPath & "\legal-lead-search.lnk"
    If (fso.FileExists(exeLocation) = False) Then Exit Sub
    If (fso.FileExists(DesktopLink) = True) Then Exit Sub
    
    On Error Resume Next

    Set link = shell.CreateShortcut(DesktopLink)
    shortName = fso.GetFileName(exeLocation)
    workingDirectory = fso.GetParentFolderName(exeLocation)
    iconLocation = exeLocation & ",0"
    link.Arguments = "1 2 3"
    link.Description = applicationTagLine
    link.iconLocation = iconLocation
    link.TargetPath = exeLocation
    link.WindowStyle = 3
    link.workingDirectory = workingDirectory
    link.Save

    Set fso = Nothing
End Sub