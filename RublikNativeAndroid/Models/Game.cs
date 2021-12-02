using System;
namespace RublikNativeAndroid.Models
{
    class Game
    {
        internal ushort id;
        internal int maxPlayers;

        internal Game(ushort id, int maxPlayers = 2)
        {
            this.id = id;
            this.maxPlayers = maxPlayers;
        }
    }
}
