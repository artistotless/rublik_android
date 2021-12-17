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
using Android.Animation;
using static Android.Animation.ValueAnimator;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.FloatingActionButton;
using RublikNativeAndroid.Adapters;
using static Android.Views.View;
using AndroidX.RecyclerView.Widget;

namespace RublikNativeAndroid.Fragments
{
    public class ShellGameFragment : Fragment, IHasToolbarTitle, IShellGameEventListener, IHideBottomNav, IAnimatorUpdateListener, IOnClickListener
    {
        private LottieAnimationView[] _eggs;
        private TextView _status, _scoresTable, _firstPlayerText, _secondPlayerText;
        private ImageView _firstPlayerImage, _secondPlayerImage;
        private RecyclerView _chatRecyclerView;
        private LottieAnimationView _loading;
        private FloatingActionButton _floatingActionButton;
        private View _bottomSheetMessages;
        private EditText _msgField;
        private Button _msgSubmit;
        private Button _scrollDownBtn;

        private BaseGameEventParserViewModel _eventParser;
        private ShellGameControllerViewModel _controller;
        private MessengerRecycleListAdapter _adapter;
        private BottomSheetDialog _bottomSheetDialog;


        private ServerEndpoint _endpoint;
        private ShellGamePlayer _selectorPlayer => _players.Count < _maxPlayersCount ? null : _players[_masterId == 0 ? 1 : 0];
        private ShellGamePlayer _masterPlayer => _players.Count <= 0 ? null : _players[_masterId];
        private List<ShellGamePlayer> _players = new List<ShellGamePlayer>();
        private int _award;
        private int _maxPlayersCount = 2;
        private int _masterId;
        private int _secondsForConnectingAllPlayers;
        private int _secondsForReconnect;
        private uint _steps;
        private int[] _scores = { 0, 0 };

        private IDisposable _eventsUnsubscriber;

        public string GetTitle() => GetString(Resource.String.shellgame);
        public ServerEndpoint GetServerEndpoint() => _endpoint;

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            try { _eventsUnsubscriber.Dispose(); }
            catch { }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _eventParser = new ShellGameEventParserViewModel(this);
            _controller = new ShellGameControllerViewModel();
            _adapter = new MessengerRecycleListAdapter(this);
            _award = Arguments.GetInt(Constants.GameTermins.AWARD);
            _endpoint = new ServerEndpoint(
                ip: Arguments.GetString(Constants.Fragments.IP),
                port: Arguments.GetInt(Constants.Fragments.PORT));
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_shellgame, container, false);
            _bottomSheetDialog = new BottomSheetDialog(Context);

            _bottomSheetMessages = LayoutInflater.From(Context).Inflate(Resource.Layout.bottom_sheet_shellgame,
                new LinearLayout(Context));

            _bottomSheetDialog.SetContentView(_bottomSheetMessages);

            _status = rootView.FindTextView(Resource.Id.shellgame_status);
            _chatRecyclerView = _bottomSheetMessages.FindRecyclerView(Resource.Id.messenger_dialog_list);
            _msgField = _bottomSheetMessages.FindEditText(Resource.Id.messenger_text_field);
            _scrollDownBtn = _bottomSheetMessages.FindButton(Resource.Id.messenger_scrolldown_btn);
            _msgSubmit = _bottomSheetMessages.FindButton(Resource.Id.messenger_send_btn);
            _floatingActionButton = rootView.FindFloatingBtn(Resource.Id.chat_floating_btn);
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

