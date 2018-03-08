// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// This file describes data structures about the protocol from client program to server that is 
// used. The basic protocol is this.
//
// After the server pipe is connected, it forks off a thread to handle the connection, and creates
// a new instance of the pipe to listen for new clients. When it gets a request, it validates
// the security and elevation level of the client. If that fails, it disconnects the client. Otherwise,
// it handles the request, sends a response (described by Response class) back to the client, then
// disconnects the pipe and ends the thread.

namespace TechTalk.SpecFlow.CodeBehindGenerator.Shared.Request
{
    /// <summary>
    /// Represents a request from the client. A request is as follows.
    /// 
    ///  Field Name         Type                Size (bytes)
    /// ----------------------------------------------------
    ///  Argument Count     UInteger            4
    ///  Arguments          Argument[]          Variable
    /// 
    /// See <see cref="Argument"/> for the format of an
    /// Argument.
    /// 
    /// </summary>
    public class BuildRequest
    {
        public readonly uint ProtocolVersion;
        public readonly ReadOnlyCollection<Argument> Arguments;

        public BuildRequest(IEnumerable<Argument> arguments)
        {
            Arguments = new ReadOnlyCollection<Argument>(arguments.ToList());

            if (Arguments.Count > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(arguments),
                    "Too many arguments: maximum of "
                    + ushort.MaxValue + " arguments allowed.");
            }
        }

        public static BuildRequest Create(string workingDirectory,
                                          string tempDirectory,
                                          IList<string> args,
                                          string keepAlive = null,
                                          string libDirectory = null)
        {
            CompilerServerLogger.Log("Creating BuildRequest");
            CompilerServerLogger.Log($"Working directory: {workingDirectory}");
            CompilerServerLogger.Log($"Temp directory: {tempDirectory}");
            CompilerServerLogger.Log($"Lib directory: {libDirectory ?? "null"}");

            var requestLength = args.Count + 1 + (libDirectory == null ? 0 : 1);
            var requestArgs = new List<Argument>(requestLength);

            //requestArgs.Add(new Argument(BuildProtocolConstants.ArgumentId.CurrentDirectory, 0, workingDirectory));
            //requestArgs.Add(new Argument(BuildProtocolConstants.ArgumentId.TempDirectory, 0, tempDirectory));

          
            return new BuildRequest(requestArgs);
        }

        public static BuildRequest CreateShutdown()
        {
            var requestArgs = new[] { new Argument(BuildProtocolConstants.ArgumentId.Shutdown, argumentIndex: 0, value: "") };
            return new BuildRequest(requestArgs);
        }

        /// <summary>
        /// Read a Request from the given stream.
        /// 
        /// The total request size must be less than 1MB.
        /// </summary>
        /// <returns>null if the Request was too large, the Request otherwise.</returns>
        public static async Task<BuildRequest> ReadAsync(Stream inStream, CancellationToken cancellationToken)
        {
            // Read the length of the request
            var lengthBuffer = new byte[4];
            CompilerServerLogger.Log("Reading length of request");
            await BuildProtocolConstants.ReadAllAsync(inStream, lengthBuffer, 4, cancellationToken).ConfigureAwait(false);
            var length = BitConverter.ToInt32(lengthBuffer, 0);

            // Back out if the request is > 1MB
            if (length > 0x100000)
            {
                CompilerServerLogger.Log("Request is over 1MB in length, cancelling read.");
                return null;
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Read the full request
            var requestBuffer = new byte[length];
            await BuildProtocolConstants.ReadAllAsync(inStream, requestBuffer, length, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            CompilerServerLogger.Log("Parsing request");
            // Parse the request into the Request data structure.
            using (var reader = new BinaryReader(new MemoryStream(requestBuffer), Encoding.Unicode))
            {
                uint argumentCount = reader.ReadUInt32();

                var argumentsBuilder = new List<Argument>((int)argumentCount);

                for (int i = 0; i < argumentCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    argumentsBuilder.Add(BuildRequest.Argument.ReadFromBinaryReader(reader));
                }

                return new BuildRequest(argumentsBuilder);
            }
        }

        /// <summary>
        /// Write a Request to the stream.
        /// </summary>
        public async Task WriteAsync(Stream outStream, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream, Encoding.Unicode))
            {
                // Format the request.
                CompilerServerLogger.Log("Formatting request");
                writer.Write(Arguments.Count);
                foreach (Argument arg in Arguments)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    arg.WriteToBinaryWriter(writer);
                }
                writer.Flush();

                cancellationToken.ThrowIfCancellationRequested();

                // Write the length of the request
                int length = checked((int)memoryStream.Length);

                // Back out if the request is > 1 MB
                if (memoryStream.Length > 0x100000)
                {
                    CompilerServerLogger.Log("Request is over 1MB in length, cancelling write");
                    throw new ArgumentOutOfRangeException();
                }

                // Send the request to the server
                CompilerServerLogger.Log("Writing length of request.");
                await outStream.WriteAsync(BitConverter.GetBytes(length), 0, 4,
                                           cancellationToken).ConfigureAwait(false);

                CompilerServerLogger.Log("Writing request of size {0}", length);
                // Write the request
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(outStream, bufferSize: length, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// A command line argument to the compilation. 
        /// An argument is formatted as follows:
        /// 
        ///  Field Name         Type            Size (bytes)
        /// --------------------------------------------------
        ///  ID                 UInteger        4
        ///  Index              UInteger        4
        ///  Value              String          Variable
        /// 
        /// Strings are encoded via a length prefix as a signed
        /// 32-bit integer, followed by an array of characters.
        /// </summary>
        public struct Argument
        {
            public readonly BuildProtocolConstants.ArgumentId ArgumentId;
            public readonly int ArgumentIndex;
            public readonly string Value;

            public Argument(BuildProtocolConstants.ArgumentId argumentId,
                            int argumentIndex,
                            string value)
            {
                ArgumentId = argumentId;
                ArgumentIndex = argumentIndex;
                Value = value;
            }

            public static Argument ReadFromBinaryReader(BinaryReader reader)
            {
                var argId = (BuildProtocolConstants.ArgumentId)reader.ReadInt32();
                var argIndex = reader.ReadInt32();
                string value = BuildProtocolConstants.ReadLengthPrefixedString(reader);
                return new Argument(argId, argIndex, value);
            }

            public void WriteToBinaryWriter(BinaryWriter writer)
            {
                writer.Write((int)ArgumentId);
                writer.Write(ArgumentIndex);
                BuildProtocolConstants.WriteLengthPrefixedString(writer, Value);
            }
        }
    }
}
