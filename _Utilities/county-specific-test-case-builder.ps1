<#
    county specific test case builder
#>

function getBlankTestContent($csname){
    $arr = @(
    "using LegalLead.PublicData.Search.Classes;",
    "using LegalLead.PublicData.Search.Util;",
    "using Moq;",
    "using OpenQA.Selenium;",
    "using System;",
    "",
    "namespace legallead.search.tests.util",
    "{",
    "    public class ~0",
    "    {",
    "         [Fact]",
    "         public void ServiceHasTypeDefined()",
    "         {",
    "             var error = Record.Exception(() => _ = typeof(~1));",
    "             Assert.Null(error);",
    "         }",
    "    }",
    "}"
    );
    [string]$cmmd = [string]::Join( [System.Environment]::NewLine, $arr )
    $tpname = $csname.Replace( "Tests", "" );
    $cmmd = $cmmd.Replace( "~0", $csname );
    $cmmd = $cmmd.Replace( "~1", $tpname );
    return $cmmd;
}


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
    if ([System.IO.File]::Exists( $testFile ) -eq $false ) {
        $cname = [System.IO.Path]::GetFileNameWithoutExtension( $testFile )
        $content = (getBlankTestContent -csname $cname)
        [System.IO.File]::WriteAllText( $testFile, $content )
    }
}