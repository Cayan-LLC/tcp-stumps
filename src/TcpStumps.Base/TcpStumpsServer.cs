namespace TcpStumps
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class TcpStumpsServer
    {
        private int _port;
        private readonly StumpManager _stumpManager = new StumpManager();
        private volatile bool _started;
        private TcpServer _server;
        private bool _allowExternalConnection;
        private IProtocolFactory _protocolFactory;

        public TcpStumpsServer()
        {
            this.ListeningPort = PortAvailability.FindRandomOpenPort();
        }

        public event EventHandler<ConnectionEventArgs> OnClientConnection;

        public event EventHandler<ConnectionEventArgs> OnClientDisconnect;

        public bool AllowExternalConnection
        {
            get => _allowExternalConnection;
            set
            {
                if (this.IsRunning)
                {
                    throw new InvalidOperationException(BaseResources.ServerIsRunning);
                }

                _allowExternalConnection = value;
            }
        }

        public bool IsRunning
        {
            get => _started;
        }

        public int ListeningPort
        {
            get => _port;
            set
            {
                if (this.IsRunning)
                {
                    throw new InvalidOperationException(BaseResources.ServerIsRunning);
                }

                if (value < IPEndPoint.MinPort || value > IPEndPoint.MaxPort)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _port = value;
            }
        }

        public IProtocolFactory Protocol
        {
            get => _protocolFactory;
            set
            {
                if (this.IsRunning)
                {
                    throw new InvalidOperationException(BaseResources.ServerIsRunning);
                }

                _protocolFactory = value ?? throw new ArgumentNullException(nameof(value));
            } 
        }

        public int StumpCount => _stumpManager.Count;

        public TcpStump AddNewStump(string stumpId)
        {
            var stump = new TcpStump(stumpId);
            _stumpManager.AddStump(stump);
            return stump;
        }

        public void DeleteAllStumps() => _stumpManager.DeleteAll();

        public void DeleteStump(string stumpId) => _stumpManager.DeleteStump(stumpId);

        public TcpStump FindStump(string stumpId) => _stumpManager.FindStump(stumpId);

        public async Task StartAsync()
        {
            if (_started)
            {
                return;
            }

            _started = true;

            if (_protocolFactory == null)
            {
                _started = false;
                throw new InvalidOperationException();
            }

            var endpoint = this.AllowExternalConnection ?
                new IPEndPoint(IPAddress.Any, _port)
                : new IPEndPoint(IPAddress.Loopback, _port);

            var server = new TcpServer(_protocolFactory, endpoint);
            ((IMessagePipelineInternal)server.Pipline).AddHandler(new StumpsMessageHandler(_stumpManager));

            server.OnClientConnection += (o, e) => this.OnClientConnection?.Invoke(o, e);
            server.OnClientDisconnect += (o, e) => this.OnClientDisconnect?.Invoke(o, e);

            await server.StartAsync();
        }

        public async Task ShutdownAsync()
        {
            if (!_started)
            {
                return;
            }

            try
            {
                _started = false;
                await _server.StopAsync();
                _server.Dispose();
                _server = null;
            }
            catch
            {

            }
        }
    }
}
