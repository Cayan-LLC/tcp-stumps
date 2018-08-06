namespace TcpStumps
{
    public static class FluentResponseExtensions
    {
        public static IStumpResponseFactory RespondWith(this TcpStump stump)
        {
            return stump.Responses;
        }

        public static IStumpResponseFactory RespondsWithMultipleOptions(this TcpStump stump, ResponseFactoryBehavior behavior)
        {
            stump.Responses = stump.Responses ?? new StumpResponseFactory(behavior);
            return stump.Responses;
        }

        public static IStumpResponseFactory TheMessage(this IStumpResponseFactory responseFactory, byte[] message)
        {
            responseFactory.AddResponseMessage(message);
            return responseFactory;
        }


        public static IStumpResponseFactory DropConnection(this IStumpResponseFactory responseFactory)
        {
            responseFactory.Add(new TcpResponse
            {
                new TcpMessage
                {
                    TerminateConnection = true
                }
            });

            return responseFactory;
        }
        
        public static IStumpResponseFactory Nothing(this IStumpResponseFactory responseFactory)
        {
            responseFactory.Add(new TcpResponse
            {
                new TcpMessage()
            });

            return responseFactory;
        }

        public static IStumpResponseFactory TheDelayedMessage(this IStumpResponseFactory responseFactory, byte[] message, int delayTime)
        {
            responseFactory.Add(new TcpResponse
            {
                new TcpMessage
                {
                    Message = message,
                    ResponseDelay = delayTime
                }
            });

            return responseFactory;
        }

        public static TcpResponse MultipleMessages(this IStumpResponseFactory responseFactory)
        {
            return responseFactory.Add(new TcpResponse());
        }

        public static TcpResponse DropConnection(this TcpResponse response)
        {
            response.Add(new TcpMessage
            {
                TerminateConnection = true
            });

            return response;
        }

        public static TcpResponse TheMessage(this TcpResponse response, byte[] message)
        {
            response.Add(new TcpMessage
            {
                Message = message
            });

            return response;
        }

        public static TcpResponse TheDelayedMessage(this TcpResponse response, byte[] message, int delayTime)
        {
            response.Add(new TcpMessage
            {
                Message = message,
                ResponseDelay = delayTime
            });

            return response;
        }
    }
}
