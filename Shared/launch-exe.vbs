' ../Shared/launch-exe.vbs
Option Explicit
Dim FSO,Title,sh,rv,ProcessPath,ProcessName,versionTag,exePath
Title = "Launching application using Vb Script"
Set sh = CreateObject("WScript.Shell")
versionTag = "2.8"
exePath = "%LOCALAPPDATA%\LegalLead\" & versionTag & "\LegalLead.PublicData.Search.exe"
ProcessPath = sh.ExpandEnvironmentStrings(exePath)

Set FSO = CreateObject("Scripting.FileSystemObject")

If FSO.FileExists(ProcessPath) Then
    'Launch program
    rv = sh.Run(DblQuote(ProcessPath),1,False)
    If rv <> 0 Then
         MsgBox "Failed : " & rv
    End If
End If

Set sh = Nothing
Set FSO = Nothing

'****************************************************
Function DblQuote(Str)
    DblQuote = Chr(34) & Str & Chr(34)
End Function
'****************************************************