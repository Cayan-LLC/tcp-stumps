namespace TcpStumps
{
    public class TcpMessage
    {
        public byte[] Message
        {
            get;
            set;
        }

        public int ResponseDelay
        {
            get;
            set;
        }

        public bool TerminateConnection
        {
            get;
            set;
        }
    }
}
