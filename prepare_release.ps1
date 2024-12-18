<#
	Prepare Release Script
	Steps:
	1. Get Release Number
		a. Prompt user for release type (major, minor, revision)
		b. Generate release number and echo back to screen
#>
param(
	[bool]$includeProjectUpdates = $true,
	[bool]$updateReleaseNotes = $true)
# Get the path of the current script
$nl = [System.Environment]::NewLine;
$delimiter = "~"
$scriptFile = $MyInvocation.MyCommand.Path
$scriptDir = [System.IO.Path]::GetDirectoryName($scriptFile)

$projectName = "LegalLead.PublicData.Search";
$changeLog = "CHANGELOG"
$versionLog = "setup-version"
$launchExe = "launch-exe"
$uninstallExe = "uninstall"
$setupProject = "setup"
$installZip = "installation"
$solution = "Thompson.RecordSearch.sln"

$projectFile = [System.IO.Path]::Combine("$scriptDir\$projectName", "$projectName.csproj");
$changeLogFile = [System.IO.Path]::Combine("$scriptDir", "$changeLog.md");
$versionLogFile = [System.IO.Path]::Combine("$scriptDir\setup", "$versionLog.txt");
$launchExeFile = [System.IO.Path]::Combine("$scriptDir\Shared", "$launchExe.vbs");
$uninstallExeFile = [System.IO.Path]::Combine("$scriptDir\Shared", "$uninstallExe.vbs");
$setupProjectFile = [System.IO.Path]::Combine("$scriptDir\setup", "$setupProject.vdproj");
$installZipFile = [System.IO.Path]::Combine("$scriptDir", "$installZip.zip");
$rollbackScript = [System.IO.Path]::Combine("$scriptDir", "git_rollback.ps1");
$commitScript = [System.IO.Path]::Combine("$scriptDir", "git_commit.ps1");
$solutionFile = [System.IO.Path]::Combine("$scriptDir", $solution);

# function(s)
function promptForList($typeName)
{
	$stp = "stop";
	$promptText = "Enter value for $typeName. (stop) when complete: "
	$arr = @();
	$lne = Read-Host -Prompt $promptText
	while ($lne -ne $stp) {
		$arr += $lne
		$lne = Read-Host -Prompt $promptText
	}
	return [string]::Join( $nl, $arr );
}
function getChangeLogText($tag){
	$title =  Read-Host -Prompt "Enter release title: "
	$changes = (promptForList -typeName "Problem Statement")
	$checks = (promptForList -typeName "Component Checks");
	$chgs = @(
		[string]::Concat("Release | $tag - $title", $nl)
		[string]::Concat("Problem Statement:", $nl, $changes, $nl)
		[string]::Concat("Component Checks:", $nl, $checks)
	);
	return [string]::Join( $delimiter, $chgs );
}
function generateMarkDown($block){
	$blck = [string]$block;
	$parts = $blck.Split($delimiter);
	$parts[0] = [string]::Concat( "# ", $parts[0].Trim() )
	$parts[1] = [string]::Concat( "## ", $parts[1].Trim() )
	$parts[2] = [string]::Concat( "### ", $parts[2].Trim() )
	
	return [string]::Join( $nl, $parts );
}

function generateChangeJson($block, $tag){
	$blck = [string]$block;
	$parts = $blck.Split($nl);
	$vrsn = $tag.Split('.');
	$vindex = [string]::Join( ".", @( $vrsn[0], $vrsn[1], $vrsn[2] ));
	$dte = (Get-Date -f "s").Replace("T", " ");
	$obj = @{
		"id" = $vindex
		"date" = $dte
		"description" = $parts
	}
	return $obj
}

function doesFileExist($item) {
	$key = $item.name
	$path = $item.value
	$shortName = [System.IO.Path]::GetFileNameWithoutExtension( $path )
	$exists = [System.IO.File]::Exists( $path );
	if ($exists -eq $false) {		
		Write-Warning "$key file ( $shortName ) is not found."
	}
	return $exists;
}

function getProjectList($directory)
{
	$exists = [System.IO.Directory]::Exists( $directory );
	if ($exists -eq $false) { return $null; }
	$files = [System.IO.DirectoryInfo]::new( $directory ).GetFiles( "*.csproj", [System.IO.SearchOption]::AllDirectories);
	$files = $files | Where-Object { 
		$nm = $_.Name;
		return $nm.IndexOf( "test", [System.StringComparison]::OrdinalIgnoreCase ) -lt 0
	}
	$files = $files | Where-Object { 
		$nm = $_.Name;
		return $nm.IndexOf( "backup", [System.StringComparison]::OrdinalIgnoreCase ) -lt 0
	}
	$files = $files | Where-Object { 
		$nm = $_.Name;
		return $nm.IndexOf( "permissions.api", [System.StringComparison]::OrdinalIgnoreCase ) -lt 0
	}
	return $files;
}

