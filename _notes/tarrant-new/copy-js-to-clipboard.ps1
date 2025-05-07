<#
    copy js file to clipboard
#>

function replaceTabs($stringWithTabs) {
    $replacementString = "    "
    $newString = $stringWithTabs -replace "\t", $replacementString
    return $newString;
}
$nl = [Environment]::NewLine
$src = "C:\_d\lead-old\_notes\tarrant-new\get-defendant-address.js";
$shortName = [System.IO.Path]::GetFileNameWithoutExtension( $src );
[string]$content = [System.IO.File]::ReadAllText( $src );
$lines = $content.Split( $nl, [System.StringSplitOptions]::RemoveEmptyEntries);
$arr = @();
foreach($line in $lines) {
    $nwcontent = replaceTabs -stringWithTabs $line
    $a = '"{0}",' -f $nwcontent
    $arr += $a
}
$json = [string]::Join( $nl, $arr);
Set-Clipboard $json
Write-Host "Copy completed. File: $shortName" -ForegroundColor Green