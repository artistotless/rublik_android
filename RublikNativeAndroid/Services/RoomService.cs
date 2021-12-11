using System.Collections.Generic;
using System.Linq;
using CrossPlatformLiveData;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{
    public class RoomService
    {
        private List<Room> _rooms = new List<Room>();
        public LiveData<List<Room>> liveData = new LiveData<List<Room>>();

        public void CreateAndAdd(int id, Game game, User host, uint award, bool hasPassword)
        {
            Room room = new Room(id, game, host, award, hasPassword);
            host.currentRoom = room;         
            _rooms.Add(room);
            liveData.PostValue(_rooms);
        }

        public void DeleteRoom(Room room)
        {
            _rooms.Remove(room);
            liveData.PostValue(_rooms);
        }

        public void CreateAndAdd(int id, Game game, IEnumerable<User> members, uint award, bool hasPassword)
        {
            Room room = new Room(id, game, members, award, hasPassword);
            foreach (User member in members)
            {
                member.currentRoom = room;
            }
            _rooms.Add(room);

            liveData.PostValue(_rooms);
        }

        public void Clear()
        {
            _rooms.Clear();
            liveData.PostValue(_rooms);
        }


        public Room GetRoomById(int id)
        {
            return _rooms.Where(x => x.id == id).SingleOrDefault();
        }
    }
}
