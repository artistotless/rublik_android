using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid
{
    public interface IMessengerInteractor
    {
        void SendPrivateMessage(int destinationUserId, string message);
        void SubscribeOnMessenger(IMessengerListener listener);
    }
}