namespace TcpStumps
{
    using System;
    using System.Buffers;

    internal class SentinelDelimitedProtocol : IProtocol
    {
        private readonly byte[] _sentinels;
        private int _readPastLength;

        public SentinelDelimitedProtocol(TcpResponse defaultResponse, int readPastLength, params byte[] sentinels)
        {
            this.DefaultResponse = defaultResponse ?? throw new ArgumentNullException(nameof(defaultResponse));
            _sentinels = sentinels ?? throw new ArgumentNullException(nameof(sentinels));
            _readPastLength = readPastLength;
        }

        public TcpResponse DefaultResponse
        {
            get;
        }

        public ProtocolProcessingBehavior ProcessBufferForMessage(IConnection connection, IMessagePipeline pipeline, ref ReadOnlySequence<byte> buffer)
        {
            SequencePosition? position = null;
            var pipelineResult = PipelineResult.Continue;

            do
            {
                position = buffer.PositionOfAny(_sentinels);

                if (position == null)
                {
                    continue;
                }

                pipelineResult = pipeline.ProcessMessage(connection, buffer.Slice(0, position.Value).ToArray(), this.DefaultResponse);

                buffer = buffer.Slice(buffer.GetPosition(1 + _readPastLength, position.Value));
            } while (position != null && pipelineResult == PipelineResult.Continue);

            if (pipelineResult == PipelineResult.StopAndDisconnect)
            {
                return ProtocolProcessingBehavior.Disconnect;
            }

            return ProtocolProcessingBehavior.ContinueProcessing;
        }
    }
}
