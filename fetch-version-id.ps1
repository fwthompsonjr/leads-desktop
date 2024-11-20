param(
    [string]$pattern = "legal-lead-install*",
    [string]$key = "BUILD_VERSION",
    [string]$index
)
try {
    Write-Host "Searching for files: $pattern"
    $replace = $pattern -replace '\*', ''
    $scriptDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path);
    $di = New-Object System.IO.DirectoryInfo($scriptDir)
    $found = $di.GetFiles(("*.zip")) | Where-Object { $_.Name -match $pattern } | Sort-Object -Property Name -Descending
    $shortName = [System.IO.Path]::GetFileName( $found[0].FullName );
    $shortName = $shortName -replace $replace, ''
    $shortName = $shortName.Replace(".zip", "")
    $shortName = $shortName.Replace("-", "")
    if ([string]::IsNullOrEmpty($index) -eq $false) {
        $shortName = "$shortName.$index";
    }
    
    Write-Host "Adding key: $key with value: $shortName"
    Add-Content -Path "$env:GITHUB_ENV" -Value "$key=$shortName"
    
} catch {
    Write-Host "Error: $_"    
    exit 1
}

