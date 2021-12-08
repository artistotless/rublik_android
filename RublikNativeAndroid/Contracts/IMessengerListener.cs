using CrossPlatformLiveData;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    public interface IMessengerListener
    {
        void OnSubscribedOnMessenger(LiveData<ChatMessage> liveData);
    }
}
