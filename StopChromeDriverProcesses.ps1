$processName = 'chromedriver';
[System.Diagnostics.Process]::GetProcessesByName( $processName ).GetEnumerator() | foreach {
    $process = [System.Diagnostics.Process]($_);
    $process.Kill();
}
