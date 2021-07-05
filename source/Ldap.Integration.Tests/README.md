# Integration Tests

Integration tests exist for real world target instances of both OpenLdap and Microsoft Active Directory implementations that are setup with known test data.

## OpenLDAP

A docker-compose stack is defined that spins up an openldap server bootstrapped with known test data.

### Configuration

Environment variables can be used to configure the integration test settings.

| Env Var | Required | Default Value |
| --- | --- | --- |
| `OCTOPUS_LDAP_OPENLDAP_SERVER` | No | localhost |
| `OCTOPUS_LDAP_OPENLDAP_PORT` | No | 389 |
| `OCTOPUS_LDAP_OPENLDAP_USER` | No | cn=admin,dc=domain1,dc=local |
| `OCTOPUS_LDAP_OPENLDAP_PASSWORD` | Yes | |

### Running Integration Tests

Powershell scripts are located [here](scripts/OpenLdap) to create/tear down the test stack.

- Create the integration test stack
```
./New-OpenLdapIntegrationTestEnvironment.ps1
```

- Run the OpenLdap integration tests

- Tear down the integration test stack
```
./Remove-OpenLdapIntegrationTestEnvironment.ps1
```

## Active Directory

As it is not possible to containerize Microsoft AD, these need to be run against a VM hosted instance somewhere that gets populated with known test data.

[This Azure Template](https://github.com/maxskunkworks/TLG/tree/master/tlg-base-config_3-vm) is one relatively straight forward option to create an AD environment.  Firewall settings will generally need to applied manually to enable inbound connections.

### Configuration

Environment variables can be used to configure the integration test settings.

| Env Var | Required | Default Value |
| --- | --- | --- |
| `OCTOPUS_LDAP_AD_SERVER` | Yes | |
| `OCTOPUS_LDAP_AD_PORT` | No | 389 |
| `OCTOPUS_LDAP_AD_USER` | Yes | |
| `OCTOPUS_LDAP_AD_PASSWORD` | Yes | |

### Running Integration Tests

Powershell scripts are located [here](scripts/ActiveDirectory) to populate an existing ActiveDirectory instance with the required test data.  Due to cross platform and other limmitations, they currently are simple scripts that must be run from a machine connected to the test domain using an account with appropriate permissions.

- Populate the integration test data
```
./Update-ActiveDirectoryIntegrationTestData.ps1
```

- Run the ActiveDirectory integration tests

- Cleanup the integration test data
```
./Remove-ActiveDirectoryIntegrationTestData.ps1
```