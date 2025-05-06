<#
    copy js file to clipboard
#>
$nl = [Environment]::NewLine
$src = "C:\_d\lead-old\_notes\tarrant-new\select-search-context.js"
[string]$content = [System.IO.File]::ReadAllText( $src );
$lines = $content.Split( $nl, [System.StringSplitOptions]::RemoveEmptyEntries);
$arr = @();
foreach($line in $lines) {
    $a = '"{0}",' -f $line
    $arr += $a
}
$json = [string]::Join( $nl, $arr);
Set-Clipboard $json
Write-Host "Copy completed"