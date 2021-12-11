using System;
using System.Collections.Generic;
using System.Threading;
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
using RublikNativeAndroid.Adapters;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;
using RublikNativeAndroid.Utils;
using RublikNativeAndroid.ViewModels;
using static Android.Views.View;

namespace RublikNativeAndroid.Fragments
{

    public class MyprofileFragment : Fragment, IHasToolbarTitle, IOnClickListener, IMessengerListener
    {
        public string GetTitle() => GetString(Resource.String.myprofile);

        private ImageView _avatar;
        private TextView _nickname, _username;
        private EditText _quote;
        private Button _balance_btn;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private RecyclerView _friends_scroll;

        private ProfileViewModel _myProfileViewModel;
        private IDisposable _unsubscriber;
        private IDisposable _unsubscriberMessenger;
        private FriendRecycleListAdapter _adapter;

        private static int _userId { get; set; }
        private static User.Data _userData { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _myProfileViewModel = _myProfileViewModel == null ? new ProfileViewModel() : _myProfileViewModel;
            _adapter = _adapter == null ? new FriendRecycleListAdapter(this) : _adapter;
            _userId = Arguments.GetInt(Constants.Fragments.USER_ID);
            ListenObservableObjects();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _unsubscriber.Dispose();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            if (_unsubscriberMessenger != null)
                _unsubscriberMessenger.Dispose();
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
            _friends_scroll.ViewAttachedToWindow += async (object sender, ViewAttachedToWindowEventArgs e) =>
            {
                if (_userData == null)
                {
                    await RequestUpdateProfileLiveData();
                }
                await RequestUpdateFriendsLiveData();

            };

            AttachAdapter(_adapter, container);
            UpdateUI(_userData);

            _swipeRefreshLayout.Refresh += async (object sender, EventArgs e) =>
            {
                await RequestUpdateProfileLiveData();
                await RequestUpdateFriendsLiveData();
                _swipeRefreshLayout.Refreshing = false;
            };

            return view;
        }

        private void InitServiceEvents(View root)
        {
            root.FindButton(Resource.Id.services_games).Click +=
                (object sender, EventArgs e) => { this.Navigator().ShowRoomsPage(); };
            root.FindButton(Resource.Id.services_news).Click +=
                (object sender, EventArgs e) => { throw new NotImplementedException(); };
            root.FindButton(Resource.Id.services_stats).Click +=
                (object sender, EventArgs e) => { throw new NotImplementedException(); };
            root.FindButton(Resource.Id.services_tournaments).Click +=
                (object sender, EventArgs e) => { throw new NotImplementedException(); };
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

        private async Task RequestUpdateProfileLiveData() => await _myProfileViewModel.GetProfileAsync(_userId);
        private async Task RequestUpdateFriendsLiveData() => await _myProfileViewModel.GetFriendsAsync(_userId);

        public void OnClick(View v) => OnFriendClicked((int)v.Tag);
        private void OnFriendClicked(int userId) => this.Navigator().ShowProfilePage(userId);

        public static MyprofileFragment NewInstance()
        {
            var fragment = new MyprofileFragment();
            var bundle = new Bundle();
            bundle.PutInt(Constants.Fragments.USER_ID, UsersService.myUser.extraData.id);
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


        public void OnSubscribedOnMessenger(LiveData<ChatMessage> liveData)
        {
            _unsubscriberMessenger = liveData.Subscribe(
                 (ChatMessage message) =>
                 {
                     Console.WriteLine($"MyprofileFragment:OnSubscribedOnMessenger THREAD # {Thread.CurrentThread.ManagedThreadId}");
                     Toast.MakeText(Context, $"Message: {message.text}", ToastLength.Short).Show();
                 },
                 (Exception e) => { },
                 () => { });
        }
    }
}


