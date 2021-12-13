using System;
using System.Collections.Generic;
using LiteNetLib;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid.Games.ShellGame
{
    enum ShellGameEvent
    {
        updateState
    }

    public class ShellGameEventParserViewModel : BaseGameEventParserViewModel
    {
        private Dictionary<ShellGameEvent, Action<NetPacketReader>> _eventReferenses;
        private IShellGameEventListener _listener;


        public ShellGameEventParserViewModel(IShellGameEventListener listener) : base(listener)
        {
            _listener = listener;
            _eventReferenses = new Dictionary<ShellGameEvent, Action<NetPacketReader>>{
            {ShellGameEvent.updateState , ParseUpdateStateEvent},
            };
        }

        public override void ParseAdditionally(ushort eventCode, NetPacketReader reader)
        {
            if (_eventReferenses.ContainsKey((ShellGameEvent)eventCode))
                _eventReferenses[(ShellGameEvent)eventCode](reader);
            else
                throw new KeyNotFoundException();
        }

        private void ParseUpdateStateEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("Обновление состояния игры");
            try
            {
                _listener.OnUpdateState(
                    steps: dataReader.GetUInt(),
                    masterId: dataReader.GetInt(),
                    scores: dataReader.GetIntArray()
                );
                //game.masterPlayer.score = scores[0];
                //game.selectorPlayer.score = scores[1];
                //Console.WriteLine(" {0}:{1}  |  {2}:{3}", game.masterPlayer.username, game.masterPlayer.score, game.selectorPlayer.username, game.selectorPlayer.score);
                //Console.WriteLine(game.masterPlayer.username == GameInstance.currentGame.user.extraData.username ? "\n Вы ходите! Спрячьте шарик " : "\n Ваш соперник прячет шарик, ожидайте...");
            }
            catch { }

        }
    }
}
