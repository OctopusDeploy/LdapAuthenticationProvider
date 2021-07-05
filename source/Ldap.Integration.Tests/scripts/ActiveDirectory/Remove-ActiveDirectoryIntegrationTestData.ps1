Import-Module ActiveDirectory

Get-ADGroup "Maintainers" | Remove-ADGroup -Confirm:$false
Get-ADGroup "Developers" | Remove-ADGroup -Confirm:$false
Get-ADGroup "DeveloperGroup1" | Remove-ADGroup -Confirm:$false
Get-ADGroup "DeveloperGroup2" | Remove-ADGroup -Confirm:$false

Get-ADUser "developer1" | Remove-ADUser -Confirm:$false
Get-ADUser "developer2" | Remove-ADUser -Confirm:$false
