$config   = 'Debug'
$projects = @('HarshPoint.Tests', 'HarshPoint.Server.Tests')
$runner   = @(Resolve-Path $PSScriptRoot\..\packages\xunit.runner.console.*.*.*\tools\xunit.console.exe)[0]

& $runner $($projects |% { "$PSScriptRoot\$_\bin\$config\$_.dll" }) -nologo @args