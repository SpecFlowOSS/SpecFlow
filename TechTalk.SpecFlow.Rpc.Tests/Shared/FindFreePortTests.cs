using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow.Rpc.Shared;
using Xunit;

namespace TechTalk.SpecFlow.Rpc.Tests.Shared
{
    public class FindFreePortTests
    {
        [Fact]
        public void GetAvailablePort_FreePort()
        {
            //ARRANGE

            //ACT
            var freePort = FindFreePort.GetAvailablePort(3483);

            //ASSERT
            freePort.Should().Be(3483);
        }

        [Fact]
        public void GetAvailablePort_StartPortIsUsed()
        {
            //ARRANGE
            var listener = new TcpListener(new IPEndPoint(IPAddress.Loopback, 3483));
            listener.Start();

            try
            {
                //ACT
                var freePort = FindFreePort.GetAvailablePort(3483);

                //ASSERT
                freePort.Should().BeGreaterThan(3483);
            }
            finally
            {
                listener.Stop();

            }
        }
    }
}
