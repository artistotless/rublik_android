using CrossPlatformLiveData;
using LiteNetLib;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    public interface IServerListener
    {
        public void OnSubscribedOnServer(LiveData<NetPacketReader> liveData);
        public ServerEndpoint GetServerEndpoint();
    }
}
