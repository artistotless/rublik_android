using System;
using System.Collections.Generic;
using Android.Content;
using AndroidX.Lifecycle;
using LiteNetLib;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Utils;

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

    public abstract class BaseGameEventParserViewModel : ViewModel
    {

        private Dictionary<GameServerEvent, Action<NetPacketReader>> _eventReferenses;
        public IGameEventListener listener;

        public BaseGameEventParserViewModel()
        {
            _eventReferenses = new Dictionary<GameServerEvent, Action<NetPacketReader>>{
            {GameServerEvent.waitingForConnecting , ParseWaitingPlayerConnectionEvent},
            {GameServerEvent.waitingForReconnecting ,ParseWaitingPlayerReconnectionEvent},
            {GameServerEvent.init , ParseInitGameEvent},
            {GameServerEvent.ready , ParseReadyGameEvent},
            {GameServerEvent.canceled , ParseCanceledGameEvent},
            {GameServerEvent.finished , ParseFinishedGameEvent},
            {GameServerEvent.chat , ParseChatGameEvent}
            };
        }

        public void SetListener(IGameEventListener listener) => this.listener = listener;

        public void ParseNetPacketReader(NetPacketReader reader)
        {
            ushort eventCode = reader.GetUShort();
            if (_eventReferenses.ContainsKey((GameServerEvent)eventCode))
                _eventReferenses[(GameServerEvent)eventCode](reader);
            else
                ParseAdditionally(eventCode, reader);
        }

        public abstract void ParseAdditionally(ushort eventCode, NetPacketReader reader);

        private void ParseWaitingPlayerConnectionEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("WaitingPlayerConnectioneEvent <- GameServer");
            Console.WriteLine("Ожидаем подключения других участников");

            listener.OnWaitingPlayerConnection();
        }

        private void ParseWaitingPlayerReconnectionEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("WaitingPlayerReconnectionEvent <- GameServer");
            Console.WriteLine("Ваш оппонент отключился, ждем его переподключения некоторое время");
            listener.OnWaitingPlayerReconnection();
        }

        private void ParseInitGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("InitGameEvent <- GameServer");
            Console.WriteLine("Все игроки подключены. Инициализация");
            GameInitialPacket initialPacket = new GameInitialPacket(dataReader);
            listener.OnInitGame(initialPacket);

        }

        private void ParseFinishedGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("FinishedGameEvent <- GameServer");
            Console.WriteLine("Игра завершена. Победителю начислен выигрыш");
            listener.OnFinishedGame();
        }

        private void ParseChatGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("ChatGameEvent <- GameServer");
            listener.OnChatGame(
                dataReader.GetInt(),
                dataReader.GetString());
        }

        private void ParseCanceledGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("CanceledGameEvent <- GameServer");
            Console.WriteLine("Игра отменена. Деньги возвращены на ваш счет");
            listener.OnCanceledGame();
        }

        private void ParseReadyGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("ReadyGameEvent <- GameServer");
            listener.OnReadyGame();
        }
    }
}