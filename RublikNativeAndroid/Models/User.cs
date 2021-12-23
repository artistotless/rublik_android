using System;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.Models
{

    public class User : IEquatable<User>
    {
        public static User myUser => ApiService.myUser;

        public class Data
        {
            public int id { get; set; }
            public string accessKey { get; set; }
            public string username { get; set; }
            public string nickname { get; set; }
            public string quote { get; set; }
            public string avatar { get; set; }
            public bool isOnline { get; set; }
            public int balance { get; set; }
        }

        public User(Data extraData) => this.extraData = extraData;
        public User(int id) : this(new Data() { id = id }) { }

        internal Data extraData { get; set; }
        internal Room currentRoom { get; set; }
        internal bool inRoom => currentRoom != null;

        public bool Equals(User other)
        {
            return other.extraData.id == this.extraData.id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            User objAsPlayer = obj as User;
            if (objAsPlayer == null) return false;
            else return Equals(objAsPlayer);
        }

    }
}
