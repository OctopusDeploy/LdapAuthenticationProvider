# Octopus Deploy LDAP Authentication Provider

![continuous](https://github.com/tunger/OctopusDeploy-LdapAuthenticationProvider/workflows/continuous/badge.svg)

This repository contains a LDAP authentication provider for [Octopus Deploy][1] based on the [Novell.Directory.Ldap.NETStandard][2] library.
This project is based on the [Octopus Deploy DirectoryServices authentication provider][3].

## Installation

1. Grab a release from the [releases page](https://github.com/tunger/OctopusDeploy-LdapAuthenticationProvider/releases).
2. Install the extension according to [Octopus Deploy documentation][4].

## Configuration

In Octopus Deploy, navigate to Configuration -> Settings -> LDAP.

|Configuration|Description|Example|
|---|---|---|
|Server|The plain hostname of the LDAP server.|localhost|
|Port|The port to access the LDAP server.|389|
|Use SSL|Whether to use Secure Socket Layer to connect to LDAP.|False|
|Ignore SSL errors|Whether to ignore certificate validation errors.|False|
|Username|The distinguished name of the user that the extension will use when connecting to the LDAP server.|cn=query,dc=example,dc=org|
|Password|The password of the user specified above.|***|
|Base DN|The root distinguished name (DN) to use when running queries.|dc=example,dc=org|
|Default Domain|This value is prepended to the username when no domain part is provided in the login form (format: DOMAIN\USERNAME). Can be left empty, in that case no domain is prepended.|
|User Filter|The filter to use when searching valid users. The wildcard * will be replaced with the search expression.|(&(objectClass=person)(sAMAccountName=*))|
|Group Filter|The filter to use when searching valid user groups. The wildcard * will be replaced with the search expression.|(&(objectClass=group)(cn=*))|
|Username Attribute|The name of the LDAP attribute containing the username, which is used to authenticate users attempting to log in to Octopus.|sAMAccountName|
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

## Known limitations

- Automatic user creation is always enabled.

[1]: https://octopus.com
[2]: https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard
[3]: https://github.com/OctopusDeploy/DirectoryServicesAuthenticationProvider
[4]: https://octopus.com/docs/administration/server-extensibility/installing-a-custom-server-extension
[5]: https://octopus.com/docs/security/users-and-teams/external-groups-and-roles

## Build and release pipeline

This project is built using GitHub actions. A NuGet package is created and pushed to Octopus Deploy, where it is deployed to NuGet repositories.

## Issues

Please see [Contributing](CONTRIBUTING.md)
