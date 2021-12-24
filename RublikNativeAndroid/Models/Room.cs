using System;
using System.Collections.Generic;
using System.Linq;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid.Models
{
    public class Room : IEquatable<Room>, IHasId
    {
        public static Room myRoom => User.myUser.currentRoom;

        public List<User> members { get; set; }
        public Game game { get; set; }
        public readonly int id;
        public User host => members[0];
        public uint award { get; set; }
        public bool hasPassword;

        public Room(int id, Game game, User host, uint award, bool hasPassword)
        {
            this.members = new List<User>();
            this.game = game;
            this.id = id;
            this.award = award;
            this.hasPassword = hasPassword;
            this.AddMember(host.Equals(User.myUser) ? User.myUser : host);
        }

        public Room(int id, Game game, IEnumerable<User> members, uint award, bool hasPassword)
        {
            this.members = members.ToList();
            this.game = game;
            this.id = id;
            this.award = award;
            this.hasPassword = hasPassword;
        }


        public void AddMember(User user)
        {
            if (game.maxPlayers > members.Count)
            {
                members.Add(user);
                user.currentRoom = this;
            }
        }

        public void RemoveMember(User user)
        {
            members.Remove(user);
            user.currentRoom = null;
        }

        public bool Equals(Room other)
        {
            if(other == null) return false;
            return other.id == this.id;
        }

        public static bool EqualById(int roomId, Room room)
        {
            if (room == null) return false;
            return room.id == roomId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            User objAsPlayer = obj as User;
            if (objAsPlayer == null) return false;
            else return Equals(objAsPlayer);
        }
        public long GetId() => id;

    }
}
