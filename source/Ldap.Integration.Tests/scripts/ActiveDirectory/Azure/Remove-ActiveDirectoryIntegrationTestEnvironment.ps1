Write-Output "Tearing down Azure AD Terraform stack"

terraform destroy --auto-approve

Write-Output "DONE!"
exit 0