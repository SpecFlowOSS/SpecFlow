using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared
{
    /// <summary>
    /// Constants about the protocol.
    /// </summary>
    public static class BuildProtocolConstants
    {
        /// <summary>
        /// The version number for this protocol.
        /// </summary>
        public const uint ProtocolVersion = 2;

        // Arguments for CSharp and VB Compiler
        public enum ArgumentId
        {
            // The current directory of the client
            CurrentDirectory = 0x51147221,

            // A comment line argument. The argument index indicates which one (0 .. N)
            CommandLineArgument,

            // The "LIB" environment variable of the client
            LibEnvVariable,

            // Request a longer keep alive time for the server
            KeepAlive,

            // Request a server shutdown from the client
            Shutdown,

            // The directory to use for temporary operations.
            TempDirectory,
        }

        /// <summary>
        /// Read a string from the Reader where the string is encoded
        /// as a length prefix (signed 32-bit integer) followed by
        /// a sequence of characters.
        /// </summary>
        public static string ReadLengthPrefixedString(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            return new String(reader.ReadChars(length));
        }

        /// <summary>
        /// Write a string to the Writer where the string is encoded
        /// as a length prefix (signed 32-bit integer) follows by
        /// a sequence of characters.
        /// </summary>
        public static void WriteLengthPrefixedString(BinaryWriter writer, string value)
        {
            writer.Write(value.Length);
            writer.Write(value.ToCharArray());
        }

        /// <summary>
        /// This task does not complete until we are completely done reading.
        /// </summary>
        internal static async Task ReadAllAsync(
            Stream stream,
            byte[] buffer,
            int count,
            CancellationToken cancellationToken)
        {
            int totalBytesRead = 0;
            do
            {
                CompilerServerLogger.Log("Attempting to read {0} bytes from the stream",
                    count - totalBytesRead);
                int bytesRead = await stream.ReadAsync(buffer,
                    totalBytesRead,
                    count - totalBytesRead,
                    cancellationToken).ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    CompilerServerLogger.Log("Unexpected -- read 0 bytes from the stream.");
                    throw new EndOfStreamException("Reached end of stream before end of read.");
                }
                CompilerServerLogger.Log("Read {0} bytes", bytesRead);
                totalBytesRead += bytesRead;
            } while (totalBytesRead < count);
            CompilerServerLogger.Log("Finished read");
        }
    }
}