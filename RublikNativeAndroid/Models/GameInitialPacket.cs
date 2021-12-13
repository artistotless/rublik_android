using System;
using LiteNetLib;

namespace RublikNativeAndroid.Models
{
    public class GameInitialPacket
    {
        public int secondsForConnectingAllPlayers;
        public int secondsForReconnect;
        public System.Collections.Generic.List<BasePlayer> players;


        public GameInitialPacket(NetPacketReader dataReader)
        {
            this.players = dataReader.Get<PlayerListSerializable>().GetPlayers();
            this.secondsForConnectingAllPlayers = dataReader.GetInt();
            this.secondsForReconnect = dataReader.GetInt();

            Console.WriteLine(" Время на подключение игроков: {0}", secondsForConnectingAllPlayers);
            Console.WriteLine(" Время на переподключение игрока: {0}", secondsForReconnect);
        }
    }
}
