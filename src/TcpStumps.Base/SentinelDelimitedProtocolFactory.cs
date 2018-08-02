namespace TcpStumps
{
    using System;

    public class SentinelDelimitedProtocolFactory : IProtocolFactory
    {
        private byte[] _sentinels;
        private TcpResponse _defaultResponse;

        public SentinelDelimitedProtocolFactory() : this(new TcpResponse { new TcpMessage { TerminateConnection = true } })
        {
        }

        public SentinelDelimitedProtocolFactory(TcpResponse defaultResponse)
        {
            this.DefaultResponse = defaultResponse;
        }

        public TcpResponse DefaultResponse
        {
            get => _defaultResponse;
            set => _defaultResponse = value ?? throw new ArgumentNullException(nameof(value));
        }

        public byte[] SentinelValues
        {
            get => _sentinels;
            set => _sentinels = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int ReadPastSentinelLength
        {
            get;
            set;
        }

        public IProtocol CreateProtocol()
        {
            if (_sentinels == null || _sentinels.Length == 0)
            {
                throw new InvalidOperationException();
            }

            var protocol = new SentinelDelimitedProtocol(_defaultResponse, this.ReadPastSentinelLength, _sentinels);
            return protocol;
        }
    }
}
