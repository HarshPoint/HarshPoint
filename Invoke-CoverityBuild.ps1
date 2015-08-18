Add-Type -AssemblyName System.Net.Http

function New-StringContent($str) {
	New-Object Net.Http.StringContent $str
}

$MSBuild     = "${Env:ProgramFiles(x86)}\MSBuild\14.0\bin\msbuild.exe"
$MSBuildArgs = @(
  "/m"
  "/l:${Env:ProgramFiles}\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
  "/p:Configuration=${Env:CONFIGURATION}"
  "/p:Platform=${Env:PLATFORM}"
)

if (($Env:APPVEYOR_SCHEDULED_BUILD -ne 'True') -and
	($Env:APPVEYOR_FORCED_BUILD    -ne 'True')) {
	& $MSBuild $MSBuildArgs
	exit $LastExitCode
}

$CoverityDir = "cov-int"
$CoverityZip = "${Env:APPVEYOR_PROJECT_NAME}.zip"

Write-Host "Building project with Coverity Scan..."
cov-build --dir $ConverityDir  $MSBuild $MSBuildArgs
if (-not $?) { exit $LastExitCode }

Write-Host "Compressing Coverity results for upload..."
7z a $CoverityZip "$CoverityDir\*"
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