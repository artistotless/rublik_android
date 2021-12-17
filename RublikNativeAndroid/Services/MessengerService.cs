using System;
using System.Threading;
using System.Threading.Tasks;
using CrossPlatformLiveData;
using LiteNetLib;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{
    public class MessengerService : IService, IDisposable
    {
        private LiveData<ChatMessage> _liveData = new LiveData<ChatMessage>();

        private NetPeer _chatServicePeer;
        private EventBasedNetListener _listener;
        private NetManager _client;
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        public void SendPrivateMessage(int destinationUserId, string message)
        {
            ChatMessage msg = new ChatMessage(destinationUserId, message);
            _chatServicePeer.Send(msg.GetNetDataWriter(), DeliveryMethod.ReliableUnordered);
        }

        public void SetListener(IMessengerListener listener)
        {
            listener.OnSubscribedOnMessenger(_liveData, this);
        }

        public void Connect(string accessKey)
        {
            if (GetConnectionState() == ConnectionState.Connected)
                return;

            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);
            CancellationToken canselToken = _cancelTokenSource.Token;
            _client.Start();

            // TODO: заменить ip сервиса на домен вида m1s.rublik.ru . Использовать DNS сервера
            _chatServicePeer = _client.Connect(Constants.Services.MESSENGER_IP, Constants.Services.MESSENGER_PORT, accessKey);
            _listener.NetworkReceiveEvent += (peer, packetReader, deliveryMethod) =>
            {
                Console.WriteLine($"NetworkReceiveEvent THREAD # {Thread.CurrentThread.ManagedThreadId}");
                ChatMessage message = new ChatMessage(packetReader);
                Console.WriteLine("[{0}][{1}]: {2}", message.timeStamp, message.authorId, message.text);

                _liveData.PostValue(message);

            };
            Task.Run(async delegate
            {
                while (!canselToken.IsCancellationRequested)
                {
                    Console.WriteLine($"PollEvents THREAD # {Thread.CurrentThread.ManagedThreadId}");
                    _client.PollEvents();
                    await Task.Delay(800);
                }
                _chatServicePeer.Disconnect();
                _client.Stop();
                Console.WriteLine($"Messenger Service was STOPED from THREAD # {Thread.CurrentThread.ManagedThreadId}");
            });

        }

        public void Disconnect()
        {
            _cancelTokenSource.Cancel();
        }

        public void Dispose()
        {
            Disconnect();
            _client = null;
            _listener = null;
        }

        public ConnectionState GetConnectionState() => _chatServicePeer == null ? ConnectionState.Disconnected : _chatServicePeer.ConnectionState;
    }
}
