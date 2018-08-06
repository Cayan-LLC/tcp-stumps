namespace TcpStumps
{
    using System;
    using System.IO.Pipelines;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    internal class ClientConnection : IConnection
    {
        private readonly IProtocol _protocol;
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        private readonly IMessagePipeline _pipeline;

        public ClientConnection(IProtocol protocol, IMessagePipeline pipeline, Socket socket)
        {
            _protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }

        public event EventHandler<ConnectionEventArgs> OnDisconnect;

        public Guid ConnectionId
        {
            get;
        } = Guid.NewGuid();

        public void Disconnect()
        {
            _socket?.Close();
        }

        public Task StartAsync()
        {
            var pipe = new Pipe();

            var writer = FillPipeAsync(pipe.Writer);
            var reader = ReadPipeAsync(pipe.Reader);

            return Task.WhenAll(writer, reader);
        }

        public async Task<int> SendAsync(byte[] buffer)
        {
            var segment = new ArraySegment<byte>(buffer);
            return await SendAsync(segment);
        }

        public async Task<int> SendAsync(byte[] buffer, int offset, int count)
        {
            var segment = new ArraySegment<byte>(buffer, offset, count);
            return await SendAsync(segment);
        }

        public async Task<int> SendAsync(ArraySegment<byte> buffer)
        {
            return await _socket.SendAsync(buffer, SocketFlags.None);
        }

        private async Task FillPipeAsync(PipeWriter writer)
        {
            while (true)
            {
                var buffer = writer.GetMemory(512).GetArray();

                try
                {
                    var bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    writer.Advance(bytesRead);
                }
                catch
                {
                    break;
                }

                var result = await writer.FlushAsync();

                if (result.IsCompleted)
                {
                    break;
                }
            }

            writer.Complete();
        }

        private async Task ReadPipeAsync(PipeReader reader)
        {
            while (true)
            {
                var result = await reader.ReadAsync();

                var buffer = result.Buffer;

                var protocolResult =  _protocol.ProcessBufferForMessage(this, _pipeline, ref buffer);

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }

                if (protocolResult == ProtocolProcessingBehavior.Disconnect)
                {
                    _socket.Close();
                    break;
                }
            }

            this.OnDisconnect?.Invoke(this, new ConnectionEventArgs(this));
        }
    }
}
