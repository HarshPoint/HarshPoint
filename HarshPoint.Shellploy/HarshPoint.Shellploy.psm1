
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
        & $Children |% { $Result.Children.Add($_) }
    }

    $Result
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
        [String]
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

    $Context = [HarshPoint.Provisioning.HarshProvisionerContext]::OpenSharePointOnline(
        $Url, $UserName, $Password
    )


    $Provisioner = New-HarshProvisioner HarshProvisioner $Children
    $Provisioner.ProvisionAsync($Context).Wait()
}