namespace TcpStumps
{
    using System;
    using System.Buffers;

    internal class FixedLengthProtocol : IProtocol
    {
        private readonly int _messageLength;

        public FixedLengthProtocol(TcpResponse defaultResponse, TcpResponse responseOnConnection, int messageLength)
        {
            this.DefaultResponse = defaultResponse ?? throw new ArgumentNullException(nameof(defaultResponse));
            this.ResponseOnConnection = responseOnConnection;

            _messageLength = messageLength > 0 ? messageLength : throw new ArgumentOutOfRangeException(nameof(messageLength));
        }

        public TcpResponse DefaultResponse
        {
            get;
        }

        public TcpResponse ResponseOnConnection
        {
            get;
            set;
        }

        public ProtocolProcessingBehavior ProcessBufferForMessage(IConnection connection, IMessagePipeline pipeline, ref ReadOnlySequence<byte> buffer)
        {
            var pipelineResult = PipelineResult.Continue;

            while (buffer.Length >= _messageLength && pipelineResult == PipelineResult.Continue)
            {
                pipelineResult = pipeline.ProcessMessage(connection, buffer.Slice(0, _messageLength).ToArray(), this.DefaultResponse);

                buffer = buffer.Slice(buffer.GetPosition(_messageLength));
            }

            if (pipelineResult == PipelineResult.StopAndDisconnect)
            {
                return ProtocolProcessingBehavior.Disconnect;
            }

            return ProtocolProcessingBehavior.ContinueProcessing;
        }
    }
}
