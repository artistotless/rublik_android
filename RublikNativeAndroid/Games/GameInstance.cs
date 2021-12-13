using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Games
{
    public enum GameStatus : ushort
    {
        waitingForConnecting,
        init,
        waitingForReconnecting,
        ready,
        canceled,
        finished,
        chat
    }

    public class GameInstance
    {
        public EventBasedNetListener listener;
        public NetManager client;
        public GameStatus status;
        public BasePlayer player;
        public string addr;
        public int port;
        public static GameInstance currentGame { get; set; }
        public CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        private NetPeer _gameServerPeer;

        public void Connect(string addr, int port, NetDataWriter initPacket)
        {
            _gameServerPeer = client.Connect(addr, port, initPacket);
        }

        public GameInstance(BasePlayer player, string addr, int port)
        {
            listener = new EventBasedNetListener();
            client = new NetManager(listener);
            this.player = player;
            this.addr = addr;
            this.port = port;
            currentGame = this;
        }
        private GameInstance() { }

        public void Send(NetDataWriter writer, DeliveryMethod deliveryMethod) => _gameServerPeer.Send(writer, deliveryMethod);
    }

}