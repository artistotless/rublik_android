using System;
using CrossPlatformLiveData;
using LiteNetLib;

namespace RublikNativeAndroid.Contracts
{
    public interface IServiceListener
    {
        public void OnSubscribedOnService(LiveData<NetPacketReader> liveData, IDisposable serviceDisposable);
    }
}
