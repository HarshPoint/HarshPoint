Add-Type -AssemblyName System.Net.Http

function New-StringContent($str) {
	New-Object Net.Http.StringContent $str
}

$Env:Path = "${Env:ProgramFiles(x86)}\MSBuild\14.0\bin\;${Env:Path}"

$MSBuildArgs = @(
  '/m'
  "/l:${Env:ProgramFiles}\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
)

if (${Env:CONFIGURATION}) {
	$MSBuildArgs += @("/p:Configuration=${Env:CONFIGURATION}")
}

if (${Env:PLATFORM}) {
	$MSBuildArgs += @("/p:Platform=${Env:PLATFORM}")
}  

if (($Env:APPVEYOR_SCHEDULED_BUILD -ne 'True') -and
	($Env:APPVEYOR_FORCED_BUILD    -ne 'True')) {
	msbuild $MSBuildArgs
	exit $LastExitCode
}

$CoverityZip = "${Env:APPVEYOR_PROJECT_NAME}.zip"

Write-Host "Building project with Coverity Scan..."
cov-build --dir cov-int msbuild  $MSBuildArgs
if (-not $?) { exit $LastExitCode }

Write-Host "Compressing Coverity results for upload..."
7z a $CoverityZip cov-int\*
if (-not $?) { exit $LastExitCode }

Write-Host "Uploading Coverity results..."

$fields = @{
	token       = New-StringContent $Env:CoverityProjectToken
	email       = New-StringContent $Env:CoverityNotificationEmail
	version     = New-StringContent $Env:APPVEYOR_BUILD_VERSION
	description = New-StringContent 'AppVeyor scheduled build'
}

$form = New-Object Net.Http.MultipartFormDataContent

$fields.Keys |% {
	$form.Add($field[$_], "`"$_`"")
}

$path   = (Resolve-Path $CoverityZip).Path
$stream = [IO.File]::OpenRead($path)

$file = New-StreamContent $CoverityZip
$form.Add($file, '"file"', (Split-Path -Leaf $CoverityZip))

$client = New-Object Net.Http.HttpClient -Property @{
	Timeout = [TimeSpan]::FromMinutes(20)
}

$url = "https://scan.coverity.com/builds?project=${Env:APPVEYOR_REPO_NAME}"
$task = $client.PostAsync($url, $form)

try {
	$task.Wait()
} 
catch [AggregateException] {
	throw $_.Exception.InnerException
}

$task.Result
$stream.Close()