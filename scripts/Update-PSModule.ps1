[CmdletBinding()]
param (
	[Parameter(Mandatory)]
	[String]
	$ProjectOutputPath,
	
	[Parameter(Mandatory)]
	[String]
	$ModuleRoot,

	[Parameter()]
	[String]
	$NuSpecTemplate = '..\nuspec.template'
)

function Get-AttributeValue($AttributeType, $Property) {
	if (-not $Property) {
		$Property = $AttributeType -replace '^Reflection\.Assembly',''
	}
	
	$Attr = @($Assembly.GetCustomAttributes(
		[Type]"${AttributeType}Attribute", 
		$false
	))[0]
	
	if ($Attr) {
		$Attr.$Property
	}
	else {
		[String]::Empty
	}
}

try {

	$ProjectOutputDir = (Split-Path -Parent $ProjectOutputPath)
	$ProjectOutputDir = (Resolve-Path $ProjectOutputDir).Path

	$AssemblyFileName = (Split-Path -Leaf   $ProjectOutputPath)

	New-Item -ItemType Directory $ModuleRoot -Force -ErrorAction Stop

	Get-ChildItem -LiteralPath $ProjectOutputDir -Filter '*.dll' `
	| Copy-Item -Destination $ModuleRoot -PassThru -ErrorAction Stop

	$ModuleRoot   = (Resolve-Path $ModuleRoot).Path
	$AssemblyPath = (Join-Path $ModuleRoot $AssemblyFileName)
	$Assembly     = [Reflection.Assembly]::LoadFrom($AssemblyPath)
	$Assembly | Format-Table

	$AssemblyName = $Assembly.GetName()
	$AssemblyInfo = @{
		Author        = Get-AttributeValue Reflection.AssemblyCompany
		CompanyName   = Get-AttributeValue Reflection.AssemblyCompany
		Copyright     = Get-AttributeValue Reflection.AssemblyCopyright
		Description   = Get-AttributeValue Reflection.AssemblyDescription
		Guid          = Get-AttributeValue Runtime.InteropServices.Guid Value
	}


	$NuSpecTemplate = (Join-Path $PSScriptRoot $NuSpecTemplate)
	$NuspecTemplate = (Resolve-Path $NuSpecTemplate).Path
	$NuSpec         = [xml](Get-Content $NuSpecTemplate)

	$ModulePath = (Join-Path $ModuleRoot "$($AssemblyName.Name).psd1")
	$NuSpecPath = (Join-Path $ModuleRoot "$($AssemblyName.Name).nuspec")

	if ($PSVersionTable.PSVersion.Major -ge 5) {
		$AssemblyInfo['LicenseUri']    = $NuSpec.package.metadata.licenseUrl
		$AssemblyInfo['ProjectUri']    = $NuSpec.package.metadata.projectUrl
		$AssemblyInfo['Tags']          = $NuSpec.package.metadata.tags
	}

	New-ModuleManifest -RootModule    $AssemblyFileName `
					   -Path          $ModulePath `
					   -ModuleVersion $AssemblyName.Version `
					   @AssemblyInfo

	$NuSpec.package.metadata.id          = $AssemblyName.Name
	$NuSpec.package.metadata.title       = $AssemblyName.Name
	$NuSpec.package.metadata.version     = (Get-AttributeValue Reflection.AssemblyInformationalVersion)
	$NuSpec.package.metadata.authors     = $AssemblyInfo['Author']
	$NuSpec.package.metadata.owners      = $AssemblyInfo['Author']
	$NuSpec.package.metadata.description = $AssemblyInfo['Description']
	$NuSpec.package.metadata.copyright   = $AssemblyInfo['Copyright']

	$NuSpec.Save($NuSpecPath)

	$NuGet = (Resolve-Path "$PSScriptRoot\..\tools\nuget.exe").Path
	& $NuGet pack $NuSpecPath -OutputDir (Split-Path -Parent $ModuleRoot) `
							  -NoPackageAnalysis
}
catch {
	Write-Error $_
	exit 1
}