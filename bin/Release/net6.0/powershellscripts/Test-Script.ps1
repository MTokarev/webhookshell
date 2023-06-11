param(
    [string]$Param1
)

Write-Output (get-childitem -Directory $Param1).Name