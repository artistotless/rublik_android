using AndroidX.Fragment.App;
using Android.OS;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Games;
using RublikNativeAndroid.Models;
using CrossPlatformLiveData;
using LiteNetLib;
using Android.Widget;
using System;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.Fragments
{
    public abstract class BaseGameFragment : Fragment, IGameEventListener, IHideBottomNav
    {
        public BaseGameControllerViewModel controller;
        public BaseGameEventParserViewModel eventParser;

        private IDisposable _eventsUnsubscriber;

        public ServerEndpoint GetServerEndpoint() => controller.endpoint;

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _eventsUnsubscriber.Dispose();
        }

        public BaseGameFragment(BaseGameControllerViewModel controller,BaseGameEventParserViewModel eventParser, string ip, int port, uint award)
        {
            this.controller = controller;
            this.controller.award = award;
            this.controller.endpoint = new ServerEndpoint(ip, port, ServerType.Game);
            this.eventParser = eventParser;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            RetainInstance = true;
            base.OnCreate(savedInstanceState);
        }

        public void OnInitGame(GameInitialPacket initialPacket)
        {
            Toast.MakeText(Context, GetString(Resource.String.initGame), ToastLength.Short).Show();
            controller.Init(initialPacket);
        }

        public virtual void OnFinishedGame() => this.Navigator().ShowGameResultPage(controller.award, controller.GetGameResult());

        public virtual void OnCanceledGame() => this.Navigator().ShowGameResultPage(controller.award, GameResult.Cancel);

        public virtual void OnReadyGame() => Toast.MakeText(Context, GetString(Resource.String.readyGame), ToastLength.Short).Show();

        public virtual void OnWaitingPlayerConnection() => Toast.MakeText(Context, GetString(Resource.String.awaitConnectionOtherPlayers), ToastLength.Short).Show();

        public virtual void OnWaitingPlayerReconnection() => Toast.MakeText(Context, GetString(Resource.String.awaitReconnection), ToastLength.Short).Show();

        public virtual void OnSubscribedOnServer(LiveData<NetPacketReader> liveData)
        {
            var liveDataDisposable = liveData.Subscribe(
                (NetPacketReader reader) =>
                {
                    Console.WriteLine($"ShellGame Fragment : OnSubscribedOnServer THREAD # {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    eventParser.ParseNetPacketReader(reader);
                },
                delegate (Exception e) { },
                delegate { }
                );

            _eventsUnsubscriber = new UnsubscriberService(liveDataDisposable);
        }

        public abstract void OnChatGame(int authorId, string message);

    }
}
