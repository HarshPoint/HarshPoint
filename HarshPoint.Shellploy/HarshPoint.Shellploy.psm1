[Type]$T_ClientContext           = 'Microsoft.SharePoint.Client.ClientContext'
[Type]$T_HarshProvisioner        = 'HarshPoint.Provisioning.HarshProvisioner'
[Type]$T_HarshProvisionerBase    = 'HarshPoint.Provisioning.Implementation.HarshProvisionerBase'
[Type]$T_HarshProvisionerContext = 'HarshPoint.Provisioning.HarshProvisionerContext'
[Type]$T_IDefaultFromContextTag  = 'HarshPoint.Provisioning.IDefaultFromContextTag'
[Type]$T_Resolve                 = 'HarshPoint.Provisioning.Resolve'
[Type]$T_SPOCredentials          = 'Microsoft.SharePoint.Client.SharePointOnlineCredentials'

function New-HarshProvisioner {

    param (
        [String]
        $Name,

        [ScriptBlock]
        $Children,

        [Hashtable]
        $Property
    )

    $Result = New-Object "HarshPoint.Provisioning.$Name" -Property $Property

    if ($Children) {
        & $Children |% { 
            if ($_ -is $T_HarshProvisionerBase) {
                $Result.Children.Add($_) 
            }
            elseif ($_ -is $T_IDefaultFromContextTag) {
                $Result.ModifyChildrenContextState($_)
            }
            else {
                throw "Cannot add the following object to a provisioner.`n" +
                      "Only other provisioners or IDefaultFromContextTag objects can be added.`n" +
                      "$_"
            }
        }
    }

    $Result
}

function New-DefaultFromContextTag {

    param (
        [String]
        $Name,

        [Object]
        $Value
    )

    New-Object "HarshPoint.Provisioning.$Name" -Property @{ Value = $Value }
}

function DefaultContentTypeGroup($Value) {
    New-DefaultFromContextTag DefaultContentTypeGroup $Value
}

function DefaultFieldGroup($Value) {
    New-DefaultFromContextTag DefaultFieldGroup $Value
}

function ContentType {

    param (
        [HarshPoint.HarshContentTypeId]
        $Id,

        [String]
        $Name,

        [String]
        $Group,
         
        [ScriptBlock]
        $Children
    )

    New-HarshProvisioner HarshContentType $Children @{
        Id    = $Id
        Name  = $Name
        Group = $Group
    }
}

function TaxonomyField {

    param (
        [Guid]
        $Id,

        [String]
        $InternalName,

        [String]
        $DisplayName,

        [Guid]
        $TermSetId,

        [Switch]
        $AllowMultipleValues
    )

    Field -Id           $Id `
          -InternalName $InternalName `
          -DisplayName  $DisplayName `
          -Type         TaxonomyFieldType `
          -Children {
        
        New-HarshProvisioner HarshTaxonomyField $null @{

            AllowMultipleValues = $AllowMultipleValues.IsPresent
            
            TermSet = [HarshPoint.Provisioning.ResolveTermStoreExtensions]::TermSetById(
                $T_Resolve::TermStoreSiteCollectionDefault(),
                $TermSetId
            )
        }
    }
}

function Field {

    param (
        [Guid]
        $Id,

        [String]
        $InternalName,

        [String]
        $DisplayName,

        [String]
        $Type,
         
        [ScriptBlock]
        $Children
    )

    New-HarshProvisioner HarshField $Children @{
        DisplayName   = $DisplayName
        Id            = $Id
        InternalName  = $InternalName
        TypeName      = $Type
    }
}

function FieldRef {

    param (
        [String]
        $InternalName
    )

    New-HarshProvisioner HarshFieldRef $null @{
        Fields = $T_Resolve::FieldByInternalName($InternalName)
    }
}

function List {

    [CmdletBinding()]
    param (

        [Parameter(Position=0, Mandatory)]
        [String]
        $Title,

        [Parameter(Position=1, Mandatory)]
        [String]
        $Url,

        [Parameter(Position=2)]
        [ScriptBlock]
        $Children,

        [Parameter()]
        [Microsoft.SharePoint.Client.ListTemplateType]
        $TemplateType = 'GenericList'
    )

    New-HarshProvisioner HarshList $Children @{
        TemplateType = $TemplateType
        Title        = $Title
        Url          = $Url
    }
}


function ContentTypeRef {

    [CmdletBinding()]
    param (

        [Parameter(Position=0, Mandatory)]
        [HarshPoint.HarshContentTypeId[]]
        $ContentTypeId
    )

    New-HarshProvisioner HarshContentTypeRef $null @{
        ContentTypes = $T_Resolve::ContentTypeById($ContentTypeId)
    }
}

function RemoveContentTypeRef {

    [CmdletBinding()]
    param (
        [Parameter(Position=0, Mandatory)]
        [HarshPoint.HarshContentTypeId[]]
        $ContentTypeId
    )

    New-HarshProvisioner HarshRemoveContentTypeRef $null @{
        ContentTypes = $T_Resolve::ContentTypeById($ContentTypeId)
    }
}

function Invoke-WithProvisionerContext {
    [CmdletBinding()]
    param (

        [Parameter(Position = 0)]
        [Uri]
        $Url,

        [Parameter(Position = 1)]
        [ScriptBlock]
        $ScriptBlock,

        [Parameter()]
        [String]
        $UserName,
        
        [Parameter()]
        [String]
        $Password
    )

    try {
        $Credentials = New-Object $T_SPOCredentials $UserName, $Password
     
        $ClientContext = New-Object $T_ClientContext $Url
        $ClientContext.Credentials = $Credentials
        
        $Context = New-Object $T_HarshProvisionerContext $ClientContext

        & $ScriptBlock $Context
    }
    finally {
        $ClientContext.Dispose()
    }
}

function Provision {

    [CmdletBinding()]
    param (

        [Parameter(Position = 0)]
        [Uri]
        $Url,

        [Parameter(Position = 1)]
        [ScriptBlock]
        $Children,

        [Parameter()]
        [String]
        $UserName,
        
        [Parameter()]
        [String]
        $Password
    )
    
    Invoke-WithProvisionerContext -Url      $Url `
                                  -UserName $UserName `
                                  -Password $Password `
                                  -ScriptBlock {

        param ($Context)

        try {
            $Provisioner = New-HarshProvisioner HarshProvisioner $Children
            $Provisioner.ProvisionAsync($Context).Wait()
        }
        catch {
            if ($_.Exception.InnerException -is [AggregateException]) {
                $_.Exception.InnerException.InnerExceptions |% {
                    Write-Error $_
                }
            }
            else {
                Write-Error $_
            }
        }
    }
}