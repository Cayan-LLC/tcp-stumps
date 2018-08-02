namespace TcpStumps
{
    using System;
    using System.Threading.Tasks;

    public interface IConnection
    {
        Guid ConnectionId { get; }

        Task<int> SendAsync(byte[] buffer);

        Task<int> SendAsync(byte[] buffer, int offset, int count);

        Task<int> SendAsync(ArraySegment<byte> buffer);
    }
}
