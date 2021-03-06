using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{

    public interface IGameEventListener : IServerListener
    {
        public void OnWaitingPlayerConnection();
        public void OnWaitingPlayerReconnection();
        public void OnInitGame(GameInitialPacket initialPacket);
        public void OnReadyGame();
        public void OnCanceledGame();
        public void OnFinishedGame();
        public void OnChatGame(int authorId, string message);
    }

}
