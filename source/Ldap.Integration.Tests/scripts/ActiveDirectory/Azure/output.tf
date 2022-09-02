output "resource_group_name" {
    value = azurerm_resource_group.rg.name
}

output "public_ip_addr" {
  value = azurerm_public_ip.public_ip.ip_address 
  description = "The public ip exposing the AD Server" 
  depends_on = [ azurerm_virtual_machine_extension.ad_configure ]
}
output "admin_password" {
  value = azurerm_windows_virtual_machine.example.admin_password
  description = "The login password for the AD Server"
  sensitive = true
}
output "admin_username" {
  value = azurerm_windows_virtual_machine.example.admin_username
  description = "The login user for the AD Server."
}

output "vm_name" {
    value = azurerm_windows_virtual_machine.example.name
}