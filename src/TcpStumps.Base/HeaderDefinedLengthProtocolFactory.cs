namespace TcpStumps
{
    using System;

    public class HeaderDefinedLengthProtocolFactory : IProtocolFactory
    {
        private int _headerLength;
        private MessageLengthCalculator _lengthCalculator;
        private TcpResponse _defaultResponse;

        public HeaderDefinedLengthProtocolFactory() : this(new TcpResponse { new TcpMessage { TerminateConnection = true } })
        {
        }

        public HeaderDefinedLengthProtocolFactory(TcpResponse defaultResponse)
        {
            this.DefaultResponse = defaultResponse;
        }

        public TcpResponse DefaultResponse
        {
            get => _defaultResponse;
            set => _defaultResponse = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int HeaderLength
        {
            get => _headerLength;
            set => _headerLength = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        public MessageLengthCalculator LengthCalculator
        {
            get => _lengthCalculator;
            set => _lengthCalculator = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TcpResponse ResponseOnConnection
        {
            get;
            set;
        }

        public IProtocol CreateProtocol()
        {
            if (_headerLength < 1 || _lengthCalculator == null)
            {
                throw new InvalidOperationException();
            }

            var protocol = new HeaderDefinedLengthProtocol(_defaultResponse, this.ResponseOnConnection, this.HeaderLength, this.LengthCalculator);
            return protocol;
        }
    }
}
