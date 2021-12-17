using AndroidX.Lifecycle;
using LiteNetLib;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.ViewModels
{
    public class MessengerEventsParserViewModel : ViewModel
    {
        private IMessengerEventListener _listener;

        public MessengerEventsParserViewModel(IMessengerEventListener listener)
        {
            _listener = listener;
        }

        public void ParseNetPacketReader(NetPacketReader reader)
        {
            if (reader.AvailableBytes > 0)
                _listener.OnComingMessage(new ChatMessage(reader));
        }
    }
}
