namespace TcpStumps
{
    using System;

    public class FixedLengthProtocolFactory : IProtocolFactory
    {
        private int _messageLength;
        private TcpResponse _defaultResponse;

        public FixedLengthProtocolFactory() : this(new TcpResponse { new TcpMessage { TerminateConnection = true } })
        {
        }

        public FixedLengthProtocolFactory(TcpResponse defaultResponse)
        {
            this.DefaultResponse = defaultResponse;
        }

        public TcpResponse DefaultResponse
        {
            get => _defaultResponse;
            set => _defaultResponse = value ?? throw new ArgumentNullException(nameof(value)); 
        }

        public int MessageLength
        {
            get => _messageLength;
            set => _messageLength = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        public TcpResponse ResponseOnConnection
        {
            get;
            set;
        }

        public IProtocol CreateProtocol()
        {
            if (_messageLength < 1)
            {
                throw new InvalidOperationException();
            }

            return new FixedLengthProtocol(_defaultResponse, this.ResponseOnConnection, _messageLength);
        }
    }
}
