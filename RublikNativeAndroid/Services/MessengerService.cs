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

        public void SetListener(IMessengerListener listener)
        {
            listener.OnSubscribedOnMessenger(liveData);
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
            _chatServicePeer = _client.Connect(Constants.Services.MESSENGER_IP, Constants.Services.MESSENGER_PORT, accessKey);
            _localListener.NetworkReceiveEvent += (peer, packetReader, deliveryMethod) =>
            {
                Console.WriteLine($"NetworkReceiveEvent THREAD # {Thread.CurrentThread.ManagedThreadId}");
                ChatMessage message = new ChatMessage(packetReader);
                Console.WriteLine("[{0}][{1}]: {2}", message.timeStamp, message.authorId, message.text);

                liveData.PostValue(message);

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
}
