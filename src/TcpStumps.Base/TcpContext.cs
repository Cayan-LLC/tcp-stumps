namespace TcpStumps
{
    using System;

    public class TcpContext : ITcpContext
    {
        internal TcpContext(IConnection connection, byte[] requestMessage)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.RequestMessage = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage));
        }

        public IConnection Connection
        {
            get;
        }

        public byte[] RequestMessage
        {
            get;
        }

        public TcpResponse Response
        {
            get;
        } = new TcpResponse();
    }
}
