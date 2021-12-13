using System.Collections.Generic;
using LiteNetLib.Utils;

namespace RublikNativeAndroid.Models
{
    public class PlayerListSerializable : INetSerializable
    {
        private List<BasePlayer> _players;

        public PlayerListSerializable() => _players = new List<BasePlayer>();
        public PlayerListSerializable(List<BasePlayer> players) => _players = players;

        public void Add(BasePlayer player) => _players.Add(player);
        public List<BasePlayer> GetPlayers() => _players;

        public void Deserialize(NetDataReader reader)
        {
            var playersCount = reader.GetInt();
            for (int i = 0; i < playersCount; i++)
            {
                var player = new BasePlayer(new User.Data()
                {
                    id = reader.GetInt(),
                    username = reader.GetString()
                });

                player.teamId = reader.GetInt();

                _players.Add(player);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(_players.Count);
            foreach (var item in _players)
            {
                writer.Put(item.extraData.id);
                writer.Put(item.teamId);
                writer.Put(item.extraData.username);
            }
        }
    }
}
