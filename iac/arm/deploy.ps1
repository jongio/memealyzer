param([string]$baseName, [string]$location, [string]$azureUsername)

$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

$rg = New-AzResourceGroup -Name "$($baseName)rg" -Location $location -Force

Write-Host $rg.ResourceGroupName

New-AzResourceGroupDeployment -Name "azsdkdemodeployment" -ResourceGroupName $rg.ResourceGroupName -TemplateFile "$scriptDir\azuredeploy.json" -baseName $baseName

# Add currently logged in user to Data Contrib Role
$principalId = (Get-AzADUser -DisplayName $azureUsername -first 1).Id

# Write-Host $principalId

$roleAssignments = Get-AzRoleAssignment -RoleDefinitionName "Storage Blob Data Contributor" -ObjectId $principalId -ResourceGroupName $rg.ResourceGroupName

if (!$roleAssignments.DisplayName) {
    Write-Host "Assigning Role to Local User"
    New-AzRoleAssignment -ObjectId $principalId -RoleDefinitionName  "Storage Blob Data Contributor" -ResourceGroupName $rg.ResourceGroupName
}

Write-Host "Done"
