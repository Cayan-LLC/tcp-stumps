namespace TcpStumps
{
    public interface IMessagePipeline
    {
        PipelineResult ProcessMessage(IConnection connection, byte[] clientMessage, TcpResponse defaultResponse);
    }
}
