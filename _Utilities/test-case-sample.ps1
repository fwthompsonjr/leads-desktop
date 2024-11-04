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
"    }",
"}"
);
[string]$cmmd = [string]::Join( [System.Environment]::NewLine, $arr )
$cmmd = $cmmd.Replace( "~0", "TravisNavigateSearchTests" );
$cmmd