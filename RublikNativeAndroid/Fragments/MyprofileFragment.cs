using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using CrossPlatformLiveData;
using FFImageLoading;
using FFImageLoading.Transformations;
using LiteNetLib;
using RublikNativeAndroid.Adapters;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;
using RublikNativeAndroid.Utils;
using RublikNativeAndroid.ViewModels;
using static Android.Views.View;

namespace RublikNativeAndroid.Fragments
{

    public class MyprofileFragment : Fragment, IHasToolbarTitle, IOnClickListener, IMessengerEventListener
    {
        public string GetTitle() => GetString(Resource.String.myprofile);

        private ImageView _avatar;
        private TextView _nickname, _username;
        private EditText _quote;
        private Button _balance_btn;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private RecyclerView _friends_scroll;

        private ProfileViewModel _myProfileViewModel;
        private ViewEventListener _viewEventListener;
        private IDisposable _unsubscriber;
        private IDisposable _eventsUnsubscriber;
        private FriendRecycleListAdapter _adapter;
        private MessengerEventsParserViewModel _eventParser;

        private static int _userId { get; set; }
        private static User.Data _userData { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _viewEventListener = new ViewEventListener();
            _myProfileViewModel = _myProfileViewModel ?? new ProfileViewModel();
            _adapter = _adapter ?? new FriendRecycleListAdapter(this);
            _eventParser = _eventParser ?? new MessengerEventsParserViewModel(this);
            _userId = Arguments.GetInt(Constants.Fragments.USER_ID);
            ListenObservableObjects();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _unsubscriber.Dispose();
            _viewEventListener.Dispose();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _eventsUnsubscriber.Dispose();  
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_myprofile, container, false);
            InitServiceEvents(view);
            _balance_btn = view.FindButton(Resource.Id.profile_balance_btn);
            _username = view.FindTextView(Resource.Id.profile_username);
            _nickname = view.FindTextView(Resource.Id.profile_nickname);
            _avatar = view.FindImageView(Resource.Id.profile_image);
            _quote = view.FindEditText(Resource.Id.profile_status_value);
            _swipeRefreshLayout = view.FindRefreshLayout(Resource.Id.profile_refresh_swipe);
            _friends_scroll = view.FindRecyclerView(Resource.Id.profile_friends_scroll_view);
            _friends_scroll.AddOnScrollListener(new RefreshLayoutCollisionFixer(_swipeRefreshLayout));
            _viewEventListener.AddListener(new OnViewAttached(_friends_scroll,
                async delegate (object sender, ViewAttachedToWindowEventArgs e)
            {
                await RequestUpdateLiveData();
            }));

            AttachAdapter(_adapter, container);
            UpdateUI(_userData);

            _swipeRefreshLayout.Refresh += async (object sender, EventArgs e) =>
            {
                await RequestUpdateLiveData(ignoreCache: true);
                _swipeRefreshLayout.Refreshing = false;
            };

            return view;
        }

        private void InitServiceEvents(View root)
        {
            _viewEventListener.AddListener(new OnClick(root.FindCardView(Resource.Id.services_games),
                (object sender, EventArgs e) => { this.Navigator().ShowRoomsPage(); }
                ));
            _viewEventListener.AddListener(new OnClick(root.FindCardView(Resource.Id.services_news),
                (object sender, EventArgs e) => { throw new NotImplementedException(); }
                ));
            _viewEventListener.AddListener(new OnClick(root.FindCardView(Resource.Id.services_stats),
                (object sender, EventArgs e) => { throw new NotImplementedException(); }
                ));
            _viewEventListener.AddListener(new OnClick(root.FindCardView(Resource.Id.services_tournaments),
                (object sender, EventArgs e) => { throw new NotImplementedException(); }
                ));
        }

        private void ListenObservableObjects()
        {
            _unsubscriber = _myProfileViewModel.liveDataProfile.Subscribe(
               (User.Data data) => { UpdateUI(data); },
               (Exception e) => { }, () => { });

            _myProfileViewModel.liveDataFriends.Subscribe(
                (List<Friend> friends) => { SetFriends(friends); },
                (Exception e) => { }, () => { });
        }

        private void AttachAdapter(FriendRecycleListAdapter adapter, ViewGroup container)
        {
            if (_friends_scroll.GetAdapter() != null)
                return;
            LinearLayoutManager layoutManager = new LinearLayoutManager(container.Context, (int)Orientation.Horizontal, false);
            _friends_scroll.SetLayoutManager(layoutManager);
            _friends_scroll.SetAdapter(adapter);
        }
        private async Task RequestUpdateLiveData(bool ignoreCache = false)
        {
            await _myProfileViewModel.GetProfileAsync(_userId, ignoreCache);
            await _myProfileViewModel.GetFriendsAsync(_userId, ignoreCache);
        }

        public void OnClick(View v) => OnFriendClicked((int)v.Tag);
        private void OnFriendClicked(int userId) => this.Navigator().ShowProfilePage(userId);

        public static MyprofileFragment NewInstance()
        {
            var fragment = new MyprofileFragment();
            var bundle = new Bundle();
            bundle.PutInt(Constants.Fragments.USER_ID, ApiService.myUser.extraData.id);
            fragment.Arguments = bundle;
            return fragment;
        }

        public void UpdateUI(User.Data user)
        {
            if (user == null)
                return;

            _userData = user;
            SetAvatar(user.avatar);
            SetBalance(user.balance);
            SetUsername(user.username);
            SetNickname(user.nickname);
            SetQuote(user.quote);
        }

        private void SetAvatar(string path)
        {
            ImageService.Instance
            .LoadUrl(string.Format(Constants.WebApiUrls.FS_AVATAR, path))
            .FadeAnimation(true)
            .Transform(new CircleTransformation())
            .Into(_avatar);
        }

        private void SetBalance(int balance) => _balance_btn.Text = $"{balance} RUB";
        private void SetUsername(string username) => _username.Text = $"@{username}";
        private void SetNickname(string nickname) => _nickname.Text = nickname;
        private void SetQuote(string quote) => _quote.Text = quote;
        private void SetFriends(List<Friend> friends) => _adapter.SetElements(friends);


        public void OnComingMessage(ChatMessage message)
        {
            Toast.MakeText(Context, $"Message: {message.text}", ToastLength.Short).Show();
            // TODO: добавить сообщение в базу данных 
        }

        public void OnSubscribedOnServer(LiveData<NetPacketReader> liveData)
        {
            var liveDataDisposable = liveData.Subscribe(
              (NetPacketReader reader) => _eventParser.ParseNetPacketReader(reader),
              delegate (Exception e) { },
              delegate { }
              );

            _eventsUnsubscriber = new UnsubscriberService(liveDataDisposable);
        }

        public ServerEndpoint GetServerEndpoint() => new ServerEndpoint(
            ip: Constants.Services.MESSENGER_IP,
            port: Constants.Services.MESSENGER_PORT,
            serverType: ServerType.Messenger);

    }
}


