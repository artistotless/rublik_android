using System;

namespace RublikNativeAndroid.Models
{

    public class User : IEquatable<User>
    {
        public class Data
        {
            public int id { get; set; }
            public string accessKey { get; set; }
            public string username { get; set; }
            public string nickname { get; set; }
            public string status { get; set; }
            public string avatar { get; set; }
            public bool isOnline { get; set; }
            public int balance { get; set; }
        }

        public User(Data extraData) => this.extraData = extraData;

        internal Data extraData { get; set; }
        internal Room currentRoom { get; set; }
        internal bool inRoom => currentRoom != null;

        public bool Equals(User other)
        {
            return other.extraData.username == this.extraData.username;
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
