using System;
namespace RublikNativeAndroid.Exceptions
{
    public class UserNotFoundException : Exception
    {
        internal string _message { get; set; }
        public UserNotFoundException(string _message) : base(_message) => this._message = _message;
    }

    public class UserDataDeserializeException : Exception
    {
        public UserDataDeserializeException() : base() { }
    }
}
