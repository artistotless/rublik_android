using System;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    public interface IMessengerListener
    {
        void OnHandleMessage(ChatMessage message);
        void OnSubscribedOnMessenger(IDisposable unsubscriber);
    }
}
