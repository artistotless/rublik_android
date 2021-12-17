using System;
using System.Threading;
using System.Threading.Tasks;
using CrossPlatformLiveData;
using LiteNetLib;
using LiteNetLib.Utils;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Games
{
    public enum GameServerEvent : ushort
    {
        waitingForConnecting,
        init,
        waitingForReconnecting,
        ready,
        canceled,
        finished,
        chat
    }

    public class GameServer : IDisposable
    {
        private EventBasedNetListener _listener;
        private NetManager _client;
        private BasePlayer _player;
        public static GameServer currentInstance { get; set; }
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        private LiveData<NetPacketReader> _liveData = new LiveData<NetPacketReader>();
        private NetPeer _gameServerPeer;

        public GameServer()
        {
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);
            currentInstance = this;
        }

        public void SetListener(IGameEventListener listener) => listener.OnSubscribedOnService(_liveData, this);

        public void Connect(BasePlayer player, ServerEndpoint endpoint)
        {
            currentInstance = this;
            _client.Start();
            NetDataWriter initPacket = new NetDataWriter();
            initPacket.Put(player.extraData.username);
            initPacket.Put(player.extraData.accessKey);

            _gameServerPeer = _client.Connect(endpoint.ip, endpoint.port, initPacket);

            CancellationToken canselToken = _cancelTokenSource.Token;


            _listener.NetworkReceiveEvent += (peer, dataReader, deliveryMethod) =>
        {
            _liveData.PostValue(dataReader);
        };

            _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
                    {
                        Console.WriteLine(
                            "\n" +
                            $" Disconnect of ShellGameServer \n" +
                            $" Peer: {peer.EndPoint} \n" +
                            $" Disconnect Info: {disconnectInfo.Reason} \n"
                            );

                        _liveData.PostValue(disconnectInfo.AdditionalData);
                    };

            Task.Run(async delegate
                    {
                        while (!canselToken.IsCancellationRequested)
                        {
                            _client.PollEvents();
                            await Task.Delay(500);
                        }
                        _gameServerPeer.Disconnect();
                        _client.Stop();
                    });
        }
        public void Send(NetDataWriter writer, DeliveryMethod deliveryMethod) => _gameServerPeer.Send(writer, deliveryMethod);

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