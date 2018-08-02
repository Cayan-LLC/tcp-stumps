namespace TcpStumps
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class TcpServer : IDisposable
    {
        private readonly IMessagePipeline _pipeline = new MessagePipeline();
        private IPEndPoint _endpoint;
        private Socket _listenSocket;
        private Task _listenTask;
        private IProtocolFactory _protocolFactory;

        private volatile bool _unbinding;

        public TcpServer(IProtocolFactory protocolFactory, IPEndPoint endpoint)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _protocolFactory = protocolFactory ?? throw new ArgumentNullException(nameof(protocolFactory));
        }

        public event EventHandler<ConnectionEventArgs> OnClientConnection;

        public event EventHandler<ConnectionEventArgs> OnClientDisconnect;

        public IMessagePipeline Pipline => _pipeline;

        public Task StartAsync()
        {
            if (_listenSocket != null)
            {
                throw new InvalidOperationException();
            }

            var listenSocket = new Socket(_endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(_endpoint);
            listenSocket.Listen(32);

            _listenSocket = listenSocket;
            _listenTask = Task.Run(() => RunAcceptLoopAsync());

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (_listenSocket != null)
            {
                _unbinding = true;
                _listenSocket.Dispose();

                await _listenTask.ConfigureAwait(false);

                _unbinding = false;
                _listenSocket = null;
                _listenTask = null;
            }
        }

        private async Task RunAcceptLoopAsync()
        {
            try
            {
                while (true)
                {
                    var acceptSocket = await _listenSocket.AcceptAsync();
                    acceptSocket.NoDelay = true;

                    var connection = new ClientConnection(_protocolFactory.CreateProtocol(), _pipeline, acceptSocket);
                    _ = HandleConnectionAsync(connection);
                }
            }
            catch (SocketException) when (!_unbinding)
            {
            }
        }

        private async Task HandleConnectionAsync(ClientConnection connection)
        {
            var task = connection.StartAsync();

            this.OnClientConnection?.Invoke(this, new ConnectionEventArgs(connection));
            connection.OnDisconnect += (o, e) => this.OnClientDisconnect?.Invoke(o, e);

            await task;
        }

        public void Dispose()
        {
            Task.WhenAll(this.StopAsync());
        }
    }
}
