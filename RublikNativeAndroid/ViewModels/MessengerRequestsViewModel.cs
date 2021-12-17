using AndroidX.Lifecycle;
using LiteNetLib;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.ViewModels
{
    public class MessengerRequestsViewModel : ViewModel
    {
        public void SendPrivateMessage(int destinationUserId, string message)
        {
            ChatMessage msg = new ChatMessage(destinationUserId, message);
            Server.currentInstance.Send(msg.GetNetDataWriter(), DeliveryMethod.ReliableUnordered);
        }
    }
}
