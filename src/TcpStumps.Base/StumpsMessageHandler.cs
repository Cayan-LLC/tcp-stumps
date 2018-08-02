namespace TcpStumps
{
    using System;

    internal class StumpsMessageHandler : IMessageHandler
    {
        private StumpManager _stumpManager;

        public StumpsMessageHandler(StumpManager manager)
        {
            _stumpManager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public PipelineResult ProcessRequest(ITcpContext context)
        {
            var stump = _stumpManager.FindStumpForMessage(context.RequestMessage);

            if (stump == null)
            {
                return PipelineResult.Continue;
            }

            var response = stump.Responses.CreateResponse(context.RequestMessage);
            response.ForEach((r) => context.Response.Add(r));

            return PipelineResult.Stop;
        }
    }
}
