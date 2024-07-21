function getVersionDetails($jsfile){
    if ( [System.IO.File]::Exists( $jsfile ) -eq $false ) { return $null; }
    $js = [System.IO.File]::ReadAllText( $jsfile ) | ConvertFrom-Json;
    return $js[0];
}

function updateGitVariable($key, $setting)
{
    try {
        $assignment="$key=$setting"
        echo $assignment | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append
    } catch {
    }
}

$basefolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$versionFile = [System.IO.Path]::Combine( $basefolder, "setup\setup-version.txt" ); 
$changelog = [System.IO.Path]::Combine( $basefolder, "CHANGELOG.md" ); 
if ( [System.IO.File]::Exists( $versionFile ) -eq $false ) { return; }
if ( [System.IO.File]::Exists( $changelog ) -eq $true )
{ 
   [System.IO.File]::Delete( $changelog ) 
}
$version = getVersionDetails -jsfile $versionFile
if ($null -eq $version ) { return; }
$versionDescription = [string]::Join([Environment]::NewLine, $version.description)
[System.IO.File]::WriteAllText( $changelog, $versionDescription);
$versionTitle = $version.title
$setupZip = [System.IO.Path]::Combine( $basefolder,"legal-lead-install-$($version.id).zip")
$excelZip = [System.IO.Path]::Combine( $basefolder,"legal-lead-excel-addin-$($version.id).zip")

    echo "ASSET_SETUP_FILE=$setupZip" | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append
    echo "ASSET_EXCEL_FILE=$excelZip" | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append
<#
updateGitVariable -key "RELEASE_TITLE" -setting $versionTitle
updateGitVariable -key "ASSET_SETUP_FILE" -setting $setupZip
updateGitVariable -key "ASSET_EXCEL_FILE" -setting $excelZip
#>