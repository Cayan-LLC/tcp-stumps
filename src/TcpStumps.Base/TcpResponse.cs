namespace TcpStumps
{
    using System.Collections.Generic;

    public class TcpResponse : List<TcpMessage> 
    {
        public TcpResponse()
        {
        }

        public TcpResponse(IEnumerable<TcpMessage> collection) : base(collection)
        {
        }
    }
}
