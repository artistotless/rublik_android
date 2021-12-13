using System.Collections.Generic;
using CrossPlatformLiveData;
using LiteNetLib;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    public interface IRoomEventListener
    {
        public void OnGotRooms(List<Room> rooms);
        public void OnDeletedRoom(int idRoom);
        public void OnHostedRoom(Room room);
        public void OnMessagedRoom(Room room);
        public void OnLeavedRoom(int idRoom, string userName);
        public void OnJoinedRoom(int idRoom, string userName);
        public void OnGameStarted(string ip, int port);

        public void OnSubscribedOnLobbyService(LiveData<NetPacketReader> liveData);
    }
}
