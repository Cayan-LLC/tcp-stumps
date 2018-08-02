namespace TcpStumps
{
    public interface IStumpResponseFactory 
    {
        ResponseFactoryBehavior Behavior { get; }

        int Count { get; }

        TcpResponse FailureResponse { get; set; }

        bool HasResponse { get; }

        TcpResponse Add(TcpResponse response);

        TcpResponse AddResponseMessage(byte[] message);

        void Clear();

        TcpResponse CreateResponse(byte[] message);

        void RemoveAt(int index);

        void ResetToBeginning();
    }
}
