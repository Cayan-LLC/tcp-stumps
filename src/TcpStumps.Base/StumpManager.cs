namespace TcpStumps
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    internal class StumpManager
    {
        private readonly List<TcpStump> _stumpList = new List<TcpStump>();
        private readonly Dictionary<string, TcpStump> _stumpReference = new Dictionary<string, TcpStump>();
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public int Count
        {
            get => _stumpList.Count;
        }

        public void AddStump(TcpStump stump)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            if (_stumpReference.ContainsKey(stump.StumpId))
            {
                throw new ArgumentException();
            }

            _lock.EnterWriteLock();

            _stumpList.Add(stump);
            _stumpReference.Add(stump.StumpId, stump);

            _lock.ExitWriteLock();
        }

        public void DeleteAll()
        {
            _lock.EnterWriteLock();

            _stumpReference.Clear();
            _stumpList.Clear();

            _lock.ExitWriteLock();
        }

        public void DeleteStump(string stumpId)
        {
            _lock.EnterWriteLock();

            if (_stumpReference.ContainsKey(stumpId))
            {
                var stump = _stumpReference[stumpId];
                _stumpReference.Remove(stumpId);
                _stumpList.Remove(stump);
            }

            _lock.ExitWriteLock();
        }

        public TcpStump FindStump(string stumpId)
        {
            _lock.EnterReadLock();

            TcpStump stump;

            try
            {
                stump = _stumpReference[stumpId];
            }
            finally
            {
                _lock.ExitReadLock();
            }

            return stump;
        }

        public TcpStump FindStumpForMessage(byte[] message)
        {
            TcpStump foundStump = null;

            _lock.EnterReadLock();

            foreach (var stump in _stumpList)
            {
                if (stump.IsMatch(message))
                {
                    foundStump = stump;
                    break;
                }
            }

            _lock.ExitReadLock();

            return foundStump;
        }
    }
}
