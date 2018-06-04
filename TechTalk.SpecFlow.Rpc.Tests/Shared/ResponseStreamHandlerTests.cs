using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

using TechTalk.SpecFlow.Rpc.Shared;
using TechTalk.SpecFlow.Rpc.Shared.Response;
using Xunit;

// ReSharper disable InconsistentNaming

namespace TechTalk.SpecFlow.GeneratorTests.Rpc
{
    public class ResponseStreamHandlerTests
    {
        public static readonly IEnumerable<object[]> ResponseStreamHandler_Read_ShouldYieldObject_Data = new[]
        {
            new object[]
            {
                @"{ Assembly: ""TestAssembly"", Type: ""TestType"", Method: ""TestMethod"", Result: ""TestResult""}",
                new Response { Assembly = "TestAssembly", Type = "TestType", Method = "TestMethod", Result = "TestResult" }
            }
        };

        [Theory]
        [MemberData(nameof(ResponseStreamHandler_Read_ShouldYieldObject_Data))]
        public void ResponseStreamHandler_Read_ShouldYieldObject(string message, Response expected)
        {
            // ARRANGE
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memoryStream, Encoding.Default, leaveOpen: true))
                {
                    writer.Write(message.Length);
                    writer.Write(message.ToCharArray());
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                // ACT
                var result = ResponseStreamHandler.Read<Response>(memoryStream);

                // ASSERT
                result.Should().Be(expected);
            }
        }

        public static readonly IEnumerable<object[]> ResponseStreamHandler_ReadInvalid_ShouldThrowException_Data = new[]
        {
            new object[]
            {
                @"Assembly: ""TestAssembly"", Type: ""TestType"", Method: ""TestMethod"", Result: ""TestResult"""
            },
        };

        [Theory]
        [MemberData(nameof(ResponseStreamHandler_ReadInvalid_ShouldThrowException_Data))]
        public void ResponseStreamHandler_ReadInvalid_ShouldThrowException(string message)
        {
            // ARRANGE
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memoryStream, Encoding.Default, leaveOpen: true))
                {
                    writer.Write(message.Length);
                    writer.Write(message.ToCharArray());
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                Action GetTestAction(Stream input) => () => ResponseStreamHandler.Read<Response>(input);

                // ACT
                Action testAction = GetTestAction(memoryStream);

                // ASSERT
                testAction.ShouldThrow<Utf8Json.JsonParsingException>();
            }
        }

        public static readonly IEnumerable<object[]> ResponseStreamHandler_Write_ShouldYieldJson_Data = new[]
        {
            new object[]
            {
                new Response { Assembly = "TestAssembly", Type = "TestType", Method = "TestMethod", Result = "TestResult" },
                @"{""Assembly"":""TestAssembly"",""Type"":""TestType"",""Method"":""TestMethod"",""Result"":""TestResult""}"
            }
        };

        [Theory]
        [MemberData(nameof(ResponseStreamHandler_Write_ShouldYieldJson_Data))]
        public void ResponseStreamHandler_Write_ShouldYieldJson(Response message, string expected)
        {
            // ARRANGE
            using (var memoryStream = new MemoryStream())
            {
                // ACT
                ResponseStreamHandler.Write(message, memoryStream);
                
                // ASSERT
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new BinaryReader(memoryStream, Encoding.Default, leaveOpen: true))
                {
                    int length = reader.ReadInt32();
                    string content = new string(reader.ReadChars(length));
                    content.Should().Be(expected);
                }
            }
        }
    }
}
