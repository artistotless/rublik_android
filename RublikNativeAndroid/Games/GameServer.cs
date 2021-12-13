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

    public class GameServer : BaseGameServer, IDisposable
    {
        private LiveData<NetPacketReader> _liveData = new LiveData<NetPacketReader>();

        public GameServer(BasePlayer player, string addr, int port) : base(player, addr, port) { }

        public void SetListener(IGameEventListener listener) => listener.OnSubscribedGameEvents(_liveData);

        public void Start()
        {
            client.Start();
            NetDataWriter initPacket = new NetDataWriter();
            initPacket.Put(player.extraData.username);
            initPacket.Put(player.extraData.accessKey);

            Connect(addr, port, initPacket);

            CancellationToken canselToken = cancelTokenSource.Token;


            listener.NetworkReceiveEvent += (peer, dataReader, deliveryMethod) =>
        {
            _liveData.PostValue(dataReader);
        };

            listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
                    {
                        Console.WriteLine(
                            "\n" +
                            $" Disconnect of ShellGameServer \n" +
                            $" Peer: {peer.EndPoint} \n" +
                            $" Disconnect Info: {disconnectInfo.Reason} \n"
                            );

                        _liveData.PostValue(disconnectInfo.AdditionalData);
                    };

            Task.Factory.StartNew(async delegate
                    {
                        while (!canselToken.IsCancellationRequested)
                        {
                            client.PollEvents();
                            await Task.Delay(500);
                        }
                    }, canselToken);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}