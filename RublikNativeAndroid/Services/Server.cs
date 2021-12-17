using LiteNetLib;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib.Utils;
using RublikNativeAndroid.Contracts;
using CrossPlatformLiveData;
using System;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{
    public enum ServerType { Messenger, Game, Lobby }

    public class Server : IServer, IDisposable
    {
        public static Server currentInstance;

        private NetPeer _serverPeer;
        private EventBasedNetListener _listener;
        private NetManager _client;
        private string _accessKey;
        private LiveData<NetPacketReader> _liveData;
        private ServerType _lastServerType;
        private CancellationTokenSource _cancelTokenSource;


        public Server(string accessKey) => _accessKey = accessKey;

        public void SetListener(IServerListener listener)
        {
            listener.OnSubscribedOnServer(_liveData, this);
        }

        private async Task DisconnectAsync()
        {
            await Task.Run(delegate { HardDisconnect(); });
        }

        public async Task ConnectAsync(ServerEndpoint endpoint)
        {
            if (_serverPeer != null)
            {
                if (_serverPeer.ConnectionState == ConnectionState.Connected && endpoint.type == _lastServerType)
                    return;
                if (_serverPeer.ConnectionState == ConnectionState.Connected)
                    await DisconnectAsync();
            }

            try
            {
                Console.WriteLine($"CURRENT_TYPE: {endpoint.type} \n LAST_TYPE: {_lastServerType}");
                Console.WriteLine($"++++ HASH CODE CancelToken ------> {_cancelTokenSource.Token.GetHashCode()}");
            }
            catch { Console.WriteLine($"++++ HASH CODE CancelToken ------> NULL"); }
            currentInstance = this;
            _lastServerType = endpoint.type;
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);
            _cancelTokenSource = new CancellationTokenSource();
            _liveData = new LiveData<NetPacketReader>();
            _client.Start();

            NetDataWriter initPacket = new NetDataWriter();
            initPacket.Put(_accessKey);
            _serverPeer = _client.Connect(endpoint.ip, endpoint.port, initPacket);


            _listener.NetworkReceiveEvent += (peer, dataReader, deliveryMethod) =>
            {
                Console.WriteLine($"Server : NetworkReceiveEvent THREAD # {Thread.CurrentThread.ManagedThreadId}");
                _liveData.PostValue(dataReader);
            };

            _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                _liveData.PostValue(disconnectInfo.AdditionalData);
            };

            StartPollEvents();

        }
        private void StartPollEvents()
        {
            Task.Run(async delegate
            {
                while (!_cancelTokenSource.Token.IsCancellationRequested)
                {
                    _client.PollEvents();
                    await Task.Delay(200);
                }
                Console.WriteLine($"++++ HASH CODE CancelToken ------> {_cancelTokenSource.Token.GetHashCode()}");
                _serverPeer.Disconnect();
                _client.Stop();
                Console.WriteLine($"Server : Disconnect THREAD # {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine($"Server : ConnectionState - {_serverPeer.ConnectionState}");
            });
        }

        public void Send(NetDataWriter writer, DeliveryMethod deliveryMethod) => _serverPeer.Send(writer, deliveryMethod);

        public void Disconnect()
        {
            _cancelTokenSource.Cancel();
            currentInstance = null;
        }

        public void HardDisconnect()
        {
            _serverPeer.Disconnect();
            _client.Stop();
        }

        public void Dispose()
        {
            //Disconnect();
            //_client = null;
            //_serverPeer = null;
            //_listener = null;
            //_liveData = null;
        }
    }
}
