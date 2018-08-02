namespace TcpStumps
{
    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using Xunit;

    public class PortAvailabilityTests
    {
        [Fact]
        public void FindRandomOpenPort_ReturnsPort()
        {
            var port = PortAvailability.FindRandomOpenPort();
            Assert.NotEqual(-1, port);
        }

        [Fact]
        public void FindRandomOpenPort_FindsValidPort()
        {
            var port = PortAvailability.FindRandomOpenPort();
            Assert.False(IsPortInUse(port));
        }

        [Fact]
        public void IsPortInUse_With135_ReturnsTrue()
        {
            /* This is a really crappy test and only works on Windows environments -- my apologies. */

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Assert.True(PortAvailability.IsPortBeingUsed(135));
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void IsPortInUse_OutsideOfRange_ReturnsTrue()
        {
            Assert.True(PortAvailability.IsPortBeingUsed(IPEndPoint.MinPort - 1));
            Assert.True(PortAvailability.IsPortBeingUsed(IPEndPoint.MaxPort + 1));
        }

        private static bool IsPortInUse(int port)
        {
            var globalIpProperties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = globalIpProperties.GetActiveTcpConnections();

            var isPortInUse = false;

            foreach (var connection in connections)
            {
                if (connection.LocalEndPoint.Port == port)
                {
                    isPortInUse = true;
                    break;
                }
            }

            return isPortInUse;
        }
    }
}
