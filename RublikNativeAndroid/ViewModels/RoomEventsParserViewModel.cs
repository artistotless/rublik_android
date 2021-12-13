using System;
using System.Collections.Generic;
using System.Linq;
using AndroidX.Lifecycle;
using LiteNetLib;
using LiteNetLib.Utils;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.ViewModels
{
    public class RoomEventsParserViewModel : ViewModel
    {
        private Dictionary<EventOption, Action<User, NetPacketReader>> _eventReferenses;
        private IRoomEventListener _listener;

        public RoomEventsParserViewModel(IRoomEventListener listener)
        {
            _listener = listener;
            _eventReferenses = new Dictionary<EventOption, Action<User, NetPacketReader>>{

                {EventOption.GotRoomList, ParseGotRoomsEvent},
                {EventOption.HostedRoom, ParseHostedRoomEvent},
                {EventOption.JoinedRoom, ParseJoinedRoomEvent},
                {EventOption.LeavedRoom, ParseLeavedRoomEvent},
                {EventOption.MessageRoom, ParseMessagedRoomEvent},
                {EventOption.DeletedRoom, ParseDeletedRoomEvent},
                {EventOption.HostOfRoomChanged, ParseDeletedRoomEvent},
                {EventOption.GameStarted, ParseGameStartedEvent},
            };
        }

        public void ParseNetPacketReader(NetPacketReader reader)
        {
            Console.WriteLine($"RoomEventsViewModel : ParseNetDataReader THREAD # {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            int code = reader.GetUShort();
            _eventReferenses[(EventOption)code](UsersService.myUser, reader);
        }

        private void ParseJoinedRoomEvent(User user, NetPacketReader dataReader)
        {
            Console.WriteLine("JointRoomEvent <- LobbyService");
            _listener.OnJoinedRoom(
             idRoom: dataReader.GetInt(),
             userName: dataReader.GetString());
        }

        private void ParseLeavedRoomEvent(User user, NetPacketReader dataReader)
        {
            Console.WriteLine("LeaveRoomEvent <- LobbyService");
            _listener.OnLeavedRoom(
                idRoom: dataReader.GetInt(),
                userName: dataReader.GetString());
        }

        private void ParseMessagedRoomEvent(User user, NetPacketReader dataReader)
        {
            Console.WriteLine("MessageRoomEvent <- LobbyService");
            if (user.inRoom)
            {
                string author = dataReader.GetString();
                string message = dataReader.GetString();
                Console.WriteLine("[{0}]: {1}", author, message);
            }
        }

        private void ParseDeletedRoomEvent(User user, NetPacketReader dataReader)
        {
            Console.WriteLine("DeleteRoomEvent <- LobbyService");
            _listener.OnDeletedRoom(idRoom: dataReader.GetInt());
        }

        private void ParseGotRoomsEvent(User user, NetPacketReader dataReader)
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

        private void ParseHostedRoomEvent(User user, NetPacketReader dataReader)
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


        private void ParseGameStartedEvent(User user, NetPacketReader dataReader)
        {
            Console.WriteLine("GameStartedEvent <- LobbyService");

            _listener.OnGameStarted(
                ip: dataReader.GetString(),
                port: dataReader.GetUShort()
                );

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
