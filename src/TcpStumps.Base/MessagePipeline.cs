namespace TcpStumps
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class MessagePipeline : IMessagePipeline, IMessagePipelineInternal
    {
        public const int MaximumResponseDelay = 120000;
        public const int MinimumResponseDelay = 0;

        private List<IMessageHandler> _handlers = new List<IMessageHandler>();

        public PipelineResult ProcessMessage(IConnection connection, byte[] message, TcpResponse defaultResponse)
        {
            var context = new TcpContext(connection, message);

            var result = ExecuteHandlers(context);

            if (result == PipelineResult.StopAndDisconnect)
            {
                return result;
            }

            if (context.Response.Count == 0)
            {
                defaultResponse.ForEach((r) => context.Response.Add(r));
            }

            result = SendResponseMessages(context);

            return result;
        }

        public void AddHandler(IMessageHandler handler) => _handlers.Add(handler);

        private PipelineResult ExecuteHandlers(ITcpContext context)
        {
            var result = PipelineResult.Continue;

            foreach (var handler in _handlers)
            {
                result = handler.ProcessRequest(context);

                if (result != PipelineResult.Continue)
                {
                    break;
                }
            }

            return result;
        }

        private PipelineResult SendResponseMessages(ITcpContext context)
        {
            var result = PipelineResult.Continue;

            foreach (var message in context.Response)
            {
                var delay = message.ResponseDelay;
                delay = delay < MessagePipeline.MaximumResponseDelay ? delay : MessagePipeline.MaximumResponseDelay;
                delay = delay > MessagePipeline.MinimumResponseDelay ? delay : MessagePipeline.MinimumResponseDelay;

                if (delay > 0)
                {
                    Task.WaitAll(Task.Delay(delay));
                }

                if (message.Message?.Length > 0)
                {
                    Task.WaitAll(context.Connection.SendAsync(message.Message));
                }

                if (message.TerminateConnection)
                {
                    result = PipelineResult.StopAndDisconnect;
                    break;
                }
            }

            return result;
        }

    }
}
