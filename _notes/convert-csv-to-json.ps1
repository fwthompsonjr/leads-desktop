<#
    court address, csv to json converter
#>


function getCountyCourtIndex( $courtName, $cidx )
{
    try {
        $itm = $courtName -replace "[^0-9]" , ''
        return [int]::Parse( $itm )
    } catch {
        return $cidx;
    }
}


$dir = "C:\_d\lead-old\_notes"
$src = "fortbend-court-address.csv"
$dest = "fortbend_court_address.json"
$srcfile = "$dir\$src"
$destfile = "$dir\$dest"

if ((Test-Path $srcfile) -eq $false) {
    Write-Host "VALIDATION: Input file $src is not found." -ForegroundColor DarkYellow
    return;
}
$pipe ='|';
$brk = [Environment]::NewLine;
$val = @();
$content = [System.IO.File]::ReadAllText( $srcfile );
$arrsource = $content.Split( $brk, [System.StringSplitOptions]::RemoveEmptyEntries );
$mx = $arrsource.Length;
for( $i = 1; $i -lt $mx; $i++){
    $dta = $arrsource.Item($i);
    $arr = $dta.Split($pipe);
    $obj = @{ 
        court = $arr[1].Trim()
        address = $arr[2].Trim()
        courtType = $arr[3].Trim().ToLower()
        }
    $val += ($obj);
}
if ($val.Length -le 0) {
    Write-Host "ERROR: Input file $src contains zero elements!" -ForegroundColor DarkRed
    return;
}
$jso = @(
    @{
        name = 'county'
        items = @();
    },
    @{
        name = 'district'
        items = @();
    },
    @{
        name = 'justice'
        items = @();
    },
    @{
        name = 'criminal'
        items = @();
    }
);
$cc_count = 1;
$dc_count = 1;
$jc_count = 1;
foreach( $js in $jso )
{
    foreach( $v in $val )
    {
        if ($v.courtType -ne $js.name ) { continue; }
        switch ($js.name)
        {
            'county' {
                $cc = @{
                    id = getCountyCourtIndex -courtName $v.court -cidx $cc_count
                    name = $v.court
                    address = [string]::Join( "|", @( $v.court, $v.address ) )
                }
                $js.items += ($cc | ConvertTo-Json | ConvertFrom-Json );
                $cc_count++
            }
            'district' {
                $cc = @{
                    id = $dc_count++
                    name = $v.court
                    address = [string]::Join( "|", @( $v.court, $v.address ) )
                }
                $js.items += ($cc | ConvertTo-Json | ConvertFrom-Json);
            }
            'justice' {
                $cc = @{
                    id = $jc_count++
                    name = $v.court
                    address = [string]::Join( "|", @( $v.court, $v.address ) )
                }
                $js.items += ($cc | ConvertTo-Json | ConvertFrom-Json);
            }
        }
    }
}
$jstring = $jso | ConvertTo-Json
$jstring = $jstring.Replace( '"@{name=', '{ "name": "');
$jstring = $jstring.Replace( '; id=', '", "id": ');
$jstring = $jstring.Replace( '; address=', ', "address": [ "');
$jstring = $jstring.Replace( '}"', '"] }');
$jstring = $jstring.Replace( '|', '", "');
$jstring
if (Test-Path $destfile) { [System.IO.File]::Delete( $destfile ); }
[System.IO.File]::WriteAllText( $destfile, $jstring );