
$testFolder = "C:\Users\frank.thompson\Downloads\dotnet-results-6.0.x"
$reportValues = @{
    totalpassed = 0
    totalfailed = 0
    totalskipped = 0
    totaloverall = 0
}
$assembyName = '(unknown)';
$reportMd = @( '#### Test Summary  ',
' ',
" * <span style='color: green'>&#x2714; Passed</span> - [totalpassed]",
" * <span style='color: red'>&#x2718; Failed</span> - [totalfailed]",
" * <span style='color: silver'>&#x2745; Skipped</span> - [totalskipped]",
" * &#x2211; Total :  - [totaloverall]");
$icons =@{
"Passed" = '<span style="color: green">&#x2714; Passed</span>'
"Failed" = '<span style="color: red">&#x2718; Failed</span>'
"Inconclusive" = '<span style="color: silver">&#x2745; Unknown</span>'
"NotExecuted" = '<span style="color: silver">&#x2745; Unknown</span>'
}

if ( [System.IO.Directory]::Exists( $testFolder ) -eq $false ) { return; }

function updateTotal( $node ) {
    [System.Xml.XmlNode]$counter = ([System.Xml.XmlNode]$node).GetElementsByTagName('Counters')[0]
    $total = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('total').Value );
    $passed = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('passed').Value );
    $failed = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('failed').Value );
    $others = $total - ($passed + $failed)
    $reportValues["totaloverall"] += $total
    $reportValues["totalpassed"] += $passed
    $reportValues["totalfailed"] += $failed
    $reportValues["totalskipped"] += $others
}

function findTestById( $node, $name ) {
    $ndlist = ([System.Xml.XmlNode]$node).ChildNodes;
    return $ndlist.GetEnumerator().Where({ ([system.xml.xmlnode]$_).Attributes.GetNamedItem( 'id' ) -eq $name });
}

function findNodeInList( $node, $name ) {
    $ndlist = ([System.Xml.XmlNode]$node).ChildNodes;
    return $ndlist.GetEnumerator().Where({ $_.Name -eq $name });
}

function getClassName( $classid, $testlist ) {
    foreach( $unitTest in $testlist.ChildNodes ) {
        $tst = ([system.xml.xmlnode]$unitTest)
        $attr = $tst.Attributes.GetNamedItem( 'id' ).Value
        if( $attr -eq $classid ) {
            [system.xml.xmlnode]$clsnode = $tst.GetElementsByTagName("TestMethod")[0];
            if( $null -eq $clsnode ) { return $find; }
            $clsName = $clsnode.Attributes.GetNamedItem('className').Value.Split( '.' );
            return $clsName[$clsName.Length-1];
        }
    }
    return $find;
}

function getSectionSummary( $node ) {
    $template = "<summary>$assembyName Passed: [passed], Failed: [failed], Other: [other]</summary>  ";
    [System.Xml.XmlNode]$counter = ([System.Xml.XmlNode]$node).GetElementsByTagName('Counters')[0]
    $total = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('total').Value );
    $passed = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('passed').Value );
    $failed = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('failed').Value );
    $others = $total - ($passed + $failed);
    $template = $template.Replace( '[passed]', $passed)
    $template = $template.Replace( '[failed]', $failed)
    $template = $template.Replace( '[other]', $others)
    return $template;
}

function getOutCome( $code ) {
    if( $icons.ContainsKey( $code ) -eq $true ) { return $icons[$code]; }
    return $code;
}

function timeSpanToSeconds( $timestring ) {
    [System.TimeSpan]$ts = [System.TimeSpan]::Parse( $timestring );
    return $ts.TotalSeconds.ToString("0.000");
}

function buildResults( $node, $testlist, $numbers ) {
    $ssline = getSectionSummary -node $numbers
    $header = '| Result | Class Name | Test Name | Duration (seconds) |  '
    $subheader = '| :-- | :-- | :-- | --: |  '
    $line = '| [outcome] | [class] | [testName] | [duration] |  '
    $arr = @();
    $arr += '<details>  '
    $arr += $ssline
    $arr += '  '
    $arr += "###### Test Results - $assembyName "
    $arr += '  '
    $arr += '<small>  '
    $arr += '  '
    $arr += $header
    $arr += $subheader
    $tmptable = @{};
    $indexes = @();
    foreach($nde in $node.ChildNodes) {
        $nn = [system.xml.xmlnode]$nde
        $testid = $nn.Attributes.GetNamedItem( 'testId' ).Value
        $outcomeid = $nn.Attributes.GetNamedItem( 'outcome' ).Value
        $outcome = getOutCome -code $outcomeid
        $classNamed = getClassName -classid $testid -testlist $testlist
        $duration = timeSpanToSeconds -timestring $nn.Attributes.GetNamedItem( 'duration' ).Value
        $tmp = @{ 
            outcome = $outcome
            className = $classNamed.Trim()
            testName = $nn.Attributes.GetNamedItem( 'testName' ).Value.Trim()
            duration = $duration
        };

        $indexes+= ( "$($tmp["className"]) $($tmp["testName"]) | $($tmptable.Count)" )
        $tmptable.Add( $tmptable.Count, $tmp );
    }
    $indexes = ($indexes | sort);
    
    $indexes.GetEnumerator() | ForEach-Object { 
        $s =   [System.Convert]::ToInt32( ([string]$_).Split('|')[1] );
        $tmp = $tmptable[$s];
        $addline = $line.Replace('[outcome]', $tmp['outcome'] );
        $addline = $addline.Replace('[testName]', $tmp['testName'] );
        $addline = $addline.Replace('[duration]', $tmp['duration'] );
        $addline = $addline.Replace('[class]', $tmp['className'] );
        $arr += $addline
        }
    
    $arr += '  '
    $arr += '</small>  '
    $arr += '</details>  '
    $assembyName = '(unknown)';
    return ( [string]::Join( [environment]::NewLine, $arr ) );
}

$folder = [System.IO.DirectoryInfo]::new( $testFolder );
$folder.GetFiles( "*.trx" ).GetEnumerator() | ForEach-Object {
    
    $assembyName = '(unknown)';
    $testfile = ([System.IO.FileInfo]$_).FullName
    $x = [xml](Get-Content $testfile)
    $doc = $x.DocumentElement;
    $nc = $doc.ChildNodes.Count;
    for($n = 0; $n -lt $nc; $n++ ) {
        if( $doc.ChildNodes[$n].Name -eq 'Results' ) { $results = $doc.ChildNodes[$n] }
        if( $doc.ChildNodes[$n].Name -eq 'TestDefinitions' ) { $definitions = $doc.ChildNodes[$n] }
        if( $doc.ChildNodes[$n].Name -eq 'ResultSummary' ) { $summary = $doc.ChildNodes[$n] }
    }

    $definitionNodes = $definitions.ChildNodes[0].Attributes.GetNamedItem( 'storage' ).Value.Split( '\' );
    $assembyName = $assembyName.Replace( 'unknown', $definitionNodes[ $definitionNodes.Count - 1 ] );

    $sbset = buildResults -node $results -testlist $definitions -numbers $summary
    $reportMd += $sbset
    updateTotal -node $summary
    $assembyName = '(unknown)';
}
$reportMd = $reportMd.Replace( 'totalpassed', $reportValues['totalpassed'] )
$reportMd = $reportMd.Replace( 'totalfailed', $reportValues['totalfailed'] )
$reportMd = $reportMd.Replace( 'totalskipped', $reportValues['totalskipped'] )
$reportMd = $reportMd.Replace( 'totaloverall', $reportValues['totaloverall'] )

$reportMd