Write-Output "Setting up Azure AD Terraform stack"

terraform init
terraform plan
terraform apply --auto-approve

#We ned to refresh the state since the public vm IP isn't allocated when the resource is first provisioned
terraform refresh

$output=(terraform output -json | ConvertFrom-json)
Write-Host "Update your Environment Variables"
Write-Host "    OCTOPUS_LDAP_AD_SERVER=$($output.public_ip_addr.value)"
Write-Host "    OCTOPUS_LDAP_AD_USER=$($output.admin_username.value)@mycompany.local"
Write-Host "    OCTOPUS_LDAP_AD_PASSWORD=$($output.admin_password.value)"
Write-Host "    OCTOPUS_LDAP_AD_PORT=389"

