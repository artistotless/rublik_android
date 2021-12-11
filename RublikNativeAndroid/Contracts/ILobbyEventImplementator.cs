using LiteNetLib.Utils;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    public interface ILobbyEventImplementator
    {
        void Execute(User user, NetDataReader dataReader);
    }
}
