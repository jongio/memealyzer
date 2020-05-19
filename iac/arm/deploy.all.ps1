param([string]$baseName, [string]$location, [string]$localUserName)

$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$rgName = "$($baseName)rg"

New-AzSubscriptionDeployment -Name "azsdkdemorgdeployment" -TemplateFile "$scriptDir\azuredeploy.all.json" -Location $location -baseName $baseName  &&
.\localUserRoleAssignment.ps1 $rgName $localUserName

Write-Host "Done"
