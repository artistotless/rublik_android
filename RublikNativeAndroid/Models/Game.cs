using System;
namespace RublikNativeAndroid.Models
{
    public class Game
    {
        public ushort id;
        public int maxPlayers;

        public Game(ushort id, int maxPlayers = 2)
        {
            this.id = id;
            this.maxPlayers = maxPlayers;
        }
    }
}
