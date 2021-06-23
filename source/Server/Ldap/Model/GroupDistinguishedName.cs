#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Octopus.Server.MessageContracts;
using Octopus.TinyTypes;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Model
{
    public class GroupDistinguishedName : CaseInsensitiveStringTinyType, IIdTinyType
    {
        public GroupDistinguishedName(string value)
            : base(value)
        {
        }
    }

    public static class GroupDistinguishedNameExtensionMethods
    {
        [return: NotNullIfNotNull("value")]
        public static GroupDistinguishedName? ToGroupDistinguishedName(this string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : new GroupDistinguishedName(value);
        }

        [return: NotNull]
        public static IEnumerable<GroupDistinguishedName> ToGroupDistinguishedNames(this IEnumerable<string>? values)
        {
            return values == null
                ? new GroupDistinguishedName[0]
                : values.Where(v => v != null)
                        .Select(v => v.ToGroupDistinguishedName());
        }
    }
}