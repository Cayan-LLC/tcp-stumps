namespace TcpStumps
{
    public static class FluentRuleExtensions
    {
        public static TcpStump WhenLengthIsEqualTo(this TcpStump stump, int value)
        {
            stump.AddRule(new MessageLengthRule(LengthEvaluation.EqualToValue, value));
            return stump;
        }

        public static TcpStump WhenLengthIsGreaterThan(this TcpStump stump, int value)
        {
            stump.AddRule(new MessageLengthRule(LengthEvaluation.GreaterThanValue, value));
            return stump;
        }

        public static TcpStump WhenLengthIsLessThan(this TcpStump stump, int value)
        {
            stump.AddRule(new MessageLengthRule(LengthEvaluation.LessThanValue, value));
            return stump;
        }

        public static TcpStump WhenMessageIsAnything(this TcpStump stump)
        {
            stump.AddRule(new AnyMessageRule());
            return stump;
        }

        public static TcpStump WhenMessageContainsBytes(this TcpStump stump, byte[] messageSegment)
        {
            stump.AddRule(new ContainsBytesRule(messageSegment));
            return stump;
        }

        public static TcpStump WhenMessageContainsBytesAfterPosition(this TcpStump stump, byte[] messageSegment, int position)
        {
            stump.AddRule(new ContainsBytesRule(messageSegment, BytesPlacement.AfterPosition, position));
            return stump;
        }

        public static TcpStump WhenMessageContainsBytesAtPosition(this TcpStump stump, byte[] messageSegment, int position)
        {
            stump.AddRule(new ContainsBytesRule(messageSegment, BytesPlacement.ExactPosition, position));
            return stump;
        }

        public static TcpStump WhenMessageContainsBytesBeforePosition(this TcpStump stump, byte[] messageSegment, int position)
        {
            stump.AddRule(new ContainsBytesRule(messageSegment, BytesPlacement.BeforePosition, position));
            return stump;
        }
    }
}
