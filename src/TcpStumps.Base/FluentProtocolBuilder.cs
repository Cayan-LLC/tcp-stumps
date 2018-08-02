namespace TcpStumps
{
    public class FluentProtocolBuilder
    {
        private TcpStumpsServer _server;

        internal FluentProtocolBuilder(TcpStumpsServer server)
        {
            _server = server;
        }

        public IProtocolFactory WithFixedLengthMessages(int messageLength)
        {
            var protocol = new FixedLengthProtocolFactory()
            {
                MessageLength = messageLength
            };

            _server.Protocol = protocol;

            return protocol;
        }

        public IProtocolFactory WithSentinelsToDeliminateMessages(params byte[] sentinels)
        {
            var protocol = new SentinelDelimitedProtocolFactory()
            {
                SentinelValues = sentinels
            };

            _server.Protocol = protocol;

            return protocol;
        }

        public IProtocolFactory WithSentinelsToDeliminateMessages(int readPastSentinelLength, params byte[] sentinels)
        {
            var protocol = new SentinelDelimitedProtocolFactory()
            {
                SentinelValues = sentinels,
                ReadPastSentinelLength = readPastSentinelLength
            };

            _server.Protocol = protocol;

            return protocol;
        }

        public IProtocolFactory WithHeadersContainingTheMessageLength(int headerLength, MessageLengthCalculator messageLengthCalculator)
        {
            var protocol = new HeaderDefinedLengthProtocolFactory()
            {
                HeaderLength = headerLength,
                LengthCalculator = messageLengthCalculator
            };

            _server.Protocol = protocol;

            return protocol;
        }
    }
}
