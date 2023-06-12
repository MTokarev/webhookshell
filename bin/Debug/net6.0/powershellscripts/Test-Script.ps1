param(
    [string]$Param1 = "DefaultValue1",
    [string]$Param2 = "DefaultValue1",
    [string]$CreateFileIn = ""
)

function Test-Script(){
    param(
        [string]$Param1,
        [string]$Param2,
        [string]$CreateFileIn
    )
    Write-Host "Param 1: '$Param1', Param 2 '$Param2'"
    if($CreateFileIn){
        New-Item -Path "$CreateFileIn\apidemo.tmp"
    }
}

Test-Script -Param1 $Param1 -Param2 $Param2 -CreateFileIn $CreateFileIn