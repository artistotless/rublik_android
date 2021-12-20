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
using Android.Animation;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.FloatingActionButton;
using RublikNativeAndroid.Adapters;
using static Android.Views.View;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using static Android.Animation.Animator;

namespace RublikNativeAndroid.Fragments
{
    public class ShellGameFragment : BaseGameFragment, IHasToolbarTitle, IShellGameEventListener, IHideBottomNav, IOnClickListener, IAnimatorListener
    {
        private LottieAnimationView[] _eggs;
        private TextView _statusView, _scoresTable, _firstPlayerText, _secondPlayerText;
        private ImageView _firstPlayerImage, _secondPlayerImage;
        private RecyclerView _chatRecyclerView;
        private LottieAnimationView _loading;
        private FloatingActionButton _floatingActionButton;
        private View _bottomSheetMessages;
        private EditText _msgField;
        private Button _msgSubmit;
        private Button _scrollDownBtn;

        private ShellGameEventParserViewModel _eventParser;
        private ShellGameControllerViewModel _controller;
        private MessengerRecycleListAdapter _adapter;
        private BottomSheetDialog _bottomSheetDialog;
        private MessengerChatView _messengerChatView;

        public string GetTitle() => GetString(Resource.String.shellgame);


        public ShellGameFragment(string ip, int port, uint award)
            : base(new ShellGameControllerViewModel(), new ShellGameEventParserViewModel(), ip, port, award) { }


        public override void OnCreate(Bundle savedInstanceState)
        {
            RetainInstance = true;
            base.OnCreate(savedInstanceState);
            _controller = base.controller as ShellGameControllerViewModel;
            _controller.SetListener(this);
            _eventParser = base.eventParser as ShellGameEventParserViewModel;
            _eventParser.SetListener(this);
            _adapter = new MessengerRecycleListAdapter(this);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_shellgame, container, false);
            _bottomSheetDialog = new BottomSheetDialog(Context);

            _bottomSheetMessages = LayoutInflater.From(Context).Inflate(Resource.Layout.bottom_sheet_chat,
                new LinearLayout(Context));

            _bottomSheetDialog.SetContentView(_bottomSheetMessages);
            _statusView = rootView.FindTextView(Resource.Id.shellgame_status);
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
            _messengerChatView = new MessengerChatView(_msgField, _msgSubmit, _chatRecyclerView, _scrollDownBtn);
            _messengerChatView
                .SetAdapter(_adapter, container)
                .SetSubmitAction(_controller.Chat)
                .VibroEnable(Context);

            ConfigureAnimationView(_eggs[0], 1);
            ConfigureAnimationView(_eggs[1], 2);
            ConfigureAnimationView(_eggs[2], 3);

            _floatingActionButton.Click += delegate (object sender, EventArgs e) { _bottomSheetDialog.Show(); };
            OnUpdateUI();
            return rootView;
        }

        private void SelectOrHide(int eggNumber)
        {
            foreach (var egg in _eggs)
                egg.Progress = 0.0f;
            _controller.Move((ushort)eggNumber);
        }

        private void ConfigureAnimationView(LottieAnimationView view, int id)
        {
            view.Click += delegate (object sender, EventArgs e) { SelectOrHide(id); };
        }

        private void AnimateEgg(int eggNumber, bool isTruePredicted)
        {
            var egg = _eggs[eggNumber];
            egg.SetAnimation("egg_lottie.json");
            egg.Loop(false);
            egg.SetMaxProgress(isTruePredicted ? 1.0f : 0.4f);
            egg.PlayAnimation();
            egg.AddAnimatorListener(this);
        }
        public void OnUpdateUI()
        {
            var playerNicknames = new TextView[] { _firstPlayerText, _secondPlayerText };
            var playerImages = new ImageView[] { _firstPlayerImage, _secondPlayerImage };
            var players = _controller.players;
            if (players == null)
                return;

            for (int i = 0; i < players.Count; i++)
            {
                playerNicknames[i].Text = players[i].extraData.nickname;

                ImageService.Instance
                .LoadUrl(string.Format(Constants.WebApiUrls.FS_AVATAR, players[i].extraData.avatar))
                .FadeAnimation(true)
                .Transform(new CircleTransformation())
                .Into(playerImages[i]);
            }

            if (_controller.lastMoveType == MoveStatus.YouHided)
                AnimateEgg(_controller.currentEggIndex, false); // Выбор яйца
            else if (_controller.lastMoveType == MoveStatus.YouWillHide)
                AnimateEgg(_controller.currentEggIndex, _controller.lastMoveWasPredicted);
            else
                UpdateScore();
        }

        private void UpdateScore()
        {
            _scoresTable.Text = _controller.GetCurrentScoreText();
            _loading.Visibility = _controller.loadingState;
            _statusView.Text = _controller.status;
        }

        public void OnUpdateState(uint steps, int masterId, int[] scores) => _controller.UpdateState(steps, masterId, scores);

        public void OnClick(View v) { }


        public override void OnChatGame(int authorId, string message)
        {
            _adapter.AddElement(new ChatMessage(UsersService.myUser.extraData.id, message, authorId, DateTime.Now));
        }
        public void OnAnimationCancel(Animator animation) { }

        public void OnAnimationRepeat(Animator animation) { }

        public void OnAnimationStart(Animator animation) { }

        public void OnAnimationEnd(Animator animation) => UpdateScore();

        public static ShellGameFragment NewInstance(ServerEndpoint endpoint, uint award)
        {
            Bundle bundle = new Bundle();
            bundle.PutString(Constants.Fragments.IP, endpoint.ip);
            bundle.PutInt(Constants.Fragments.PORT, endpoint.port);
            bundle.PutInt(Constants.GameTermins.AWARD, (int)award);
            ShellGameFragment fragment = new ShellGameFragment(endpoint.ip, endpoint.port, award);
            fragment.Arguments = bundle;
            return fragment;
        }
    }
}
