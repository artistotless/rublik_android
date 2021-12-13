using System;
using System.Collections.Generic;
using System.Linq;
using AndroidX.Lifecycle;
using LiteNetLib;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Games;
using RublikNativeAndroid.Models;

namespace GameServices
{
    public class BaseGameEventParserViewModel : ViewModel
    {

        private Dictionary<GameStatus, Action<NetPacketReader>> _eventReferenses;
        private IGameEventListener _listener;

        public BaseGameEventParserViewModel(IGameEventListener listener)
        {
            _listener = listener;
            _eventReferenses = new Dictionary<GameStatus, Action<NetPacketReader>>{
            {GameStatus.waitingForConnecting , ParseWaitingPlayerConnectionEvent},
            {GameStatus.waitingForReconnecting ,ParseWaitingPlayerReconnectionEvent},
            {GameStatus.init , ParseInitGameEvent},
            {GameStatus.ready , ParseReadyGameEvent},
            {GameStatus.canceled , ParseCanceledGameEvent},
            {GameStatus.finished , ParseFinishedGameEvent},
            {GameStatus.chat , ParseChatGameEvent}
            };
        }

        public void ParseNetPacketReader(NetPacketReader reader)
        {
            Console.WriteLine($"RoomEventsViewModel : ParseNetDataReader THREAD # {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            _eventReferenses[(GameStatus)reader.GetUShort()](reader);
        }

        private void ParseWaitingPlayerConnectionEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("WaitingPlayerConnectioneEvent <- GameServer");
            Console.WriteLine("Ожидаем подключения других участников");
            _listener.OnWaitingPlayerConnection(GameStatus.waitingForConnecting);
        }

        private void ParseWaitingPlayerReconnectionEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("WaitingPlayerReconnectionEvent <- GameServer");
            Console.WriteLine("Ваш оппонент отключился, ждем его переподключения некоторое время");
            _listener.OnWaitingPlayerReconnection(GameStatus.waitingForReconnecting);
        }

        private void ParseInitGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("InitGameEvent <- GameServer");
            Console.WriteLine("Все игроки подключены. Инициализация");

            var players = dataReader.Get<PlayerListSerializable>();
            //game.players.Clear();
            foreach (var item in players.GetPlayers())
            {
                Console.WriteLine(item);
                //game.players.Add(new GameInstance.Player(item));
            }
            var secondsForConnectingAllPlayers = dataReader.GetInt();
            var secondsForReconnect = dataReader.GetInt();


            Console.WriteLine(" Время на подключение игроков: {0}", secondsForConnectingAllPlayers);
            Console.WriteLine(" Время на переподключение игрока: {0}", secondsForReconnect);
            Console.WriteLine(players.GetPlayers()[0].extraData.id == GameInstance.currentGame.player.extraData.id ? "\n Вы ходите! Спрячьте шарик " : "\n Ваш соперник прячет шарик, ожидайте...");
            //_listener.OnReadyGame(GameStatus.ready);

        }

        private void ParseFinishedGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("FinishedGameEvent <- GameServer");
            Console.WriteLine("Игра завершена. Победителю начислен выигрыш");
            _listener.OnFinishedGame(GameStatus.finished);
        }

        private void ParseChatGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("ChatGameEvent <- GameServer");
            _listener.OnChatGame(
                authorId: dataReader.GetInt(),
                message: dataReader.GetString());
        }

        private void ParseCanceledGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("CanceledGameEvent <- GameServer");
            Console.WriteLine("Игра отменена. Деньги возвращены на ваш счет");
            _listener.OnCanceledGame(GameStatus.canceled);
        }

        private void ParseReadyGameEvent(NetPacketReader dataReader)
        {
            Console.WriteLine("ReadyGameEvent <- GameServer");
            Console.WriteLine("Игра готова, можно продолжать играть");
            var steps = dataReader.GetUInt();
            var masterId = dataReader.GetInt();
            int[] scores = dataReader.GetIntArray();
            try
            {
                //game.masterPlayer.score = scores[0];
                //game.selectorPlayer.score = scores[1];
            }
            catch { }
            Console.WriteLine(" Round: #{0}", steps);
            //Console.WriteLine(" {0}:{1}  |  {2}:{3}", game.masterPlayer.username, game.masterPlayer.score, game.selectorPlayer.username, game.selectorPlayer.score);
            //Console.WriteLine(game.masterPlayer.username == GameInstance.currentGame.user.extraData.username ? "\n Вы ходите! Спрячьте шарик " : "\n Ваш соперник прячет шарик, ожидайте...");

            //game.status = GameStatus.ready;
        }
    }
}