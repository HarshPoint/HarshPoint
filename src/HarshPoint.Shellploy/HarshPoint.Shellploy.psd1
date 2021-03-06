@{
	ModuleVersion = '%Version%'
	GUID          = '%Guid%'
	Author        = '%Author%'
	CompanyName   = '%CompanyName%'
	Copyright     = '%Copyright%'
	Description   = '%Description%'

	PrivateData = @{
		PSData = @{
			Tags           = '%Tags%'
			LicenseUri     = '%LicenseUri%'
			ProjectUri     = '%ProjectUri%'
			# IconUri        = ''
			# ReleaseNotes   = ''
		}
	}

	# load the assembly first so ps1xml files can reference it
	RequiredAssemblies = @('HarshPoint.Shellploy.dll')
	RootModule         = 'HarshPoint.Shellploy.psm1'
	NestedModules      = @('HarshPoint.Shellploy.dll')
	FormatsToProcess   = @('HarshPoint.Shellploy.format.ps1xml')

	DefaultCommandPrefix = ''
	FunctionsToExport    = '*'
	CmdletsToExport      = '*'
	VariablesToExport    = '*'
	AliasesToExport      = '*'
	PowerShellVersion    = '4.0'

# Minimum version of Microsoft .NET Framework required by this module
# DotNetFrameworkVersion = ''

# Minimum version of the common language runtime (CLR) required by this module
# CLRVersion = ''

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
# 



# HelpInfo URI of this module
# HelpInfoURI = ''

}

