param(
    [string]$pattern = "legal-lead-install*",
    [string]$key = "ASSET_SETUP_FILE"
)
try {
    Write-Host "Searching for files: $pattern"
    $scriptDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path);
    $di = New-Object System.IO.DirectoryInfo($scriptDir)
    $found = $di.GetFiles(("*.zip")) | Where-Object { $_.Name -match $pattern } | Sort-Object -Property Name -Descending
    $shortName = [System.IO.Path]::GetFileName( $found[0].FullName );
    $fileName = "./$shortName"
    
    Write-Host "Adding key: $key with value: $fileName"
    
    Add-Content -Path "$env:GITHUB_ENV" -Value "$key=$fileName"
} catch {
    Write-Host "Error: $_"    
    exit 1
}

