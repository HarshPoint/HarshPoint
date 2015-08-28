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

	$AssemblyFileName = (Split-Path -Leaf $ProjectOutputPath)

	if (Test-Path -LiteralPath $ModuleRoot) {
		Remove-Item -LiteralPath $ModuleRoot `
					-ErrorAction Stop `
					-Recurse `
					-Force
	}

	New-Item -Path        $ModuleRoot `
	         -ItemType    Directory `
			 -ErrorAction Stop `
			 -Force 

	Get-ChildItem -LiteralPath $ProjectOutputDir `
				  -Include     '*.dll', '*.ps1xml', '*.ps[dm]1' `
			      -Recurse `
	| Copy-Item -Destination $ModuleRoot `
	            -ErrorAction Stop `
	            -PassThru 

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
		Version       = Get-AttributeValue Reflection.AssemblyInformationalVersion
		Guid          = Get-AttributeValue Runtime.InteropServices.Guid Value
	}

	$NuSpecTemplate = (Join-Path $PSScriptRoot $NuSpecTemplate)
	$NuspecTemplate = (Resolve-Path $NuSpecTemplate).Path
	$NuSpec         = [xml](Get-Content $NuSpecTemplate)
	$NuSpecPath     = (Join-Path $ModuleRoot "$($AssemblyName.Name).nuspec")

	$AssemblyInfo['LicenseUri']    = $NuSpec.package.metadata.licenseUrl
	$AssemblyInfo['ProjectUri']    = $NuSpec.package.metadata.projectUrl
	$AssemblyInfo['Tags']          = $NuSpec.package.metadata.tags

	$ModulePath = (Join-Path   $ModuleRoot "$($AssemblyName.Name).psd1")
	$ModuleText = (Get-Content $ModulePath -Raw)

	$AssemblyInfo.Keys |% {
		$ModuleText = $ModuleText -replace "%${_}%", $AssemblyInfo[$_]
	}

	Set-Content -LiteralPath $ModulePath -Value $ModuleText -Encoding UTF8
	Write-Information "Updated $ModulePath"

	$NuSpec.package.metadata.id          = $AssemblyName.Name
	$NuSpec.package.metadata.title       = $AssemblyName.Name
	$NuSpec.package.metadata.version     = $AssemblyInfo['Version']
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