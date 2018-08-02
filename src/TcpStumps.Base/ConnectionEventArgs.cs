namespace TcpStumps
{
    using System;

    public class ConnectionEventArgs : EventArgs
    {
        internal ConnectionEventArgs(IConnection connection) => this.Connection = connection;

        public IConnection Connection
        {
            get;
        }
    }
}
