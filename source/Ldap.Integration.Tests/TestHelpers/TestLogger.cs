using System;
using System.Runtime.CompilerServices;
using Octopus.Diagnostics;
using Xunit.Abstractions;

namespace Ldap.Integration.Tests.TestHelpers
{
    class TestLogger : ISystemLog
    {
        readonly ITestOutputHelper _testOutputHelper;

        public string CorrelationId => throw new NotImplementedException();

        public TestLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public static string GetCaller([CallerMemberName] string caller = null)
        {
            return caller;
        }

        public ISystemLog ChildContext(string[] sensitiveValues)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public void Error(string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText}");
        }

        public void Error(Exception error)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {error}");
        }

        public void Error(Exception error, string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText} {error}");
        }

        public void ErrorFormat(string messageFormat, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(messageFormat, args)}");
        }

        public void ErrorFormat(Exception error, string format, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(format, args)} {error}");
        }

        public void Fatal(string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText}");
        }

        public void Fatal(Exception error)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {error}");
        }

        public void Fatal(Exception error, string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText} {error}");
        }

        public void FatalFormat(string messageFormat, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(messageFormat, args)}");
        }

        public void FatalFormat(Exception error, string format, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(format, args)} {error}");
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void Info(string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText}");
        }

        public void Info(Exception error)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {error}");
        }

        public void Info(Exception error, string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText} {error}");
        }

        public void InfoFormat(string messageFormat, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(messageFormat, args)}");
        }

        public void InfoFormat(Exception error, string format, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(format, args)} {error}");
        }

        public void Trace(string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText}");
        }

        public void Trace(Exception error)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {error}");
        }

        public void Trace(Exception error, string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText}");
        }

        public void TraceFormat(string messageFormat, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(messageFormat, args)}");
        }

        public void TraceFormat(Exception error, string format, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(format, args)} {error}");
        }

        public void Verbose(string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText}");
        }

        public void Verbose(Exception error)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {error}");
        }

        public void Verbose(Exception error, string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText}");
        }

        public void VerboseFormat(string messageFormat, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(messageFormat, args)}");
        }

        public void VerboseFormat(Exception error, string format, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(format, args)} {error}");
        }

        public void Warn(string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText}");
        }

        public void Warn(Exception error)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {error}");
        }

        public void Warn(Exception error, string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {messageText}");
        }

        public void WarnFormat(string messageFormat, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(messageFormat, args)}");
        }

        public void WarnFormat(Exception error, string format, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {string.Format(format, args)} {error}");
        }

        public void WithSensitiveValue(string sensitiveValue)
        {
            throw new NotImplementedException();
        }

        public void WithSensitiveValues(string[] sensitiveValues)
        {
            throw new NotImplementedException();
        }

        public void Write(LogCategory category, string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {category} {messageText}");
        }

        public void Write(LogCategory category, Exception error)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {category} {error}");
        }

        public void Write(LogCategory category, Exception error, string messageText)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {category} {messageText} {error}");
        }

        public void WriteFormat(LogCategory category, string messageFormat, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {category} {string.Format(messageFormat, args)}");
        }

        public void WriteFormat(LogCategory category, Exception error, string messageFormat, params object[] args)
        {
            _testOutputHelper.WriteLine($"{GetCaller()}: {category} {string.Format(messageFormat,args)} {error}");
        }
    }
}
