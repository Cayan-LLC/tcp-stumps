namespace TcpStumps
{
    public interface ITcpContext
    {
        IConnection Connection
        {
            get;
        }

        byte[] RequestMessage
        {
            get;
        }


        TcpResponse Response
        {
            get;
        }
    }
}
