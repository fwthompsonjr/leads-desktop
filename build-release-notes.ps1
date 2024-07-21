function getVersionDetails($jsfile){
    if ( [System.IO.File]::Exists( $jsfile ) -eq $false ) { return $null; }
    $js = [System.IO.File]::ReadAllText( $jsfile ) | ConvertFrom-Json;
    return $js[0];
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
