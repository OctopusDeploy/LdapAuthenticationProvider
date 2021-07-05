Import-Module ActiveDirectory

New-ADGroup "Maintainers" -Path "OU=Groups,DC=mycompany,DC=local" -GroupCategory Security -GroupScope Global
New-ADGroup "Developers" -Path "OU=Groups,DC=mycompany,DC=local" -GroupCategory Security -GroupScope Global
New-ADGroup "DeveloperGroup1" -Path "OU=Groups,DC=mycompany,DC=local" -GroupCategory Security -GroupScope Global
New-ADGroup "DeveloperGroup2" -Path "OU=Groups,DC=mycompany,DC=local" -GroupCategory Security -GroupScope Global

$devPass = ConvertTo-SecureString -AsPlainText "devp@ss01!" -Force
New-ADUser -Name "developer1" -Enabled:$true -GivenName "Developer" -Surname "1" -SamAccountName "developer1" -UserPrincipalName "developer1@mycompany.local" -AccountPassword $devPass -EmailAddress "developer1@mycompany.local" -DisplayName "Developer User 1"
New-ADUser -Name "developer2" -Enabled:$true -GivenName "Developer" -Surname "2" -SamAccountName "developer2" -UserPrincipalName "developer2@mycompany.local" -AccountPassword $devPass -EmailAddress "developer2@mycompany.local" -DisplayName "Developer User 2"

Get-ADGroup "Maintainers" | Add-ADGroupMember -Members @(Get-ADGroup "Developers")
Get-ADGroup "Developers" | Add-ADGroupMember -Members @(Get-ADGroup "DeveloperGroup1")
Get-ADGroup "Developers" | Add-ADGroupMember -Members @(Get-ADGroup "DeveloperGroup2")

Get-ADGroup "DeveloperGroup1" | Add-ADGroupMember -Members @(Get-ADUser "developer1")
Get-ADGroup "DeveloperGroup2" | Add-ADGroupMember -Members @(Get-ADUser "developer2")
