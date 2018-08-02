namespace TcpStumps
{
    internal interface IMessageHandler
    {
        PipelineResult ProcessRequest(ITcpContext context);
    }
}
