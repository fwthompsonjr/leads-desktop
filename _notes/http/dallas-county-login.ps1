<#
    HTTP CLient Test
    1. Login
#>

# 2025-04-25T17%3a50%3a09
$currentPath = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path )
$binPath = [System.IO.Path]::Combine( $currentPath, "bin" )
$agilityFile = [System.IO.Path]::Combine( $binPath, "HtmlAgilityPack.dll" )

# Load the HtmlAgilityPack assembly
Add-Type -Path $agilityFile

function getFormAction( $html_content ) {
    $doc = New-Object HtmlAgilityPack.HtmlDocument
    $doc.LoadHtml( $html_content );
    $fnode = $doc.DocumentNode.SelectSingleNode("//form")
    if ($null -eq $fnode ) {
        return ""
    }
    return $fnode.GetAttributeValue( "action", "" );
}

function getFormVariables( $html_content ) {

    # Initialize an empty hash table
    $formData = @{}

    $doc = New-Object HtmlAgilityPack.HtmlDocument
    $doc.LoadHtml( $html_content );
    $frm = $doc.DocumentNode.SelectSingleNode("//form")
    if ($null -eq $frm ) {
        return $formData
    }
    
    # Iterate through all input elements within the form
    $frm.SelectNodes("//input").ForEach({
        $name = $_.GetAttributeValue("name", "")
        $value = $_.GetAttributeValue("value", "")
    
        # Add the name and value to the hash table if the name is not empty
        if ($name -ne "") {
            $formData[$name] = $value
        }
    })

    return $formData
}


function normalizeHtml($htmlInput, $uid, $pwd) {

    # Create a new HtmlDocument object
    $htmlDoc = New-Object HtmlAgilityPack.HtmlDocument

    # Load the HTML content into the HtmlDocument object
    $htmlDoc.LoadHtml($htmlInput)

    # Normalize the HTML
    $htmlDoc.OptionOutputAsXml = $true
    $nde = $htmlDoc.DocumentNode;
    $cbx = $nde.SelectSingleNode('//input[@id="TOSCheckBox"]')
    if ($null -ne $cbx ) {
        $cbx.SetAttributeValue("checked", "true");
        $cbx.SetAttributeValue("value", "on");
        ## $cbx.SetAttributeValue("name", "TOSCheckBox");
    }
    $items = @("//input[@id='UserName'];$uid",
    "//input[@id='Password'];$pwd");
    foreach($i in $items) {
        $xp = $i.Split(';')[0];
        $xval = $i.Split(';')[1];
        $tbx = $nde.SelectSingleNode($xp);
        if ($null -ne $tbx ) {
            $tbx.SetAttributeValue("value", $xval);
        }
    }
    # Convert the normalized HTML to XML
    return $nde.OuterHtml
    # $xml = [xml]$xmlContent
    # return $nde

}

$uprefix = "https://odysseyidentityprovider.tylerhost.net"
$nw =[DateTime]::UtcNow;
$d = $nw.ToString('yyyy-MM-ddTHH:mm:ss')
$encodedDate = [string]::Concat( [System.Uri]::EscapeDataString($d), "Z")
$qry = (
"https://odysseyidentityprovider.tylerhost.net/idp/account/signin?ReturnUrl=%2fidp%2fissue%2fwsfed%3fwa%3dwsignin1.0%26" +
"wtrealm%3dhttps%253a%252f%252fcourtsportal.dallascounty.org%252fDALLASPROD%252f%26" +
"wctx%3drm%253d0%2526id%253dpassive%2526ru%253d%25252fDALLASPROD%25252fAccount%25252fLogin%26" +
"wct%3d$encodedDate%26" +
"wauth%3durn%253a1&" +
"wa=wsignin1.0&" +
"wtrealm=https%3a%2f%2fcourtsportal.dallascounty.org%2fDALLASPROD%2f&" +
"wctx=rm%3d0%26id%3dpassive%26ru%3d%252fDALLASPROD%252fAccount%252fLogin&" +
"wct=$encodedDate&" +
"wauth=urn%3a1");


$destinations= @{
    login = $qry
}
$settings = @{
    email = "kerriphillipslaw@gmail.com"
    pwd = "M@ri1007"
}

$actual = $destinations.login
# Set-Clipboard $actual
$response = Invoke-WebRequest -Uri $actual
$htmlContent = $response.Content
# Load the HTML content into an XML object
[string]$html = [string](normalizeHtml -htmlInput $htmlContent -uid $settings.email -pwd $settings.pwd)


# Extract the form action URL
$form_action = getFormAction -html_content $html
$form_action = [string]::Concat( $uprefix, $form_action );
$formData = getFormVariables -html_content $html
$session = $null
try {
$response = (Invoke-WebRequest -Uri $form_action -Method Post -Body $formData -SessionVariable session -ErrorAction Ignore)
} catch {
 # no action at this time
}
$headers = $response.Headers["Set-Cookie"]
$headers.Split(";") | Write-Output
<#
if ($null -ne $session ) {
    foreach($c in $session.Cookies) {
        $dp = $c.GetCookies("https://odysseyidentityprovider.tylerhost.net/")
    }
}
#>