namespace TcpStumps
{
    using System.Buffers;

    public interface IProtocol
    {
        TcpResponse DefaultResponse
        {
            get;
        }

        ProtocolProcessingBehavior ProcessBufferForMessage(IConnection connection, IMessagePipeline pipeline, ref ReadOnlySequence<byte> bufferWrapper);
    }
}
