namespace TcpStumps
{
    public interface IProtocolFactory
    {
        TcpResponse DefaultResponse
        {
            get;
            set;
        }

        IProtocol CreateProtocol();
    }
}
