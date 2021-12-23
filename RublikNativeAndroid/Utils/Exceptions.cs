using System;
namespace RublikNativeAndroid.Exceptions
{
    public class UserNotFoundException : Exception
    {
        internal string _message { get; set; }
        public UserNotFoundException(string _message) : base(_message) => this._message = _message;
    }

    public class ItemNotFoundException : Exception
    {
        internal string _message { get; set; }
        public ItemNotFoundException(string _message) : base(_message) => this._message = _message;
    }

    public class GameNotFoundException : Exception
    {
        internal string _message { get; set; }
        public GameNotFoundException(string _message) : base(_message) => this._message = _message;
    }

    public class UserDataDeserializeException : Exception
    {
        public UserDataDeserializeException() : base() { }
    }

    public class DeserializeException : Exception
    {
        public DeserializeException() : base() { }
    }

    public class GameDataDeserializeException : Exception
    {
        public GameDataDeserializeException() : base() { }
    }
}
