function getVersionDetails($jsfile){
    if ( [System.IO.File]::Exists( $jsfile ) -eq $false ) { return $null; }
    $js = [System.IO.File]::ReadAllText( $jsfile ) | ConvertFrom-Json;
    return $js[0];
}

$basefolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$versionFile = [System.IO.Path]::Combine( $basefolder, "setup\setup-version.txt" ); 
$version = getVersionDetails -jsfile $versionFile
$sources = @("setup\release\setup.exe", "setup\release\setup.msi");
$hasSource = [System.IO.File]::Exists( $versionFile ) 
if ( $hasSource -eq $false ) { return; }
$destination = [System.IO.Path]::Combine( $basefolder, "version\$($version.id)" );
$zipname = [System.IO.Path]::Combine( $basefolder, "legal-lead-install-$($version.id).zip" );
$installname = [System.IO.Path]::Combine( $basefolder, "installation.zip" );
if ( [System.IO.Directory]::Exists( $destination ) -eq $false ) { 
    [System.IO.Directory]::CreateDirectory( $destination ) | Out-Null
}
$srcfiles = "";
foreach($s in $sources)
{
    $src = [System.IO.Path]::Combine( $basefolder, $s ); 
    $sname = [System.IO.Path]::GetFileName( $src );
    $dest = [System.IO.Path]::Combine( $destination, $sname ); 
    if ( [System.IO.File]::Exists( $src ) -eq $true -and [System.IO.File]::Exists( $dest ) -eq $false ) {
        [System.IO.File]::Copy( $src, $dest, $true ) | Out-Null
    }
    if ( [System.IO.File]::Exists( $dest ) -eq $true ) {
        if ( [string]::IsNullOrEmpty( $srcfiles ) -eq $false ) { $srcfiles += ", " }
        $srcfiles += $dest
    }
}
if ( [string]::IsNullOrWhiteSpace( $srcfiles ) -eq $true ) { return; }
if ( [System.IO.File]::Exists( $zipname ) -eq $true ) { [System.IO.File]::Delete( $zipname ) | Out-Null }
$srcpattern = [string]::Concat( $destination, "\*.*");
Compress-Archive -Path $srcpattern -DestinationPath $zipname
if ( [System.IO.File]::Exists( $installname ) -eq $true ) { [System.IO.File]::Delete( $installname ) | Out-Null }
[System.IO.File]::Copy( $zipname, $installname );