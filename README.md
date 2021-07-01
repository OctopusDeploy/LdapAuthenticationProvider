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
|Nested Group Filter|The filter to use when searching for nested groups. The wildcard * will be replaced by the distinguished name of the initial group.|(&(objectClass=group)(uniqueMember=*))|
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
