namespace TcpStumps
{
    using System;
    using System.Buffers;

    internal class HeaderDefinedLengthProtocol : IProtocol
    {
        private enum ProtocolState
        {
            WaitingForHeader = 0,
            WaitingForMessage = 1
        }

        private readonly int _headerLength;
        private readonly MessageLengthCalculator _lengthCalculator;

        private int _currentMessageLength;
        private ProtocolState _currentState;

        public HeaderDefinedLengthProtocol(TcpResponse defaultResponse, int headerLength, MessageLengthCalculator lengthCalculator)
        {
            this.DefaultResponse = defaultResponse ?? throw new ArgumentNullException(nameof(defaultResponse));
            _headerLength = headerLength > 0 ? headerLength : throw new ArgumentOutOfRangeException(nameof(headerLength));
            _lengthCalculator = _lengthCalculator ?? throw new ArgumentNullException(nameof(lengthCalculator));
        }

        public TcpResponse DefaultResponse
        {
            get;
        }

        public ProtocolProcessingBehavior ProcessBufferForMessage(IConnection connection, IMessagePipeline pipeline, ref ReadOnlySequence<byte> buffer)
        {
            var pipelineResult = PipelineResult.Continue;

            var waitForLength = _currentState == ProtocolState.WaitingForHeader ? _headerLength : _currentMessageLength;

            while (buffer.Length >= waitForLength && pipelineResult == PipelineResult.Continue)
            {
                var bytes = buffer.Slice(0, waitForLength).ToArray();

                if (_currentState == ProtocolState.WaitingForHeader)
                {
                    _currentMessageLength = _lengthCalculator(bytes);
                    waitForLength = _currentMessageLength;
                    _currentState = ProtocolState.WaitingForMessage;

                    if (buffer.Length < waitForLength)
                    {
                        continue;
                    }

                    bytes = buffer.Slice(0, waitForLength).ToArray();
                }

                pipelineResult = pipeline.ProcessMessage(connection, bytes, this.DefaultResponse);

                buffer = buffer.Slice(buffer.GetPosition(waitForLength));
                _currentState = ProtocolState.WaitingForHeader;
            }

            if (pipelineResult == PipelineResult.StopAndDisconnect)
            {
                return ProtocolProcessingBehavior.Disconnect;
            }

            return ProtocolProcessingBehavior.ContinueProcessing;
        }
    }
}
