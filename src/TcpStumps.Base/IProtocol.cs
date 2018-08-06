namespace TcpStumps
{
    using System.Buffers;

    public interface IProtocol
    {
        TcpResponse DefaultResponse
        {
            get;
        }

        TcpResponse ResponseOnConnection
        {
            get;
            set;
        }

        ProtocolProcessingBehavior ProcessBufferForMessage(IConnection connection, IMessagePipeline pipeline, ref ReadOnlySequence<byte> bufferWrapper);
    }
}
