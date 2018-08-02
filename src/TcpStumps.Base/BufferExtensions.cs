namespace TcpStumps
{
    using System;
    using System.Runtime.InteropServices;
    using System.Buffers;

    internal static class BufferExtensions
    {
        public static ArraySegment<byte> GetArray(this Memory<byte> memory)
        {
            return ((ReadOnlyMemory<byte>)memory).GetArray();
        }

        public static ArraySegment<byte> GetArray(this ReadOnlyMemory<byte> memory)
        {
            if (!MemoryMarshal.TryGetArray(memory, out var result))
            {
                throw new InvalidOperationException("Buffer backed by array was expected");
            }
            return result;
        }

        public static SequencePosition? PositionOfAny<T>(in this ReadOnlySequence<T> source, params T[] values) where T : IEquatable<T>
        {
            SequencePosition? position = null;

            foreach (var value in values)
            {
                position = source.PositionOf(value);

                if (position != null)
                {
                    break;
                }
            }

            return position;
        }
    }
}
