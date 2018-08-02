namespace TcpStumps
{
    using System;

    public static class FluentServerExtensions
    {
        public static TcpStumpsServer AllowingExternalConnections(this TcpStumpsServer server)
        {
            server.AllowExternalConnection = true;
            return server;
        }

        public static TcpStumpsServer ListeningOnPort(this TcpStumpsServer server, int port)
        {
            server.ListeningPort = port;
            return server;
        }

        public static TcpStump HandlesRequest(this TcpStumpsServer server)
        {
            var stumpId = Guid.NewGuid().ToString();
            var stump = server.AddNewStump(stumpId);
            return stump;
        }

        public static TcpStump HandlesRequest(this TcpStumpsServer server, string stumpId)
        {
            var stump = server.AddNewStump(stumpId);
            return stump;
        }

        public static FluentProtocolBuilder UsesProtocol(this TcpStumpsServer server)
        {
            var protocolBuilder = new FluentProtocolBuilder(server);
            return protocolBuilder;
        }

        public static TcpResponse ReturnsByDefault(this IProtocolFactory protocolFactory)
        {
            var response = new TcpResponse();
            protocolFactory.DefaultResponse = response;
            return response;
        }

    }
}
