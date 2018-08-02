namespace TcpStumps
{
    using System;

    public class ContainsBytesRule : IStumpRule
    {
        public ContainsBytesRule(byte[] messageSegment) : this(messageSegment, BytesPlacement.AnyPosition, -1)
        {
        }

        public ContainsBytesRule(byte[] messageSegment, BytesPlacement placement, int position)
        {
            if (messageSegment == null || messageSegment.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(messageSegment));
            }

            this.MessageSegment = messageSegment;
            this.Placement = placement;
            this.ExpectedPosition = position;
        }

        public byte[] MessageSegment
        {
            get;
            set;
        }

        public BytesPlacement Placement
        {
            get;
            set;
        }

        public int ExpectedPosition
        {
            get;
            set;
        }

        public ContainsBytesRule()
        {

        }

        public bool IsMatch(byte[] message)
        {
            if (message == null)
            {
                return false;   
            }

            var position = IndexOfSegment(message);

            if (position == -1)
            {
                return false;
            }

            switch (this.Placement)
            {
                case BytesPlacement.AnyPosition:
                    return true;

                case BytesPlacement.ExactPosition:
                    return position == this.ExpectedPosition;

                case BytesPlacement.BeforePosition:
                    return position < this.ExpectedPosition;

                case BytesPlacement.OnOrBeforePosition:
                    return position <= this.ExpectedPosition;

                case BytesPlacement.AfterPosition:
                    return position > this.ExpectedPosition;

                case BytesPlacement.OnOrAfterPosition:
                    return position >= this.ExpectedPosition;
            }

            return false;
        }

        private int IndexOfSegment(byte[] message)
        {
            if (message.Length < this.MessageSegment.Length)
            {
                return -1;
            }

            for (var i = 0; i < message.Length; i++)
            {
                var found = true;

                for (var j = 0; j < this.MessageSegment.Length; j++)
                {
                    if (message[i + j] != this.MessageSegment[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
