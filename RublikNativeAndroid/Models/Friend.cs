using System;

namespace RublikNativeAndroid
{
    public class Friend
    {
        public int id { get; set; }
        public string nickname { get; set; }
        public string imageUrl { get; set; }

        public static Friend Fake()
        {
            return new Friend()
            {
                id = new Random().Next(1000),
                nickname = $"User#{new Random().Next(500)}",
                imageUrl = "https://camo.githubusercontent.com/3c96409fc61548f73989aca823bbbf961189a7a94c8a78e4be1d5fe526f569ad/687474703a2f2f672e7265636f726469742e636f2f70326f454e32624c496e2e676966"
            };
        }

        public static explicit operator Friend(Java.Lang.Object v)
        {
            throw new NotImplementedException();
        }
    }
}