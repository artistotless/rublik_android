namespace RublikNativeAndroid.Models
{
    public class Game
    {
        public ushort id { get; set; }
        public uint minPlayers { get; set; }
        public uint maxPlayers;
        public uint minBid { get; set; }
        public uint maxBid { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string title { get; set; }

        public Game() { }
        public Game(ushort id, int maxPlayers = 2)
        {
            this.id = id;
            this.maxPlayers = (uint)maxPlayers;
        }

    }
}
