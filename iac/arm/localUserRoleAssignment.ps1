param([string]$rgName, [string]$localUserName)

# Add currently logged in user to Data Contrib Role
$principalId = (Get-AzADUser -DisplayName $localUserName -first 1).Id

# Write-Host $principalId

$roleAssignments = Get-AzRoleAssignment -RoleDefinitionName "Storage Blob Data Contributor" -ObjectId $principalId -ResourceGroupName $rgName

if (!$roleAssignments.DisplayName) {
    Write-Host "Assigning Role to Local User"
    New-AzRoleAssignment -ObjectId $principalId -RoleDefinitionName  "Storage Blob Data Contributor" -ResourceGroupName $rgName
}