namespace TcpStumps
{
    using System.Buffers;

    public class BufferWrapper
    {
        public BufferWrapper(ReadOnlySequence<byte> buffer)
        {
            this.Buffer = buffer;
        }

        public ReadOnlySequence<byte> Buffer
        {
            get;
            set;
        }
    }
}
