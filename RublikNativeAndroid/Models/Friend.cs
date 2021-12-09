
using System;

namespace RublikNativeAndroid
{
    public class Friend : Contracts.IHasId
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int status { get; set; }
        public string avatarUrl { get; set;}
        public long GetId() => this.id;
    }
}