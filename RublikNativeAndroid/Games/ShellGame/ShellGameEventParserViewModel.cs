using System;
using System.Collections.Generic;
using LiteNetLib;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid.Games.ShellGame
{
    enum ShellGameEvent
    {
        updateState = Constants.Numbers.GameEnumCodeStart.ShellGame,
        successPredicted,
        failPredicted
    }

    public class ShellGameEventParserViewModel : BaseGameEventParserViewModel
    {
        private Dictionary<ShellGameEvent, Action<NetPacketReader>> _eventReferenses;
        private IShellGameEventListener _listener;


        public ShellGameEventParserViewModel() : base()
        {
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
                (listener as IShellGameEventListener).OnUpdateState(
                    steps: dataReader.GetUInt(),
                    masterId: dataReader.GetInt(),
                    scores: dataReader.GetIntArray()
                );
            }
            catch { }

        }
    }
}
