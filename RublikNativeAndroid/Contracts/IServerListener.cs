using System;
using CrossPlatformLiveData;
using LiteNetLib;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    public interface IServerListener
    {
        public void OnSubscribedOnServer(LiveData<NetPacketReader> liveData, IDisposable serviceDisposable);
        public ServerEndpoint GetServerEndpoint();
    }
}
