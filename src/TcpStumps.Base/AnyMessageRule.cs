namespace TcpStumps
{
    public class AnyMessageRule : IStumpRule
    {
        public bool IsMatch(byte[] message)
        {
            return true;
        }
    }
}
