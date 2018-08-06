namespace TcpStumps
{
    public interface IProtocolFactory
    {
        TcpResponse DefaultResponse
        {
            get;
            set;
        }

        TcpResponse ResponseOnConnection
        {
            get;
            set;
        }

        IProtocol CreateProtocol();
    }
}
