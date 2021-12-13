using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;
using GameServices;
using RublikNativeAndroid.Models;
using CrossPlatformLiveData;
using LiteNetLib;
using Android.Widget;
using RublikNativeAndroid.Games;
using System;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.Fragments
{
    public class ShellGameFragment : Fragment, IHasToolbarTitle, IGameEventListener, IHideBottomNav
    {
        private Button _hide;
        private Button _select;
        private EditText _place;
        private TextView _status;

        private BaseGameEventParserViewModel _eventParser;
        private ShellGameControllerViewModel _controller;

        private ShellGame _game;

        private IDisposable _eventsUnsubscriber;

        public string GetTitle() => GetString(Resource.String.shellgame);

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            if (_eventsUnsubscriber != null)
                _eventsUnsubscriber.Dispose();

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _eventParser = new BaseGameEventParserViewModel(this);
            
            string ip = Arguments.GetString(Constants.Fragments.IP);
            int port = Arguments.GetInt(Constants.Fragments.PORT);
            try
            {
                _game = new ShellGame(new ShellGame.Player(UsersService.myUser.extraData), ip, port);
                _game.SetListener(this);
                _game.Start();

                _controller = new ShellGameControllerViewModel(_game);
            }
            catch { }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_shellgame, container, false);
            _status = rootView.FindTextView(Resource.Id.shellgame_status);

            //_hide.Click += delegate (object sender, EventArgs e) { _controller.HideBall(ushort.Parse(_place.Text)); };
            //_select.Click += delegate (object sender, EventArgs e) { _controller.SelectBall(ushort.Parse(_place.Text)); };

            return rootView;
        }

        public void OnChatGame(int authorId, string message)
        {
            Toast.MakeText(Context, $"authorId: {authorId} \n message: {message}", ToastLength.Short).Show();
        }

        public void OnFinishedGame(GameStatus newStatus)
        {
            Toast.MakeText(Context, $"Игра закончена", ToastLength.Short).Show();
        }

        public void OnCanceledGame(GameStatus newStatus)
        {
            Toast.MakeText(Context, $"Игра отменена, все средства возвращены игрокам", ToastLength.Short).Show();
        }

        public void OnInitGame(Room room)
        {
            Toast.MakeText(Context, $"Игра инициализирована", ToastLength.Short).Show();
        }

        public void OnReadyGame(Room room)
        {
            Toast.MakeText(Context, $"Игра готова", ToastLength.Short).Show();
        }

        public void OnSubscribedGameEvents(LiveData<NetPacketReader> liveData)
        {
            _eventsUnsubscriber = liveData.Subscribe(
                delegate (NetPacketReader reader)
                {
                    Console.WriteLine($"RoomsFragment : OnSubscribedOnLobbyService THREAD # {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    _eventParser.ParseNetPacketReader(reader);
                },
                delegate (Exception e) { },
                delegate { }
                );
        }

        public void OnWaitingPlayerConnection(GameStatus newStatus)
        {
            Toast.MakeText(Context, $"Ожиданием подключения игроков", ToastLength.Short).Show();
        }

        public void OnWaitingPlayerReconnection(GameStatus newStatus)
        {
            Toast.MakeText(Context, $"Оппонент вылетел, ждем переподключения", ToastLength.Short).Show();
        }

        public static ShellGameFragment NewInstance(string ip, int port)
        {
            Bundle bundle = new Bundle();
            bundle.PutString(Constants.Fragments.IP, ip);
            bundle.PutInt(Constants.Fragments.PORT, port);
            ShellGameFragment fragment = new ShellGameFragment();
            fragment.Arguments = bundle;
            return fragment;
        }
    }
}
