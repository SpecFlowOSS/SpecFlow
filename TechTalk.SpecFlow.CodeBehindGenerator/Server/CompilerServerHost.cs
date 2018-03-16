using System.Threading;
using TechTalk.SpecFlow.CodeBehindGenerator.Shared.Response;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    internal abstract class CompilerServerHost : ICompilerServerHost
    {
        /// <summary>
        /// Directory that contains the compiler executables and the response files. 
        /// </summary>
        public string ClientDirectory { get; }

        /// <summary>
        /// Directory that contains mscorlib.  Can be null when the host is executing in a CoreCLR context.
        /// </summary>
        public string SdkDirectory { get; }

        protected CompilerServerHost(string clientDirectory, string sdkDirectory)
        {
            ClientDirectory = clientDirectory;
            SdkDirectory = sdkDirectory;
        }
        

        public InitProjectResponse RunCompilation(RunRequest request, CancellationToken cancellationToken)
        {
            return new InitProjectResponse();
        }
    }
}