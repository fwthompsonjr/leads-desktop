' ../Shared/launch-exe.vbs
Option Explicit
Dim Title,sh,rv,ProcessPath,ProcessName
Title = "Launching application using Vb Script"
Set sh = CreateObject("WScript.Shell")
ProcessPath = sh.ExpandEnvironmentStrings("%LOCALAPPDATA%\LegalLead\2.7\LegalLead.PublicData.Search.exe")
CreateShortcut sh, ProcessPath

rv = sh.Run(DblQuote(ProcessPath),1,False)
If rv <> 0 Then
     MsgBox "Failed : " & rv
End If

Set sh = Nothing

'****************************************************
Function DblQuote(Str)
    DblQuote = Chr(34) & Str & Chr(34)
End Function
'****************************************************

' -- create desktop shortcut
Sub CreateShortcut(shell, exeLocation) 

Dim fso, DesktopPath, DesktopLink
Dim shortName, iconLocation, workingDirectory
Dim link

    Set fso = CreateObject("Scripting.FileSystemObject")
    DesktopPath = shell.SpecialFolders("Desktop")
    DesktopLink = DesktopPath & "\legal-lead-search.lnk"
    If (fso.FileExists(exeLocation) = False) Then Exit Sub
    If (fso.FileExists(DesktopLink) = True) Then
        fso.DeleteFile DesktopLink, True
    End If
    
    On Error Resume Next

    Set link = shell.CreateShortcut(DesktopLink)
    shortName = fso.GetFileName(exeLocation)
    workingDirectory = fso.GetParentFolderName(exeLocation)
    iconLocation = exeLocation & ",0"
    link.Arguments = "1 2 3"
    link.Description = "Legal Lead Search - v 2.7.1"
    link.iconLocation = iconLocation
    link.TargetPath = exeLocation
    link.WindowStyle = 3
    link.workingDirectory = workingDirectory
    link.Save

    Set fso = Nothing
End Sub