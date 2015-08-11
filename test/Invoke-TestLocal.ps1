& powershell.exe {     
    Remove-Item env:\HarshPointTestUrl
    & "$($args[0])\Invoke-Test.ps1" -notrait HarshPoint.Server=1 
} -args $PSScriptRoot 