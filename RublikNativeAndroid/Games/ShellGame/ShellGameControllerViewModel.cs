using System.Collections.Generic;
using System.Linq;
using static System.Console;
using Android.Views;
using AndroidX.Lifecycle;
using LiteNetLib;
using LiteNetLib.Utils;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;
using RublikNativeAndroid.Fragments;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid.Games
{

    enum ControllerAction : ushort { Move, Chat }
    public enum MoveStatus : ushort
    {
        OpponentWillHide, OpponentHided,
        YouWillHide, YouHided
    }

    public class ShellGameControllerViewModel : BaseGameControllerViewModel
    {
        public int currentEggIndex;
        public ViewStates loadingState;
        public MoveStatus lastMoveType;
        public bool lastMoveWasPredicted
        {
            get { try { return _scores[0] > MyPlayer.score; } catch { return false; } }
        }

        private IShellGameEventListener _shellGameEventListener;
        private int _masterId;
        private uint _steps;
        private int[] _scores = { 0, 0 };


        private ShellGamePlayer _selectorPlayer => players.Count < _maxPlayersCount ? null : (ShellGamePlayer)players[_masterId == 0 ? 1 : 0];
        private ShellGamePlayer _masterPlayer => players.Count <= 0 ? null : (ShellGamePlayer)players[_masterId];

        private bool isMasterPlayer(User player) => player.extraData.id == _masterPlayer.extraData.id;


        public override async void Init(GameInitialPacket initialPacket)
        {
            _shellGameEventListener = gameEventListener as IShellGameEventListener;
            _secondsForConnectingAllPlayers = initialPacket.secondsForConnectingAllPlayers;
            _secondsForReconnect = initialPacket.secondsForReconnect;
            players = initialPacket.players.ToList();

            for (int i = 0; i < players.Count; i++)
            {
                var userModel = await ApiService.GetUserAsync(players[i].extraData.id);
                players[i] = new ShellGamePlayer(userModel.extraData);
            }

            UpdateState(_steps, _masterId, _scores);
        }

        private void UpdateLastMoveType(bool moved)
        {
            if (!moved && isMasterPlayer(MyPlayer))
                lastMoveType = MoveStatus.YouWillHide;

            if (!moved && isMasterPlayer(Opponent))
                lastMoveType = MoveStatus.OpponentWillHide;

            if (moved && isMasterPlayer(MyPlayer))
                lastMoveType = MoveStatus.YouHided;

            if (moved && isMasterPlayer(Opponent))
                lastMoveType = MoveStatus.OpponentHided;
        }


        public void UpdateState(uint steps, int masterId, int[] scores)
        {
            bool isNewRound = steps - _steps == 2 || steps == 0;
            bool isMoved = steps - _steps == 1;

            _masterId = masterId;
            _scores = scores;

            WriteLine(
                $"steps - {steps} \n" +
                $" masterId - {masterId} \n " +
                $"scores - {scores} \n " +
                $"isNewRound - {isNewRound} \n" +
                $"isMasterPlayer(MyPlayer)) - {isMasterPlayer(MyPlayer)} \n" +
                $"isMasterPlayer(Opponent)) - {isMasterPlayer(Opponent)} \n" +
                $"Opponent id - {Opponent.extraData.id} \n" +
                $"Myplayer id - {MyPlayer.extraData.id} \n" +
                $"isMoved - {isMoved}");

            UpdateLastMoveType(moved: isMoved);

            if (isNewRound)
                _steps = steps;


            switch (lastMoveType)
            {
                case MoveStatus.YouWillHide:
                    status = "Спрячьте птенца в любое яйцо";
                    loadingState = ViewStates.Invisible;
                    break;

                case MoveStatus.OpponentWillHide:
                    status = "Оппонент загадывает яйцо";
                    loadingState = ViewStates.Visible;
                    break;

                case MoveStatus.YouHided:
                    status = "Ожидаем решения оппонента";
                    loadingState = ViewStates.Visible;
                    break;

                case MoveStatus.OpponentHided:
                    status = "Угадайте в каком яйце сидит птенец";
                    loadingState = ViewStates.Invisible;
                    break;
            }

            _shellGameEventListener.OnUpdateUI();

            _masterPlayer.score = _scores[0];
            _selectorPlayer.score = _scores[1];
        }

        public string GetCurrentScoreText()
        {
            if (players.Count == 0) return "0:0";
            return isMasterPlayer(players[0]) ? $"{_scores[0]}:{_scores[1]}" : $"{_scores[1]}:{_scores[0]}";
        }

        public override GameResult GetGameResult() => MyPlayer.score > Opponent.score ? GameResult.Win : GameResult.Lose;
        private ShellGamePlayer MyPlayer => (ShellGamePlayer)players.Find(x => x.extraData.id == ApiService.myUser.extraData.id);
        private ShellGamePlayer Opponent => (ShellGamePlayer)players.Find(x => x.extraData.id != ApiService.myUser.extraData.id);


        public void Move(ushort idPlace)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ControllerAction.Move);
            writer.Put(idPlace);
            Server.currentInstance.Send(writer, DeliveryMethod.ReliableOrdered);
            currentEggIndex = idPlace - 1;
            UpdateLastMoveType(moved: true);
        }


        public void Chat(string msg)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ControllerAction.Chat);
            writer.Put(msg);
            Server.currentInstance.Send(writer, DeliveryMethod.ReliableOrdered);
        }

    }
}