            AttachAdapter(_adapter, container);
            _msgField.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                if (string.IsNullOrEmpty(_msgField.Text))
                    _msgSubmit.Visibility = ViewStates.Gone;
                else
                    _msgSubmit.Visibility = ViewStates.Visible;
            };

            _scrollDownBtn.Click += (object sender, EventArgs e) => { SmoothScroolDown(); };

            _chatRecyclerView.ScrollChange += (object sender, ScrollChangeEventArgs e) =>
            {
                if (_adapter.ItemCount - GetCurrentRecyclerViewPos() > 3)
                    _scrollDownBtn.Visibility = ViewStates.Visible;
                else
                    _scrollDownBtn.Visibility = ViewStates.Gone;
            };

            _msgSubmit.Click += (object sender, EventArgs e) =>
            {
                Vibrator v = Vibrator.FromContext(Context);

                // Vibrate for 20 milliseconds
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    v.Vibrate(VibrationEffect.CreateOneShot(20, 10));

                else
                    //deprecated in API 26 
                    v.Vibrate(20);

                string message = _msgField.Text;
                _controller.Chat(message);
                _msgField.SetText(string.Empty, TextView.BufferType.Normal);
            };

            ConfigureAnimationView(_eggs[0], 1);
            ConfigureAnimationView(_eggs[1], 2);
            ConfigureAnimationView(_eggs[2], 3);
            _floatingActionButton.Click += delegate (object sender, EventArgs e) { _bottomSheetDialog.Show(); };

            return rootView;
        }

        private void ConfigureAnimationView(LottieAnimationView view, int id)
        {
            view.Click += delegate (object sender, EventArgs e) { SelectOrHide(id); };
            view.AddAnimatorUpdateListener(this);
        }
        private int GetCurrentRecyclerViewPos() => (_chatRecyclerView.GetLayoutManager() as LinearLayoutManager).FindLastVisibleItemPosition();

        private void AttachAdapter(MessengerRecycleListAdapter adapter, ViewGroup container)
        {
            LinearLayoutManager layoutManager = new LinearLayoutManager(container.Context, (int)Orientation.Vertical, false);

            layoutManager.StackFromEnd = true;

            _chatRecyclerView.SetLayoutManager(layoutManager);
            _chatRecyclerView.SetAdapter(adapter);
        }

        private void SelectOrHide(int eggNumber) => _controller.Move((ushort)eggNumber);

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
            _adapter.AddElement(new ChatMessage(UsersService.myUser.extraData.id, message, authorId, DateTime.Now));
        }

        private void SmoothScroolDown()
        {
            _scrollDownBtn.Visibility = ViewStates.Gone;
            _chatRecyclerView.SmoothScrollToPosition(_adapter.ItemCount - 1);
        }

        public void OnFinishedGame()
        {
            var myPlayerIndex = _players.IndexOf(new ShellGamePlayer(UsersService.myUser.extraData));
            var opponent = _players[myPlayerIndex == 0 ? 1 : 0];
            GameResult gameResult = _players[myPlayerIndex].score > opponent.score ? GameResult.Win : GameResult.Lose;
            this.Navigator().ShowGameResultPage(_award, gameResult);

        }

        public void OnCanceledGame()
        {
            this.Navigator().ShowGameResultPage(_award, GameResult.Cancel);
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

        public void OnSubscribedOnService(LiveData<NetPacketReader> liveData, IDisposable serviceDisposable)
        {
            var liveDataDisposable = liveData.Subscribe(
                (NetPacketReader reader) =>
                {
                    Console.WriteLine($"RoomsFragment : OnSubscribedOnLobbyService THREAD # {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    _eventParser.ParseNetPacketReader(reader);
                },
                delegate (Exception e) { },
                delegate { }
                );

            _eventsUnsubscriber = new UnsubscriberService(serviceDisposable, liveDataDisposable);
        }

        public void OnWaitingPlayerConnection()
        {
            Toast.MakeText(Context, GetString(Resource.String.awaitConnectionOtherPlayers), ToastLength.Short).Show();
        }

        public void OnWaitingPlayerReconnection()
        {
            Toast.MakeText(Context, GetString(Resource.String.awaitReconnection), ToastLength.Short).Show();
        }

        public static ShellGameFragment NewInstance(ServerEndpoint endpoint, uint award)
        {
            Bundle bundle = new Bundle();
            bundle.PutString(Constants.Fragments.IP, endpoint.ip);
            bundle.PutInt(Constants.Fragments.PORT, endpoint.port);
            bundle.PutInt(Constants.GameTermins.AWARD, (int)award);
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

        public void OnClick(View v)
        {
            //throw new NotImplementedException();
        }
    }
}
