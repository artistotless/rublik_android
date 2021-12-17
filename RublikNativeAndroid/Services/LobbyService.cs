using LiteNetLib;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib.Utils;
using RublikNativeAndroid.Contracts;

using CrossPlatformLiveData;
using System;

namespace RublikNativeAndroid.Services
{
    enum ActionRequest : ushort
    {
        HostRoom,
        GetRooms,
        LeaveRoom,
        JoinRoom,
        MessageToRoom,
        StartGame
    }

    enum EventOption : ushort
    {
        HostedRoom,
        JoinedRoom,
        LeavedRoom,
        MessageRoom,
        GotRoomList,
        DeletedRoom,
        HostOfRoomChanged,
        GameStarted
    }

    public class LobbyService : IService, IDisposable
    {
        public static LobbyService currentInstance;

        private NetPeer _lobbyServicePeer;
        private EventBasedNetListener _listener;
        private NetManager _client;
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        private LiveData<NetPacketReader> _liveData = new LiveData<NetPacketReader>();

        public LobbyService() => currentInstance = this;

        public void SetListener(IRoomEventListener listener) => listener.OnSubscribedOnService(_liveData, this);

        public void Connect(string accessKey)
        {
            LobbyService.currentInstance = this;
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);
            CancellationToken cancelToken = _cancelTokenSource.Token;
            _client.Start();

            NetDataWriter initPacket = new NetDataWriter();
            initPacket.Put(accessKey);
            // TODO: заменить ip сервиса на домен вида l1s.rublik.ru . Использовать DNS сервера
            _lobbyServicePeer = _client.Connect(Constants.Services.LOBBY_IP, Constants.Services.LOBBY_PORT, initPacket);


            _listener.NetworkReceiveEvent += (peer, dataReader, deliveryMethod) =>
            {
                Console.WriteLine($"Lobby Service : NetworkReceiveEvent THREAD # {Thread.CurrentThread.ManagedThreadId}");
                _liveData.PostValue(dataReader);
            };

            _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                _liveData.PostValue(disconnectInfo.AdditionalData);
            };


            Task.Run(async delegate
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    _client.PollEvents();
                    await Task.Delay(400);
                }
                _lobbyServicePeer.Disconnect();
                _client.Stop();
                Console.WriteLine($"Lobby Service : Disconnect THREAD # {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine($"Lobby Service : ConnectionState - {_lobbyServicePeer.ConnectionState}");
            });

        }

        public void Send(NetDataWriter writer, DeliveryMethod deliveryMethod) => _lobbyServicePeer.Send(writer, deliveryMethod);

        public void Disconnect()
        {
            _cancelTokenSource.Cancel();             
            currentInstance = null;
        }

        public void Dispose()
        {
            Disconnect();
            _client = null;
            _listener = null;
            _liveData.Value.Clear();
        }
    }
}
