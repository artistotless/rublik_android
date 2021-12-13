using CrossPlatformLiveData;
using LiteNetLib;
using RublikNativeAndroid.Games;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{

    public interface IGameEventListener
    {
        public void OnWaitingPlayerConnection(GameStatus newStatus);
        public void OnWaitingPlayerReconnection(GameStatus newStatus);
        public void OnInitGame(Room room);
        public void OnReadyGame(Room room);
        public void OnCanceledGame(GameStatus newStatus);
        public void OnFinishedGame(GameStatus newStatus);
        public void OnChatGame(int authorId, string message);

        public void OnSubscribedGameEvents(LiveData<NetPacketReader> liveData);
    }

}
