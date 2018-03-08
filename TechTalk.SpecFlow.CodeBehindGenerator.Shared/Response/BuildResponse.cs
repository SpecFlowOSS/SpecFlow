using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    /// <summary>
    /// Base class for all possible responses to a request.
    /// The ResponseType enum should list all possible response types
    /// and ReadResponse creates the appropriate response subclass based
    /// on the response type sent by the client.
    /// The format of a response is:
    ///
    /// Field Name       Field Type          Size (bytes)
    /// -------------------------------------------------
    /// responseLength   int (positive)      4  
    /// responseType     enum ResponseType   4
    /// responseBody     Response subclass   variable
    /// </summary>
    public abstract class BuildResponse
    {
        public enum ResponseType
        {
            // The client and server are using incompatible protocol versions.
            MismatchedVersion,

            // The build request completed on the server and the results are contained
            // in the message. 
            Completed,

            // The build request could not be run on the server due because it created
            // an unresolvable inconsistency with analyzers.  
            AnalyzerInconsistency,

            // The shutdown request completed and the server process information is 
            // contained in the message. 
            Shutdown,

            // The request was rejected by the server.  
            Rejected,
        }

        public abstract ResponseType Type { get; }

        public async Task WriteAsync(Stream outStream,
            CancellationToken cancellationToken)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream, Encoding.Unicode))
            {
                // Format the response
                CompilerServerLogger.Log("Formatting Response");
                writer.Write((int)Type);

                AddResponseBody(writer);
                writer.Flush();

                cancellationToken.ThrowIfCancellationRequested();

                // Send the response to the client

                // Write the length of the response
                int length = checked((int)memoryStream.Length);

                CompilerServerLogger.Log("Writing response length");
                // There is no way to know the number of bytes written to
                // the pipe stream. We just have to assume all of them are written.
                await outStream.WriteAsync(BitConverter.GetBytes(length),
                    0,
                    4,
                    cancellationToken).ConfigureAwait(false);

                // Write the response
                CompilerServerLogger.Log("Writing response of size {0}", length);
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(outStream, bufferSize: length, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        protected abstract void AddResponseBody(BinaryWriter writer);

        /// <summary>
        /// May throw exceptions if there are pipe problems.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<BuildResponse> ReadAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            CompilerServerLogger.Log("Reading response length");
            // Read the response length
            var lengthBuffer = new byte[4];
            await BuildProtocolConstants.ReadAllAsync(stream, lengthBuffer, 4, cancellationToken).ConfigureAwait(false);
            var length = BitConverter.ToUInt32(lengthBuffer, 0);

            // Read the response
            CompilerServerLogger.Log("Reading response of length {0}", length);
            var responseBuffer = new byte[length];
            await BuildProtocolConstants.ReadAllAsync(stream,
                responseBuffer,
                responseBuffer.Length,
                cancellationToken).ConfigureAwait(false);

            using (var reader = new BinaryReader(new MemoryStream(responseBuffer), Encoding.Unicode))
            {
                var responseType = (ResponseType)reader.ReadInt32();

                switch (responseType)
                {
                    case ResponseType.Completed:
                        return CompletedBuildResponse.Create(reader);
                    case ResponseType.MismatchedVersion:
                        return new MismatchedVersionBuildResponse();
                    case ResponseType.Shutdown:
                        return ShutdownBuildResponse.Create(reader);
                    case ResponseType.Rejected:
                        return new RejectedBuildResponse();
                    default:
                        throw new InvalidOperationException("Received invalid response type from server.");
                }
            }
        }
    }
}