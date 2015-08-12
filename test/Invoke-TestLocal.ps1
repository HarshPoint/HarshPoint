try {
    $env:HarshPointTestUrl_ = $env:HarshPointTestUrl
    & "$PSScriptRoot\Invoke-Test.ps1" -notrait HarshPoint.Server=1 @args
}
finally {
    $env:HarshPointTestUrl = $env:HarshPointTestUrl_
}