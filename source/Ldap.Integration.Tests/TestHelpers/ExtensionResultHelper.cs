using System;
using Octopus.Server.Extensibility.Results;
using Xunit;

namespace Ldap.Integration.Tests.TestHelpers
{
    internal static class ExtensionResultHelper
    {
        internal static void AssertSuccesfulExtensionResult<T>(IResultFromExtension<T> result)
        {
            Assert.NotNull(result);
            if (result is FailureResultFromExtension)
            {
                throw new Exception(string.Join("\n", (result as FailureResultFromExtension).Errors));
            }
        }
    }
}