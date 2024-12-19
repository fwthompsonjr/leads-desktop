<#
	git commit command
	commit and push code to remote
	parameters: 
		type := conventional commit type
		message := commit message

#>
param ( 
	[string]$type = "fix",
	[string]$message = "Code update for presentation layer")

$commitDefault = "fix"
$messageDefault = "Code update for presentation layer"
$commitTypes = @(
  'build',
  'chore',
  'ci',
  'docs',
  'feat',
  'fix',
  'perf',
  'refactor',
  'revert',
  'style',
  'test'
);
if ([string]::IsNullOrWhiteSpace($type) -eq $true) { $type = $commitDefault; }
if ([string]::IsNullOrWhiteSpace($message) -eq $true) { $message = $messageDefault; }

$mes = [string]::Concat( $type, ": ", $message);
git add .
git commit -m $mes
git push