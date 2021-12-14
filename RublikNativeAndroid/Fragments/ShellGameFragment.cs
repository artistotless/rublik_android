using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Games;
using RublikNativeAndroid.Models;
using CrossPlatformLiveData;
using LiteNetLib;
using Android.Widget;
using System;
using RublikNativeAndroid.Services;
using Com.Airbnb.Lottie;
using RublikNativeAndroid.Games.ShellGame;
using System.Collections.Generic;
using System.Linq;
using Android.Views.Animations;
using Android.Animation;
using static Android.Animation.ValueAnimator;

namespace RublikNativeAndroid.Fragments
{
    public class ShellGameFragment : Fragment, IHasToolbarTitle, IShellGameEventListener, IHideBottomNav, IAnimatorUpdateListener
    {
        private LottieAnimationView[] _eggs;
        private TextView _status, _scoresTable, _firstPlayerText, _secondPlayerText;
        private ImageView _firstPlayerImage, _secondPlayerImage;
        private LottieAnimationView _loading;

        private BaseGameEventParserViewModel _eventParser;
        private ShellGameControllerViewModel _controller;

        private GameServer _game;

        private ShellGamePlayer _selectorPlayer => _players.Count < _maxPlayersCount ? null : _players[_masterId == 0 ? 1 : 0];
        private ShellGamePlayer _masterPlayer => _players.Count <= 0 ? null : _players[_masterId];
        private List<ShellGamePlayer> _players = new List<ShellGamePlayer>();
        private int _maxPlayersCount = 2;
        private int _masterId;
        private int _secondsForConnectingAllPlayers;
        private int _secondsForReconnect;
        private uint _steps;
        private int[] _scores = { 0, 0 };

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
            _eventParser = new ShellGameEventParserViewModel(this);

            string ip = Arguments.GetString(Constants.Fragments.IP);
            int port = Arguments.GetInt(Constants.Fragments.PORT);
            try
            {
                _game = new GameServer(new ShellGamePlayer(UsersService.myUser.extraData), ip, port);
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
            _firstPlayerText = rootView.FindTextView(Resource.Id.player1_name);
            _secondPlayerText = rootView.FindTextView(Resource.Id.player2_name);
            _scoresTable = rootView.FindTextView(Resource.Id.score);
            _firstPlayerImage = rootView.FindImageView(Resource.Id.player1_image);
            _secondPlayerImage = rootView.FindImageView(Resource.Id.player2_image);
            _loading = rootView.FindLottie(Resource.Id.loading);
            _eggs = new LottieAnimationView[]{
                    rootView.FindLottie(Resource.Id.egg1),
                    rootView.FindLottie(Resource.Id.egg2),
                    rootView.FindLottie(Resource.Id.egg3),
                    };

            ConfigureAnimationView(_eggs[0], 1);
            ConfigureAnimationView(_eggs[1], 2);
            ConfigureAnimationView(_eggs[2], 3);

            //_hide.Click += delegate (object sender, EventArgs e) { _controller.HideBall(ushort.Parse(_place.Text)); };
            //_select.Click += delegate (object sender, EventArgs e) { _controller.SelectBall(ushort.Parse(_place.Text)); };

            return rootView;
        }

        private void ConfigureAnimationView(LottieAnimationView view, int id)
        {
            view.Click += delegate (object sender, EventArgs e) { SelectOrHide(id); };
            view.AddAnimatorUpdateListener(this);
        }

        private void SelectOrHide(int eggNumber)
        {
            Toast.MakeText(Context, $"Egg number #{eggNumber}", ToastLength.Short).Show();
            _controller.Move((ushort)eggNumber);

        }

        private void AnimateEgg(int eggNumber, bool isTruePredicted)
        {
            var egg = _eggs[eggNumber];

            egg.SetAnimation("egg_lottie.json");
            egg.Loop(false);
            egg.SetMaxProgress(isTruePredicted ? 1.0f : 0.4f);
            egg.PlayAnimation();
            egg.Progress = 0.0f;
        }

        public void OnChatGame(int authorId, string message)
        {
            Toast.MakeText(Context, $"{GetString(Resource.String.send_message)}: {message}", ToastLength.Long).Show();
        }

        public void OnFinishedGame()
        {
            Toast.MakeText(Context, GetString(Resource.String.finishedGame), ToastLength.Short).Show();
        }

        public void OnCanceledGame()
        {
            Toast.MakeText(Context, GetString(Resource.String.canceledGame), ToastLength.Short).Show();
        }

        public void OnInitGame(GameInitialPacket initialPacket)
        {
            Toast.MakeText(Context, GetString(Resource.String.initGame), ToastLength.Short).Show();
            _secondsForConnectingAllPlayers = initialPacket.secondsForConnectingAllPlayers;
            _secondsForReconnect = initialPacket.secondsForReconnect;
            _players = initialPacket.players.Select(x => new ShellGamePlayer(x.extraData)).ToList();

            UpdateUI(isNewRound: true);
        }

        public void OnReadyGame()
        {
            Toast.MakeText(Context, GetString(Resource.String.readyGame), ToastLength.Short).Show();
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

        public void OnWaitingPlayerConnection()
        {
            Toast.MakeText(Context, GetString(Resource.String.awaitConnectionOtherPlayers), ToastLength.Short).Show();
        }

        public void OnWaitingPlayerReconnection()
        {
            Toast.MakeText(Context, GetString(Resource.String.awaitReconnection), ToastLength.Short).Show();
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

        private bool isMasterPlayer(User player) => player.extraData.id == _masterPlayer.extraData.id;

        private void UpdateUI(bool isNewRound = false)
        {

            _firstPlayerText.Text = _players[0].extraData.username;
            _secondPlayerText.Text = _players[1].extraData.username;

            Console.WriteLine(
                $"Master - {_masterPlayer.extraData.username} : {_masterPlayer.score} \n" +
                $"Selector - {_selectorPlayer.extraData.username} : {_selectorPlayer.score}  \n"
                );

            if (isMasterPlayer(UsersService.myUser))
            {
                AnimateEgg(_controller.currentEggIndex, _scores[0] > _masterPlayer.score);
                _loading.Visibility = !isNewRound ? ViewStates.Visible : ViewStates.Gone;
                _status.Text = !isNewRound ? "Ожидаем решения оппонента" : "Спрячьте птенца в любое яйцо";
            }
            else
            {
                _loading.Visibility = isNewRound ? ViewStates.Visible : ViewStates.Gone;
                _status.Text = isNewRound ? "Оппонент загадывает яйцо" : "Угадайте в каком яйце сидит птенец";
            }

            _masterPlayer.score = _scores[0];
            _selectorPlayer.score = _scores[1];


            _scoresTable.Text = isMasterPlayer(_players[0]) ? $"{_scores[0]}:{_scores[1]}" : $"{_scores[1]}:{_scores[0]}";
        }

        public void OnUpdateState(uint steps, int masterId, int[] scores)
        {
            bool isNewRound = steps > _steps;
            _steps = steps;
            _masterId = masterId;
            _scores = scores;

            Console.WriteLine($"steps - {_steps} \n masterId - {_masterId} \n scores - {_scores} \n isNewRound - {isNewRound}");
            UpdateUI(isNewRound: isNewRound);
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            animation.AnimationEnd += (object sender, EventArgs e) => { _eggs[0].Progress = 0; _eggs[1].Progress = 0; _eggs[2].Progress = 0; };
        }
    }
}
