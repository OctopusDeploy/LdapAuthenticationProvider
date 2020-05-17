namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface ILdapContextProvider
    {
        LdapContext GetContext();
    }
}