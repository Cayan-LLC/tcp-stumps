namespace TcpStumps
{
    using System;
    using System.Net;
    using System.Net.NetworkInformation;

    /// <summary>
    ///     A class that contains a set of Network related functions.
    /// </summary>
    public static class PortAvailability
    {
        /// <summary>
        /// The minimum value used when trying to find an open TCP port.
        /// </summary>
        public const int MinimumPort = 7000;

        /// <summary>
        /// The maximum value used when trying to find an open TCP port.
        /// </summary>
        public const int MaximumPort = 10000;

        /// <summary>
        ///     Finds a random open port.
        /// </summary>
        /// <returns>
        ///     An <see cref="Int32"/> representing an available TCP port.
        /// </returns>
        public static int FindRandomOpenPort()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var endpointList = properties.GetActiveTcpListeners();

            var usedPorts = new bool[PortAvailability.MaximumPort - PortAvailability.MinimumPort + 1];

            foreach (var endpoint in endpointList)
            {
                if (endpoint.Port >= PortAvailability.MinimumPort &&
                    endpoint.Port <= PortAvailability.MaximumPort)
                {
                    var port = endpoint.Port - PortAvailability.MinimumPort;
                    usedPorts[port] = true;
                }
            }

            var rnd = new Random();

            var foundPort = -1;

            // Maximum 100 tries for sanity
            for (int i = 0; i < 100; i++)
            {
                var portGuess = rnd.Next(usedPorts.Length - 1);

                if (!usedPorts[portGuess])
                {
                    foundPort = portGuess + PortAvailability.MinimumPort;
                    break;
                }
            }

            return foundPort;
        }

        /// <summary>
        ///     Determines whether the specified port is being used.
        /// </summary>
        /// <param name="localPort">The local port.</param>
        /// <returns>
        ///     <c>true</c> if the port is being used; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPortBeingUsed(int localPort)
        {
            if (localPort < IPEndPoint.MinPort || localPort > IPEndPoint.MaxPort)
            {
                return true;
            }

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endpointList = properties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in endpointList)
            {
                if (endpoint.Port == localPort)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
