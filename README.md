# Octopus Deploy LDAP Authentication Provider

[![Build, Test, Package and Push](https://github.com/OctopusDeploy/LdapAuthenticationProvider/actions/workflows/main.yml/badge.svg)](https://github.com/OctopusDeploy/LdapAuthenticationProvider/actions/workflows/main.yml)

This repository contains an LDAP authentication provider for [Octopus Deploy][1] based on the [Novell.Directory.Ldap.NETStandard][2] library.
It was originally authored by [@tunger](https://github.com/tunger) and transferred to [@OctopusDeploy](https://github.com/OctopusDeploy) in May 2021
This project is based on the [Octopus Deploy DirectoryServices authentication provider][3].

## Compatibility

| LDAP Extension Release | Compatible Octopus Server Release | Notes |
|---|---|---|
| 0.9.* | 2020.6 | Custom Extension.  Last version of community created extension |
| 1.0.* | 2021.1 | Custom Extension.  Officially provided by [@OctopusDeploy](https://github.com/OctopusDeploy) |
| 2.0.* | 2021.2 | Built in extension. Bundled with Octopus Server |
| 3.0.* | 2021.3 | Built in extension. Bundled with Octopus Server |

## Installation

From Octopus Server 2021.2 onwards (Ldap extension 2.0+) this extension is bundled with Octopus Server, so no installation is necessary.

For previous versions:

1. Grab a release from the [releases page](https://github.com/OctopusDeploy/LdapAuthenticationProvider/releases).
2. Install as a custom extension according to the [Octopus Deploy documentation][4].

## Configuration

In Octopus Deploy, navigate to Configuration -> Settings -> LDAP.

|Configuration|Description|Example|
|---|---|---|
|Server|The plain hostname of the LDAP server.|localhost|
|Port|The port to access the LDAP server.|389|
|Security Protocol|Options for secure connections (None, SSL (LDAPS) or StartTLS).|None|
|Ignore SSL errors|Whether to ignore certificate validation errors when using a secure connection method.|False|
|Username|The distinguished name of the user that the extension will use when connecting to the LDAP server.|cn=query,dc=example,dc=org|
|Password|The password of the user specified above.|***|
|User Base DN|The root distinguished name (DN) to use when running queries for Users.|cn=Users,dc=example,dc=org|
|Default Domain|This value is prepended to the username when no domain part is provided in the login form (format: DOMAIN\USERNAME). Can be left empty, in that case no domain is prepended.|
|User Filter|The filter to use when searching valid users. The wildcard * will be replaced with the search expression.|(&(objectClass=person)(sAMAccountName=*))|
|Group Base DN|The root distinguished name (DN) to use when running queries for Groups.|ou=Groups,dc=example,dc=org|
|Group Filter|The filter to use when searching valid user groups. The wildcard * will be replaced with the search expression.|(&(objectClass=group)(cn=*))|
|Nested Group Filter|The filter to use when searching for a group's parents. The wildcard * will be replaced by the distinguished name of the initial group.|(&(objectClass=group)(member=*))|
|Nested Group Search Depth|Specifies how many levels of nesting will be searched. Set to '0' to disable searching for nested groups.|5|
|Allow Auto User Creation|Specifies whether users not already set up in Octopus Deploy will be automatically created upon successful LDAP login.|false|
|Referral Following Enabled|Sets whether or not to allow referral following.|true|
|Referral Hop Limit|Sets the maximum number of referrals to follow during automatic referral following.|10|
|Constraint Time Limit|Sets the time limit in seconds for LDAP operations on the directory.  '0' specifies no limit.|0|
|Unique Account Name Attribute|Set the name of the LDAP attribute containing the unique account name, which is used to authenticate via the logon form.  This will be 'sAMAccountName' for Active Directory.|sAMAccountName|
|User Display Name Attribute|The name of the LDAP attribute containing the user's full name.|displayName|
|User Principal Name Attribute|The name of the LDAP attribute containing the user's principal name.|userPrincipalName|
|User Membership Attribute|The name of the LDAP attribute to use when loading the user's groups.|memberOf|
|User Email Attribute|The name of the LDAP attribute containing the user's email address.|mail|
|Group Name Attribute|The name of the LDAP attribute containing the group's name.|cn|
|Is Enabled|Enables the authentication provider if true.|true|

## Usage

This extension behaves similar to the AD authentication provider. 
- You can map LDAP groups to teams.
- You can assign logins to existing users, or simply login if you have never logged in.
- You can see mapped teams from external groups in user details.

Refer to the [Octopus Documentation][5] for more information.

[1]: https://octopus.com
[2]: https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard
[3]: https://github.com/OctopusDeploy/DirectoryServicesAuthenticationProvider
[4]: https://octopus.com/docs/administration/server-extensibility/installing-a-custom-server-extension
[5]: https://octopus.com/docs/security/users-and-teams/external-groups-and-roles

## Build and release pipeline

This project is built using GitHub actions. A NuGet package is created and pushed to Octopus Deploy, where it is deployed to NuGet repositories.

## Testing

Integration testing against Microsoft ActiveDirectory and OpenLdap using known test data is used to validate behaviour - see [here](source/Ldap.Integration.Tests/README.md) for more details.

## Issues

Please see [Contributing](CONTRIBUTING.md)
