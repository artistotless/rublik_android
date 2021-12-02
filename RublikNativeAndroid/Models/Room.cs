using System;
using System.Collections.Generic;
using System.Linq;

namespace RublikNativeAndroid.Models
{
    class Room
    {
        internal static List<Room> rooms = new List<Room>();

        internal List<User> members { get; private set; }
        internal Game game { get; private set; }
        internal readonly int id;
        internal User host => members[0];
        internal uint award { get; set; }
        internal readonly bool hasPassword;


        private Room(int id, Game game, User host, uint award, bool hasPassword)
        {
            this.members = new List<User>();
            this.members.Add(host);
            this.game = game;
            this.id = id;
            this.award = award;
            this.hasPassword = hasPassword;
        }

        internal void AddMember(User user)
        {
            if (game.maxPlayers > members.Count)
            {
                members.Add(user);
                user.currentRoom = this;
            }
        }

        internal void RemoveMember(User user)
        {
            members.Remove(user);
            user.currentRoom = null;
        }

        private Room(int id, Game game, IEnumerable<User> members, uint award, bool hasPassword)
        {
            this.members = members.ToList();
            this.game = game;
            this.id = id;
            this.award = award;
            this.hasPassword = hasPassword;
        }

        internal static void CreateAndAdd(int id, Game game, User host, uint award, bool hasPassword)
        {
            Room room = new Room(id, game, host, award, hasPassword);
            host.currentRoom = room;
            rooms.Add(room);
        }

        internal static void DeleteRoom(Room room)
        {
            rooms.Remove(room);
        }

        internal static void CreateAndAdd(int id, Game game, IEnumerable<User> members, uint award, bool hasPassword)
        {
            Room room = new Room(id, game, members, award, hasPassword);
            foreach (User member in members)
            {
                member.currentRoom = room;
            }
            rooms.Add(room);
        }

        internal static void Clear()
        {
            rooms.Clear();
        }


        internal static Room GetRoomById(int id)
        {
            return rooms.Where(x => x.id == id).SingleOrDefault();
        }


    }
}
