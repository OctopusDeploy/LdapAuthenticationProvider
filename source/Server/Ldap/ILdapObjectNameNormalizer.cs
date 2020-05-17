namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface ILdapObjectNameNormalizer
    {
        void NormalizeName(string name, out string namePart, out string domainPart);

        string BuildUserName(string name, string domain);
    }
}