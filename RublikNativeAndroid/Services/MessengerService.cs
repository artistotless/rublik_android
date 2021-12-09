using System;
using System.Threading;
using System.Threading.Tasks;
using CrossPlatformLiveData;
using LiteNetLib;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{
    public class MessengerService : IService
    {
        //private IMessengerListener _fragmentListener;
        public LiveData<ChatMessage> liveData = new LiveData<ChatMessage>();

        private NetPeer _chatServicePeer;
        private EventBasedNetListener _localListener;
        private NetManager _client;
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        public void SendPrivateMessage(int destinationUserId, string message)
        {
            ChatMessage msg = new ChatMessage(destinationUserId, message);
            _chatServicePeer.Send(msg.GetNetDataWriter(), DeliveryMethod.ReliableUnordered);
        }

        public IDisposable SetListener(IMessengerListener listener)
        {
            //_fragmentListener = listener;
            //listener.OnSubscribedOnMessenger()
            listener.OnSubscribedOnMessenger(liveData);
            return null;
        }

        public void Connect(string accessKey)
        {
            if (GetConnectionState() == ConnectionState.Connected)
                return;

            _localListener = new EventBasedNetListener();
            _client = new NetManager(_localListener);
            CancellationToken canselToken = _cancelTokenSource.Token;
            _client.Start();

            // TODO: заменить ip сервиса на домен вида m1s.rublik.ru . Использовать DNS сервера
            _chatServicePeer = _client.Connect("192.168.43.44", 9052, accessKey);
            _localListener.NetworkReceiveEvent += (peer, packetReader, deliveryMethod) =>
            {
                Console.WriteLine($"NetworkReceiveEvent THREAD # {Thread.CurrentThread.ManagedThreadId}");
                ChatMessage message = new ChatMessage(packetReader);
                Console.WriteLine("[{0}][{1}]: {2}", message.timeStamp, message.authorId, message.text);

                /* if (_fragmentListener == null)
                     return; */

                liveData.PostValue(message);
                //_fragmentListener.OnHandleMessage(message);
            };
            Task.Run(async () =>
            {
                while (!canselToken.IsCancellationRequested)
                {
                    //Console.WriteLine($"PollEvents THREAD # {Thread.CurrentThread.ManagedThreadId}");
                    _client.PollEvents();
                    await Task.Delay(500);
                }
            }, canselToken);

        }

        public void Stop()
        {
            _cancelTokenSource.Cancel();
            _client.Stop();
        }

        public ConnectionState GetConnectionState() => _chatServicePeer == null ? ConnectionState.Disconnected : _chatServicePeer.ConnectionState;
    }

    public class Unsubscriber : IDisposable
    {
        private Action unSubscribeAction;
        public Unsubscriber(Action unSubscribeAction) => this.unSubscribeAction = unSubscribeAction;
        public void Dispose() => unSubscribeAction.Invoke();
    }
}
