using System.Collections.Generic;
using AndroidX.Lifecycle;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Fragments;
using System.Linq;

namespace RublikNativeAndroid.Games
{


    public abstract class BaseGameControllerViewModel : ViewModel
    {
        public uint award;
        public List<BasePlayer> players = new List<BasePlayer>();
        public ServerEndpoint endpoint;
        public string status;


        public  IGameEventListener gameEventListener;
        public int _maxPlayersCount = 2;
        public int _secondsForConnectingAllPlayers;
        public int _secondsForReconnect;

        public void SetListener(IGameEventListener shellGameEventListener)
        {
            gameEventListener = shellGameEventListener;
        }

        public virtual void Init(GameInitialPacket initialPacket)
        {
            _secondsForConnectingAllPlayers = initialPacket.secondsForConnectingAllPlayers;
            _secondsForReconnect = initialPacket.secondsForReconnect;
            players = initialPacket.players.ToList();
        }

        public abstract GameResult GetGameResult();

    }
}
