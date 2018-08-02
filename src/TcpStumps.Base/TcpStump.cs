namespace TcpStumps
{
    using System;
    using System.Collections.Generic;

    public sealed class TcpStump
    {
        private readonly List<IStumpRule> _ruleList = new List<IStumpRule>();
        private IStumpResponseFactory _responseFactory = new StumpResponseFactory();

        public TcpStump(string stumpId)
        {
            this.StumpId = !string.IsNullOrWhiteSpace(stumpId) ? stumpId : throw new ArgumentNullException(nameof(stumpId));
        }

        public int Count => _ruleList.Count;

        public IStumpResponseFactory Responses
        {
            get => _responseFactory;
            set => _responseFactory = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string StumpId
        {
            get;
            private set;
        }

        public void AddRule(IStumpRule rule)
        {
            rule = rule ?? throw new ArgumentNullException(nameof(rule));
            _ruleList.Add(rule);
        }

        public bool IsMatch(byte[] message)
        {
            if (message == null
                || message.Length == 0
                || _responseFactory.HasResponse == false
                || _ruleList.Count == 0)
            {
                return false;   
            }

            var match = true;

            foreach (var rule in _ruleList)
            {
                match &= rule.IsMatch(message);

                if (!match)
                {
                    break;
                }
            }

            return match;
        }
    }
}
