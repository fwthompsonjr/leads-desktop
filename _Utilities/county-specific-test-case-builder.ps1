<#
    county specific test case builder
#>
$dest = "Travis";
$src = "Dallas";
$pattern = "*$dest*.cs";
$srcdir = "C:\_d\lead-old\LegalLead.PublicData.Search\Util";
$targetdir = "C:\_d\lead-old\UnitTests\legallead.search.tests\util";
$di = [System.IO.DirectoryInfo]::new( $srcdir );
$found = $di.GetFiles($pattern);
$found | ForEach-Object {
    [System.IO.FileInfo]$info = $_;
    $name = [string]::Concat( [System.IO.Path]::GetFileNameWithoutExtension( $info.Name ), "Tests.cs");
    $templateName = [string]::Concat( [System.IO.Path]::GetFileNameWithoutExtension( $info.Name.Replace($dest, $src) ), "Tests.cs");
    $testFile = [System.IO.Path]::Combine( $targetdir, $name );
    $templateFile = [System.IO.Path]::Combine( $targetdir, $templateName );
    if ([System.IO.File]::Exists( $templateFile ) -eq $true -and [System.IO.File]::Exists( $testFile ) -eq $false ) {
        $content = [System.IO.File]::ReadAllText( $templateFile );
        $incidentId = $content.IndexOf( $src );
        while ($incidentId -ge 0 ) {
            $content = $content.Replace( $src, $dest );
            $incidentId = $content.IndexOf( $src );
        }
        [System.IO.File]::WriteAllText( $testFile, $content )
    }
}