
$null = [reflection.assembly]::LoadFrom( 'D:\src\harshpoint\HarshPoint\bin\Debug\HarshPoint.dll' )

function New-Provisioner {

    param (
        [String]
        $Name,

        [Hashtable]
        $Property
    )

    New-Object "HarshPoint.Provisioning.$Name" -Property $Property
}

function Field {

    param (
        [Guid]
        $Id,

        [String]
        $InternalName,

        [String]
        $Type
    )

    New-Provisioner HarshFieldSchemaXmlProvisioner @{
        FieldId       = $Id
        FieldTypeName = $Type
        InternalName  = $InternalName
    }
}