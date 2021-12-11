using System;
using System.Collections.Generic;
using System.Linq;
using AndroidX.Lifecycle;
using LiteNetLib.Utils;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.ViewModels
{
    public class RoomEventsViewModel : ViewModel
    {
        private Dictionary<EventOption, Action<User, NetDataReader>> _eventReferenses;
        private IRoomEventListener _listener;

        public RoomEventsViewModel(IRoomEventListener listener)
        {
            _listener = listener;
            _eventReferenses = new Dictionary<EventOption, Action<User, NetDataReader>>{

                {EventOption.GotRoomList, OnGotRooms},
                {EventOption.HostedRoom, OnHostedRoom},
                {EventOption.JoinedRoom, OnJoinedRoom},
                {EventOption.LeavedRoom, OnLeavedRoom},
                {EventOption.MessageRoom, OnMessagedRoom},
                {EventOption.DeletedRoom, OnDeletedRoom},
                {EventOption.GameStarted, OnGameStarted},
            };
        }

        public void ParseNetDataReader(NetDataReader reader)
        {
            Console.WriteLine($"RoomEventsViewModel : ParseNetDataReader THREAD # {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            _eventReferenses[(EventOption)reader.GetUShort()](UsersService.myUser, reader);
        }

        private void OnJoinedRoom(User user, NetDataReader dataReader)
        {
            Console.WriteLine("JointRoomEvent <- LobbyService");
            _listener.OnJoinedRoom(
             idRoom: dataReader.GetInt(),
             userName: dataReader.GetString());
        }

        private void OnLeavedRoom(User user, NetDataReader dataReader)
        {
            Console.WriteLine("LeaveRoomEvent <- LobbyService");
            _listener.OnLeavedRoom(
                idRoom: dataReader.GetInt(),
                userName: dataReader.GetString());
        }

        private void OnMessagedRoom(User user, NetDataReader dataReader)
        {
            Console.WriteLine("MessageRoomEvent <- LobbyService");
            if (user.inRoom)
            {
                string author = dataReader.GetString();
                string message = dataReader.GetString();
                Console.WriteLine("[{0}]: {1}", author, message);
            }
        }

        private void OnDeletedRoom(User user, NetDataReader dataReader)
        {
            Console.WriteLine("DeleteRoomEvent <- LobbyService");
            _listener.OnDeletedRoom(idRoom: dataReader.GetInt());
        }

        private void OnGotRooms(User user, NetDataReader dataReader)
        {
            Console.WriteLine("GetRoomsEvent <- LobbyService");
            List<Room> rooms = new List<Room>();
            int roomsCount = dataReader.GetInt();
            for (int i = 0; i < roomsCount; i++)
            {
                rooms.Add(
                new Room(
                     id: dataReader.GetInt(),
                     game: new Game(dataReader.GetUShort()),
                     members: dataReader.GetStringArray().Select(x => new User(x)).ToList(),
                     award: dataReader.GetUInt(),
                     hasPassword: dataReader.GetBool()
                    ));
            }

            _listener.OnGotRooms(rooms);
        }

        private void OnHostedRoom(User user, NetDataReader dataReader)
        {
            Console.WriteLine("HostRoomEvent <- LobbyService");

            _listener.OnHostedRoom(new Room(
                id: dataReader.GetInt(),
                game: new Game(dataReader.GetUShort()),
                host: new User(dataReader.GetString()),
                award: dataReader.GetUInt(),
                hasPassword: dataReader.GetBool()
                ));

            //TODO: запрос данных из БД или кэшХранилища по gameId  и формирование инстанса типа Game
        }


        private void OnGameStarted(User user, NetDataReader dataReader)
        {
            Console.WriteLine("GameStartedEvent <- LobbyService");

            _listener.OnGameStarted(new Room(1, null, new List<User>(), 100, true));

            /*
            string ip = dataReader.GetString();
            ushort port = dataReader.GetUShort();
            System.Console.WriteLine("Endpoint of gameServer - {0}:{1}", ip, port);
            if (ip == "undefined") { System.Console.WriteLine("Ошибка, LSPM: undefined"); return; }
            Room.Clear();
            var game = new ShellGame(user, ip, port);
            GameInstance.currentGame = game;
            game.Start();*/

        }
    }
}
