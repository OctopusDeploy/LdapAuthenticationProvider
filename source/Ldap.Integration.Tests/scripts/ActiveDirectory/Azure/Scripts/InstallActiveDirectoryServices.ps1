Write-Host $(Get-Date) " >> Starting"

Import-Module ServerManager
Install-windowsfeature -name AD-Domain-Services –IncludeManagementTools
Install-WindowsFeature –Name GPMC
Write-Host $(Get-Date) " >> Tools Installed"


$domainname = "mycompany.local"
$NTDPath = "C:\Windows\ntds"
$logPath = "C:\Windows\ntds"
$sysvolPath = "C:\Windows\Sysvol"
$domainmode = "win2012R2"
$forestmode = "win2012R2"
$password = ConvertTo-SecureString "AdminPassword01!!" -AsPlainText -Force
Install-ADDSForest -CreateDnsDelegation:$false -DatabasePath $NTDPath -SafeModeAdministratorPassword $password -DomainMode $domainmode -DomainName $domainname -ForestMode $forestmode -InstallDns:$true -LogPath $logPath -NoRebootOnCompletion:$true -SysvolPath $sysvolPath -Force:$true
Write-Host $(Get-Date) " >> AD Configured"

shutdown /f /r /t 1