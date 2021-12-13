using CrossPlatformLiveData;
using LiteNetLib;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{

    public interface IGameEventListener
    {
        public void OnWaitingPlayerConnection();
        public void OnWaitingPlayerReconnection();
        public void OnInitGame(GameInitialPacket initialPacket);
        public void OnReadyGame();
        public void OnCanceledGame();
        public void OnFinishedGame();
        public void OnChatGame(int authorId, string message);

        public void OnSubscribedGameEvents(LiveData<NetPacketReader> liveData);
    }

}
