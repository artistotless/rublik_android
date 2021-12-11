
namespace RublikNativeAndroid
{
    public interface IMessengerInteractor
    {
        void SendPrivateMessage(int destinationUserId, string message);
    }
}