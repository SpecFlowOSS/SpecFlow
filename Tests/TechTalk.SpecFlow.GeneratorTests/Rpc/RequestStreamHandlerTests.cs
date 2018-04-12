using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Rpc.Shared;
using TechTalk.SpecFlow.Rpc.Shared.Request;
using TechTalk.SpecFlow.Rpc.Shared.Response;
using Xunit;

// ReSharper disable InconsistentNaming

namespace TechTalk.SpecFlow.GeneratorTests.Rpc
{
    public class RequestStreamHandlerTests
    {
        public static readonly IEnumerable<object[]> RequestStreamHandler_Read_ShouldYieldObject_Data = new[]
        {
            new object[]
            {
                @"{ Assembly: ""TestAssembly"", Type: ""TestType"", Method: ""TestMethod"", Arguments: ""Args"", IsShutDown: true, IsPing: false }",
                new Request { Assembly = "TestAssembly", Type = "TestType", Method = "TestMethod", Arguments = "Args", IsShutDown = true, IsPing = false }
            }
        };

        [Theory(DisplayName = "Reading valid data should yield a request")]
        [MemberData(nameof(RequestStreamHandler_Read_ShouldYieldObject_Data))]
        public void RequestStreamHandler_Read_ShouldYieldObject(string message, Request expected)
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
                var result = RequestStreamHandler.Read<Request>(memoryStream);

                // ASSERT
                result.Should().Be(expected);
            }
        }

        public static readonly IEnumerable<object[]> RequestStreamHandler_ReadInvalid_ShouldThrowException_Data = new[]
        {
            new object[]
            {
                @"Assembly: ""TestAssembly"", Type: ""TestType"", Method: ""TestMethod"""
            },
        };

        [Theory(DisplayName = "Reading invalid data should throw exception")]
        [MemberData(nameof(RequestStreamHandler_ReadInvalid_ShouldThrowException_Data))]
        public void RequestStreamHandler_ReadInvalid_ShouldThrowException(string message)
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

                Action GetTestAction(Stream input) => () => RequestStreamHandler.Read<Request>(input);

                // ACT
                Action testAction = GetTestAction(memoryStream);

                // ASSERT
                testAction.ShouldThrow<JsonReaderException>();
            }
        }

        public static readonly IEnumerable<object[]> RequestStreamHandler_Write_ShouldYieldJson_Data = new[]
        {
            new object[]
            {
                new Request { Assembly = "TestAssembly", Type = "TestType", Method = "TestMethod", Arguments = "Args", IsShutDown = true, IsPing = false },
                @"{""Assembly"":""TestAssembly"",""Type"":""TestType"",""Method"":""TestMethod"",""Arguments"":""Args"",""IsShutDown"":true,""IsPing"":false}",
            }
        };

        [Theory(DisplayName = "Writing a request should yield JSON data")]
        [MemberData(nameof(RequestStreamHandler_Write_ShouldYieldJson_Data))]
        public void RequestStreamHandler_Write_ShouldYieldJson(Request message, string expected)
        {
            // ARRANGE
            using (var memoryStream = new MemoryStream())
            {
                // ACT
                RequestStreamHandler.Write(message, memoryStream);
                
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
