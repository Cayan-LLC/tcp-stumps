namespace TcpStumps
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class StumpResponseFactory : IStumpResponseFactory
    {
        private readonly Random _random = new Random(Environment.TickCount);

        private readonly List<TcpResponse> _responses = new List<TcpResponse>();

        private TcpResponse _failureResponse = new TcpResponse
        {
            new TcpMessage
            {
                TerminateConnection = true
            }
        };

        private volatile int _position = -1;

        private ReaderWriterLockSlim _listLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private ReaderWriterLockSlim _positionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public StumpResponseFactory() : this(ResponseFactoryBehavior.OrderedInfinite)
        {
        }

        public StumpResponseFactory(ResponseFactoryBehavior behavior)
        {
            this.Behavior = behavior;
        }

        public StumpResponseFactory(TcpResponse failureResponse) : this(ResponseFactoryBehavior.OrderedThenFailure)
        {
            _failureResponse = failureResponse ?? throw new ArgumentNullException(nameof(failureResponse));
        }

        public StumpResponseFactory(ResponseFactoryBehavior behavior, IEnumerable<TcpResponse> responses) : this(behavior)
        {
            responses = responses ?? throw new ArgumentNullException(nameof(responses));
            _responses.AddRange(responses);
        }

        public StumpResponseFactory(TcpResponse failureResponse, IEnumerable<TcpResponse> responses) : this(failureResponse)
        {
            responses = responses ?? throw new ArgumentNullException(nameof(responses));
            _responses.AddRange(responses);
        }

        public TcpResponse this[int index]
        {
            get
            {
                try
                {
                    _listLock.EnterReadLock();
                    return _responses[index];
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _listLock.ExitReadLock();
                }
            }
        }

        public ResponseFactoryBehavior Behavior
        {
            get;
        }

        public int Count
        {
            get
            {
                _listLock.EnterReadLock();
                var count = _responses.Count;
                _listLock.ExitReadLock();

                return count;
            }
        }

        public TcpResponse FailureResponse
        {
            get
            {
                _listLock.EnterReadLock();
                var response = _failureResponse;
                _listLock.ExitReadLock();

                return response;
            }
            set
            {
                _listLock.EnterWriteLock();
                _failureResponse = value;
                _listLock.ExitWriteLock();
            }
        }

        public bool HasResponse
        {
            get
            {
                if (this.Count == 0)
                {
                    return false;
                }

                if (this.Behavior == ResponseFactoryBehavior.OrderedThenFailure &&
                    this.FailureResponse == null)
                {
                    return false;
                }

                return true;
            }
        }

        public TcpResponse Add(TcpResponse response)
        {
            response = response ?? throw new ArgumentNullException(nameof(response));

            _listLock.EnterWriteLock();
            _positionLock.EnterWriteLock();

            _responses.Add(response);
            Interlocked.Exchange(ref _position, -1);

            _listLock.ExitWriteLock();
            _positionLock.ExitWriteLock();

            return response;
        }

        public TcpResponse AddResponseMessage(byte[] message)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));
            message = message.Length > 0 ? message : throw new ArgumentOutOfRangeException(nameof(message));

            var response = new TcpResponse
            {
                new TcpMessage
                {
                    Message = message
                }
            };

            return this.Add(response);
        }

        public void Clear()
        {
            _listLock.EnterWriteLock();
            _positionLock.EnterWriteLock();

            _responses.Clear();
            Interlocked.Exchange(ref _position, -1);

            _listLock.ExitWriteLock();
            _positionLock.ExitWriteLock();
        }

        public virtual TcpResponse CreateResponse(byte[] message)
        {
            if (this.Behavior == ResponseFactoryBehavior.Random)
            {
                return ChooseRandomResponse();
            }
            else if (this.Behavior == ResponseFactoryBehavior.OrderedInfinite)
            {
                return ChooseNextResponse();
            }

            return ChooseNextResponseOrDefault();
        }

        public void RemoveAt(int index)
        {
            _listLock.EnterWriteLock();
            _positionLock.EnterWriteLock();

            try
            {
                _responses.RemoveAt(index);
                Interlocked.Exchange(ref _position, -1);
            }
            catch
            {
                throw;
            }
            finally
            {
                _listLock.ExitWriteLock();
                _positionLock.ExitWriteLock();
            }
        }

        public void ResetToBeginning()
        {
            if (this.Behavior == ResponseFactoryBehavior.Random)
            {
                return;
            }

            _positionLock.EnterWriteLock();

            Interlocked.Exchange(ref _position, -1);

            _positionLock.ExitWriteLock();
        }

        private TcpResponse ChooseNextResponse()
        {
            _positionLock.EnterWriteLock();
            _listLock.EnterReadLock();

            if (_responses.Count == 0)
            {
                _positionLock.ExitWriteLock();
                _listLock.ExitReadLock();

                return this.FailureResponse;
            }

            var index = Interlocked.Increment(ref _position);

            if (_position >= _responses.Count)
            {
                Interlocked.Exchange(ref _position, 0);
                index = 0;
            }

            _positionLock.ExitWriteLock();

            var response = _responses[index];

            _listLock.ExitReadLock();

            return response;
        }

        private TcpResponse ChooseNextResponseOrDefault()
        {
            var response = this.FailureResponse;

            _listLock.EnterReadLock();

            var index = Interlocked.Increment(ref _position);

            if (_position < _responses.Count)
            {
                response = _responses[_position];
            }

            _listLock.ExitReadLock();

            return response;
        }

        private TcpResponse ChooseRandomResponse()
        {
            _listLock.EnterReadLock();

            if (_responses.Count == 0)
            {
                _listLock.ExitReadLock();

                return this.FailureResponse;
            }

            var index = _random.Next(_responses.Count);
            var response = _responses[index];

            _listLock.ExitReadLock();

            return response;
        }
    }
}
