' ../Shared/uninstall.vbs
Option Explicit
Dim Title,sh,rv,ProcessPath,ProcessName,RootPath,DownloadPath,XmlDataPath,XmlPath
Title = "Launching application using Vb Script"
Set sh = CreateObject("WScript.Shell")
RootPath = sh.ExpandEnvironmentStrings("%LOCALAPPDATA%\LegalLead\2.7")
DownloadPath = sh.ExpandEnvironmentStrings("%LOCALAPPDATA%\LegalLead\2.7\_downloads")
XmlDataPath = sh.ExpandEnvironmentStrings("%LOCALAPPDATA%\LegalLead\2.7\xml\data")
XmlPath = sh.ExpandEnvironmentStrings("%LOCALAPPDATA%\LegalLead\2.7\xml")
DeleteShortcut sh
DeleteFolder XmlDataPath
DeleteFolder XmlPath
DeleteFolder DownloadPath
DeleteFolder RootPath

Set sh = Nothing

'****************************************************
Function DblQuote(Str)
    DblQuote = Chr(34) & Str & Chr(34)
End Function
'****************************************************

' -- create desktop shortcut
Sub DeleteShortcut(shell) 

Dim fso, DesktopPath, DesktopLink

    Set fso = CreateObject("Scripting.FileSystemObject")
    DesktopPath = shell.SpecialFolders("Desktop")
    DesktopLink = DesktopPath & "\legal-lead-search.lnk"
    If (fso.FileExists(DesktopLink) = True) Then
        fso.DeleteFile DesktopLink, True
    End If
    
    On Error Resume Next

    Set fso = Nothing
End Sub

Sub DeleteFolder(path)
	Dim fso, folder, f
    Set fso = CreateObject("Scripting.FileSystemObject")
	if fso.FolderExists(path) = False Then 
		set fso = Nothing
		exit sub
	End if
	Set folder = fso.GetFolder(path)
	On Error Resume Next
	
	For Each f In folder.Files
	   Name = f.Name
	   f.Delete True
	Next
	folder.Delete
	Set folder = Nothing
	Set fso = Nothing

End Sub