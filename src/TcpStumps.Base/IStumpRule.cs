namespace TcpStumps
{
    public interface IStumpRule
    {
        bool IsMatch(byte[] message);
    }
}
