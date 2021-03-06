using System.Collections.Generic;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    public interface IRoomEventListener: IServerListener
    {
        public void OnGotRooms(List<Room> rooms);
        public void OnDeletedRoom(int idRoom);
        public void OnHostedRoom(Room room);
        public void OnMessagedRoom(Room room);
        public void OnLeavedRoom(int idRoom, int userId);
        public void OnJoinedRoom(int idRoom, int userId);
        public void OnGameStarted(ServerEndpoint endpoint);

    }
}
