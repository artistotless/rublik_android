using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    public interface IMessengerEventListener: IServerListener
    {
        void OnComingMessage(ChatMessage message);
    }
}
