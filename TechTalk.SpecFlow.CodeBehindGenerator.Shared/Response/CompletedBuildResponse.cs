using System;
using System.IO;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    /// <summary>
    /// Represents a Response from the server. A response is as follows.
    /// 
    ///  Field Name         Type            Size (bytes)
    /// --------------------------------------------------
    ///  Length             UInteger        4
    ///  ReturnCode         Integer         4
    ///  Output             String          Variable
    ///  ErrorOutput        String          Variable
    /// 
    /// Strings are encoded via a character count prefix as a 
    /// 32-bit integer, followed by an array of characters.
    /// 
    /// </summary>
    public sealed class CompletedBuildResponse : BuildResponse
    {
        public readonly int ReturnCode;
        public readonly bool Utf8Output;
        public readonly string Output;
        public readonly string ErrorOutput;

        public CompletedBuildResponse(int returnCode,
            bool utf8output,
            string output)
        {
            ReturnCode = returnCode;
            Utf8Output = utf8output;
            Output = output;

            // This field existed to support writing to Console.Error.  The compiler doesn't ever write to 
            // this field or Console.Error.  This field is only kept around in order to maintain the existing
            // protocol semantics.
            ErrorOutput = string.Empty;
        }

        public override ResponseType Type => ResponseType.Completed;

        public static CompletedBuildResponse Create(BinaryReader reader)
        {
            var returnCode = reader.ReadInt32();
            var utf8Output = reader.ReadBoolean();
            var output = BuildProtocolConstants.ReadLengthPrefixedString(reader);
            var errorOutput = BuildProtocolConstants.ReadLengthPrefixedString(reader);
            if (!string.IsNullOrEmpty(errorOutput))
            {
                throw new InvalidOperationException();
            }

            return new CompletedBuildResponse(returnCode, utf8Output, output);
        }

        protected override void AddResponseBody(BinaryWriter writer)
        {
            writer.Write(ReturnCode);
            writer.Write(Utf8Output);
            BuildProtocolConstants.WriteLengthPrefixedString(writer, Output);
            BuildProtocolConstants.WriteLengthPrefixedString(writer, ErrorOutput);
        }
    }
}