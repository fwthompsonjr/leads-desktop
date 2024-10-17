' ../Shared/uninstall.vbs
Option Explicit
Dim Title,sh,rv,ProcessPath,ProcessName,RootPath,DownloadPath,XmlDataPath,XmlPath
Dim versionTag, versionPrefix
Title = "Launching application using Vb Script"
Set sh = CreateObject("WScript.Shell")
versionTag = "2.8"
versionPrefix = "%LOCALAPPDATA%\LegalLead\" & versionTag
RootPath = sh.ExpandEnvironmentStrings(versionPrefix)
DownloadPath = sh.ExpandEnvironmentStrings(versionPrefix & "\_downloads")
XmlDataPath = sh.ExpandEnvironmentStrings(versionPrefix & "\xml\data")
XmlPath = sh.ExpandEnvironmentStrings(versionPrefix & "\xml")
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