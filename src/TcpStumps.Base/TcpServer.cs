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

                    var protocol = _protocolFactory.CreateProtocol();
                    var connection = new ClientConnection(protocol, _pipeline, acceptSocket);
                    _ = HandleConnectionAsync(connection);

                    if (protocol.ResponseOnConnection != null)
                    {
                        var result = this.SendResponse(connection, protocol.ResponseOnConnection);
                        if (result == PipelineResult.StopAndDisconnect)
                        {
                            connection.Disconnect();
                        } 
                    }
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

        private PipelineResult SendResponse(ClientConnection connection, TcpResponse response)
        {
            var result = PipelineResult.Continue;

            if (connection == null || response == null)
            {
                return result;
            }

            foreach (var message in response)
            {
                var delay = message.ResponseDelay;
                delay = delay < MessagePipeline.MaximumResponseDelay ? delay : MessagePipeline.MaximumResponseDelay;
                delay = delay > MessagePipeline.MinimumResponseDelay ? delay : MessagePipeline.MinimumResponseDelay;

                if (delay > 0)
                {
                    Task.WaitAll(Task.Delay(delay));
                }

                if (message.Message?.Length > 0)
                {
                    Task.WaitAll(connection.SendAsync(message.Message));
                }

                if (message.TerminateConnection)
                {
                    result = PipelineResult.StopAndDisconnect;
                    break;
                }
            }

            return result;
        }

        public void Dispose()
        {
            Task.WhenAll(this.StopAsync());
        }
    }
}
