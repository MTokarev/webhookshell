param(
    [string]$Param1,
    [string]$Param2,
    [string]$CreateFileIn=""
)

function Test-Script(){
    param(
        [string]$Param1,
        [string]$Param2,
        [string]$CreateFileIn
    )
    Write-Output $Param1 $Param2
    if($CreateFileIn){
        New-Item -Path "$CreateFileIn\apidemo.tmp"
    }
}

Test-Script -Param1 $Param1 -Param2 $Param2 -CreateFileIn $CreateFileIn