namespace TcpStumps
{
    public class MessageLengthRule : IStumpRule
    {
        public MessageLengthRule(LengthEvaluation evaluation, int value)
        {
            this.Evaluation = evaluation;
            this.Value = value;
        }

        public int Value
        {
            get;
            set;
        }

        public LengthEvaluation Evaluation
        {
            get;
            set;
        }

        public bool IsMatch(byte[] message)
        {
            if (message == null)
            {
                return false;
            }

            switch (this.Evaluation)
            {
                case LengthEvaluation.EqualToValue:
                    return message.Length == this.Value;

                case LengthEvaluation.GreaterThanValue:
                    return message.Length > this.Value;

                case LengthEvaluation.LessThanValue:
                    return message.Length < this.Value;
            }

            return false;
        }
    }
}
