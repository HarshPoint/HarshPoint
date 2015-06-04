[Type]$T_ClientContext           = 'Microsoft.SharePoint.Client.ClientContext'
[Type]$T_HarshProvisioner        = 'HarshPoint.Provisioning.HarshProvisioner'
[Type]$T_HarshProvisionerBase    = 'HarshPoint.Provisioning.Implementation.HarshProvisionerBase'
[Type]$T_HarshProvisionerContext = 'HarshPoint.Provisioning.HarshProvisionerContext'
[Type]$T_IDefaultFromContextTag  = 'HarshPoint.Provisioning.IDefaultFromContextTag'
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

    New-HarshProvisioner HarshFieldSchemaXmlProvisioner $Children @{
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
        Fields = [HarshPoint.Provisioning.Resolve]::FieldByInternalName($InternalName)
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

    try {
        $Credentials = New-Object $T_SPOCredentials $UserName, $Password
     
        $ClientContext = New-Object $T_ClientContext $Url
        $ClientContext.Credentials = $Credentials
        
        $Context = New-Object $T_HarshProvisionerContext $ClientContext


        $Provisioner = New-HarshProvisioner HarshProvisioner $Children
        $Provisioner.ProvisionAsync($Context).Wait()
    }
    finally {
        $ClientContext.Dispose()
    }
}