function getProjectVersion($project)
{
	$exists = [System.IO.File]::Exists( $project );
	if ($exists -eq $false) { return $null; }
	[xml]$content = [System.IO.File]::ReadAllText( $project );
	[System.Xml.XmlNode]$node = $content.DocumentElement.SelectSingleNode("//FileVersion");
	if ($null -eq $node) { return $null }
	return $node.InnerText;
}

function getNextProjectVersion($current){
	$alist = "major,minor,patch,build";
	$changeNames = $alist.Split(',')
	$changeName = Read-Host -Prompt "Enter revision type ($alist)"
	while ($changeNames.Contains($changeName) -eq $false) {
		$changeName = Read-Host -Prompt "Invalid type please choose ($alist): "
	}
	
	$aversion = ([string]$current).Split(".");
	for($c = 0; $c -lt $changeNames.Count; $c++) {
		$change = $changeNames[$c]
		if ($change -eq $changeName.ToLower()) {
			$inumber = [System.Convert]::ToInt32($aversion[$c]) + 1;
			$aversion[$c] = [System.Convert]::ToString( $inumber );
			if ($c -lt 1) { $aversion[1] = '0' }
			if ($c -lt 2) { $aversion[2] = '0' }
			if ($c -lt 3) { $aversion[3] = '0' }
			break;
		}
	}
	$nwversion = [string]::Join(".", $aversion);
	return @{
		change = $changeName
		tag = $nwversion
	};
}

function setVersionNumber($project, $tag) {
	
	$exists = [System.IO.File]::Exists( $project );
	if ($exists -eq $false) { return $null; }
	$searches = @( "//FileVersion", "//AssemblyVersion")
	[xml]$content = [System.IO.File]::ReadAllText( $project );
	[System.Xml.XmlNode]$element = $content.DocumentElement;
	$conversionComplete = $true;
	foreach($xpath in $searches) {
		[System.Xml.XmlNode]$node = $element.SelectSingleNode($xpath);
		if ($null -eq $node ) { $conversionComplete = $false; break; }
		$node.InnerText = $tag;
	}
	if ($conversionComplete -eq $true) {
		$content.Save( $project );
		dotnet build $project -c Release
	}
	return $conversionComplete;
}
function buildSolution(){
	$proj = $projectFile
	$sln = "<full path to solution file>"
	devenv.com $sln /build "Release" /project $proj
}
function rollbackChanges()
{
	& $rollbackScript		
}

function commitChanges($text)
{
	& $commitScript -type 'fix' -message $text		
}
$expected = @(
	@{ 
		name = "project"
		value = "$projectFile"
	},
	@{ 
		name = "changeLog"
		value = $changeLogFile
	},
	@{ 
		name = "versionLog"
		value = $versionLogFile
	},
	@{ 
		name = "launch utility"
		value = $launchExeFile
	},
	@{ 
		name = "uninstall script"
		value =$uninstallExeFile
	},
	@{ 
		name = "setup project"
		value = $setupProjectFile
	},
	@{ 
		name ="installation package"
		value = $installZipFile
	},
	@{ 
		name ="solution file"
		value = $solutionFile
	}
);
$canExecute = $true;

## Section: Input Valiadtion
Write-Host "Begining input validations:"
foreach($expectation in $expected) {
	$isfound = doesFileExist -item $expectation
	if ($isfound -eq $false) {
		$canExecute = $false;
		break
	} else {
		Write-Host " . $($expectation.name) verified" -ForegroundColor Green
	}
}
if ($canExecute -eq $false) {
	Write-Host "One or more expected files are missing." -ForegroundColor Red
	return;
}
Write-Host "Completed input validations:"

## Section: Get Release Version Numbers
Write-Host "Begining release creation process:"
$project_list = (getProjectList -directory $scriptDir)
$project_version = (getProjectVersion -project $projectFile)
$tagNumber = $project_version;

## Section: Update *.csproj files and get new tag
if ($includeProjectUpdates -eq $true)
{
	
if ($null -eq $project_list) {
	throw "Unable to find project list in target directory"
}
Write-Host " . Found ( $($project_list.Count) project files to convert ):" -ForegroundColor Green
if ($null -eq $project_version) {
	throw "Unable to find project version number"
}
Write-Host " .. Current version $project_version"
$revisionType = getNextProjectVersion -current $project_version
$tagNumber = $revisionType.tag;
Write-Host " .. $($revisionType.change) $($revisionType.tag)"

$project_converted_count = 0;
$project_list | ForEach-Object {
	$target_file = $_.FullName;
	Write-Host " .. setting tag for $($_.Name)"
	$converted = (setVersionNumber -project $target_file -tag $tagNumber);
	if ($converted -eq $true) { $project_converted_count++ }
}
if ($project_converted_count -ne $project_list.Count) {
	rollbackChanges
	throw "failure updating project files."
}
else {
	Write-Host "Saving project number updates."
	## commitChanges -text "$tagNumber : updating project to prepare release"
}
}

## Section: Get Change text
Write-Host "Generating release notes."
$changetext = getChangeLogText -tag $tagNumber
$markdown = generateMarkDown -block $changetext
$jsonitem = generateChangeJson -block $changetext -tag $tagNumber
$jsonitem | ConvertTo-Json