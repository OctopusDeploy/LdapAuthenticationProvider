Import-Module ActiveDirectory

New-ADOrganizationalUnit "Groups"

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

New-ADGroup "SpecialGroup (with brackets)" -Path "OU=Groups,DC=mycompany,DC=local" -GroupCategory Security -GroupScope Global
New-ADGroup "SpecialGroup# with a hash" -Path "OU=Groups,DC=mycompany,DC=local" -GroupCategory Security -GroupScope Global
New-ADGroup "SpecialGroup, with a comma" -SamAccountName "SpecialGroup_ with a comma" -Path "OU=Groups,DC=mycompany,DC=local" -GroupCategory Security -GroupScope Global
New-ADGroup "SpecialGroup * ( ) . & - _ [ ] `` ~ | @ $ % ^ ? : { } ! '" -SamAccountName "SpecialGroup _ ( ) . & - _ _ _ ` ~ _ @ $ % ^ _ _ { } ! '" -Path "OU=Groups,DC=mycompany,DC=local" -GroupCategory Security -GroupScope Global

New-ADUser -Name "special#1" -Enabled:$true -GivenName "Special" -Surname "#1" -SamAccountName "special#1" -UserPrincipalName "special#1@mycompany.local" -AccountPassword $devPass -EmailAddress "special#1@mycompany.local" -DisplayName "Special User #1"

Get-ADGroup "SpecialGroup (with brackets)" | Add-ADGroupMember  -Members @(Get-ADUser "special#1")
Get-ADGroup "SpecialGroup# with a hash" | Add-ADGroupMember  -Members @(Get-ADUser "special#1")
Get-ADGroup "SpecialGroup_ with a comma" | Add-ADGroupMember  -Members @(Get-ADUser "special#1")
Get-ADGroup "SpecialGroup _ ( ) . & - _ _ _ ` ~ _ @ $ % ^ _ _ { } ! '" | Add-ADGroupMember  -Members @(Get-ADUser "special#1")