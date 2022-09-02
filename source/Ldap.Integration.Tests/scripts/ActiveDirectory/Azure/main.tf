terraform {
  required_version = ">=0.12"
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "~>2.0"
    }
  }
}

provider "azurerm" {
  features {}
}

## <https://www.terraform.io/docs/providers/azurerm/r/resource_group.html>
resource "azurerm_resource_group" "rg" {
  name     = "rg_authtest_ad"
  location =  var.resource_group_location
}

## <https://www.terraform.io/docs/providers/azurerm/r/virtual_network.html>
resource "azurerm_virtual_network" "vnet" {
  name                = "vNet1"
  address_space       = ["10.0.0.0/16"]
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
}

## <https://www.terraform.io/docs/providers/azurerm/r/subnet.html> 
resource "azurerm_subnet" "subnet" {
  name                 = "subnet"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes       = ["10.0.2.0/24"]
}

resource "azurerm_public_ip" "public_ip" {
  name                = "public_ip"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  allocation_method   = "Dynamic"
}

## <https://www.terraform.io/docs/providers/azurerm/r/network_interface.html>
resource "azurerm_network_interface" "example" {
  name                = "nic1"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name


  ip_configuration {
    name                          = "internal"
    subnet_id                     = azurerm_subnet.subnet.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id = azurerm_public_ip.public_ip.id 
  }
}

# Create Network Security Group and rule
resource "azurerm_network_security_group" "myterraformnsg" {
    name                = "securitygroup"
    location            = azurerm_resource_group.rg.location
    resource_group_name = azurerm_resource_group.rg.name

    security_rule {
        name                       = "LDAP"
        priority                   = 1002
        direction                  = "Inbound"
        access                     = "Allow"
        protocol                   = "Tcp"
        source_port_range          = "*"
        destination_port_range     = "389"
        source_address_prefix      = "*"
        destination_address_prefix = "*"
    }
}

resource "azurerm_network_interface_security_group_association" "example" {
    network_interface_id      = azurerm_network_interface.example.id
    network_security_group_id = azurerm_network_security_group.myterraformnsg.id
}


resource "random_password" "password" {
  length           = 16
  special          = true
  override_special = "_%@"
}

## <https://www.terraform.io/docs/providers/azurerm/r/windows_virtual_machine.html>
resource "azurerm_windows_virtual_machine" "example" {
  name                = "ad-vm"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  size                = "Standard_F2"
  admin_username      = "adminuser"
  admin_password      = random_password.password.result
  network_interface_ids = [
    azurerm_network_interface.example.id,
  ]

  os_disk {
    caching              = "ReadWrite"
    storage_account_type = "Standard_LRS"
  }

  source_image_reference {
    publisher = "MicrosoftWindowsServer"
    offer     = "WindowsServer"
    sku       = "2016-Datacenter"
    version   = "latest"
  }
}

#Provisioning the AD Services
resource "azurerm_virtual_machine_extension" "ad_install" {
  name                 = "Active_Directory_Installation"
  virtual_machine_id   = azurerm_windows_virtual_machine.example.id
  publisher            = "Microsoft.Compute"
  type                 = "CustomScriptExtension"
  type_handler_version = "1.10"

  protected_settings = <<SETTINGS
  {
     "commandToExecute": "powershell -encodedCommand ${textencodebase64(file("Scripts/InstallActiveDirectoryServices.ps1"), "UTF-16LE")}"
  }
  SETTINGS
}

#Provisioning the AD Data must take place after AD Service installation above
locals {
  # Before attempting any AD commands, we have to wait for the ADWS to come online
  waitForAdScript = <<SCRIPT
      [int]$Retrycount = 10 
      [int]$SleepSeconds = 30
      do {
          try {
              #Run very basic AD script that will fail until the ADWS is online and responding
              Get-ADUser -Filter * | Out-Null
              break
          }
          catch {
              if ($Retrycount -lt 0) {
                  Write-Host "Operation Failed after $Retrycount retrys."
                  return $false
              } else {
                  Write-Host "Operation Failed retrying in 30 seconds..."
                  Start-Sleep -Seconds $SleepSeconds
                  $Retrycount = $Retrycount - 1
              }
          }
      }
      While ($true)
  SCRIPT
  settings_windows = {
    script   = "${concat(split("\n", local.waitForAdScript), split("\n", file("Scripts/ConfigureActiveDirectoryData.ps1")))}"
  }
}

resource "azurerm_virtual_machine_extension" "ad_configure" {
  name                        = "Active_Directory_Configuration"
  virtual_machine_id          =  azurerm_windows_virtual_machine.example.id
  publisher                   = "Microsoft.CPlat.Core"
  type                        = "RunCommandWindows"
  type_handler_version        = "1.1"
  auto_upgrade_minor_version  = true
  settings                    = "${jsonencode(local.settings_windows)}"
  depends_on                  = [ azurerm_virtual_machine_extension.ad_install ]
}