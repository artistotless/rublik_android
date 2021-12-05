using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.ViewModels;
using System;
using System.Collections.Generic;
using RublikNativeAndroid.Models;
using FFImageLoading;
using FFImageLoading.Transformations;
using RublikNativeAndroid.Adapters;
using static Android.Views.View;
using AndroidX.RecyclerView.Widget;
using RublikNativeAndroid.Utils;
using System.Threading.Tasks;
using AndroidX.SwipeRefreshLayout.Widget;

namespace RublikNativeAndroid.Fragments
{
    public class ProfileFragment : Fragment, IHasToolbarTitle, IOnClickListener
    {
        public string GetTitle() => GetString(Resource.String.profile);

        private ImageView _avatar;
        private TextView _nickname, _username;
        private Button _msgBtn;
        private EditText _quote;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private RecyclerView _friends_scroll;

        private int _userId { get; set; }
        private User.Data _userData { get; set; }

        private FriendRecycleListAdapter _adapter;
        private ProfileViewModel _profileViewModel;
        private IDisposable _unsubscriber;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _userId = Arguments.GetInt(Constants.Fragments.USER_ID);
            _profileViewModel = _profileViewModel == null ? new ProfileViewModel() : _profileViewModel;
            _adapter = _adapter == null ? new FriendRecycleListAdapter(this) : _adapter;

            ListenObservableObjects();

        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _unsubscriber.Dispose();
        }



        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_profile, container, false);
            _msgBtn = view.FindButton(Resource.Id.profile_send_msg_btn);
            _username = view.FindTextView(Resource.Id.profile_username);
            _avatar = view.FindImageView(Resource.Id.profile_image);
            _quote = view.FindEditText(Resource.Id.profile_status_value);
            _nickname = view.FindTextView(Resource.Id.profile_nickname);
            _msgBtn = view.FindButton(Resource.Id.profile_send_msg_btn);
            _swipeRefreshLayout = view.FindRefreshLayout(Resource.Id.profile_refresh_swipe);
            _friends_scroll = view.FindRecyclerView(Resource.Id.profile_friends_scroll_view);
            _friends_scroll.AddOnScrollListener(new RefreshLayoutCollisionFixer(_swipeRefreshLayout));
            _friends_scroll.ViewAttachedToWindow += async (object sender, ViewAttachedToWindowEventArgs e) =>
            {
                await RequestUpdateFriendsLiveData();
            };

            AttachAdapter(_adapter, container);
            UpdateUI(_userData);


            Task.Run(async () => { await RequestUpdateProfileLiveData(); });

            _swipeRefreshLayout.Refresh += async (object sender, EventArgs e) =>
            {
                await RequestUpdateProfileLiveData();
                await RequestUpdateFriendsLiveData();
                _swipeRefreshLayout.Refreshing = false;
            };

            _msgBtn.Click += (object sender, EventArgs e) => { this.Navigator().ShowMessenger(_userId); };

            return view;
        }

        private void ListenObservableObjects()
        {
            _unsubscriber = _profileViewModel.liveDataProfile.Subscribe(
               (User.Data data) => { UpdateUI(data); },
               (Exception e) => { }, () => { });

            _profileViewModel.liveDataFriends.Subscribe(
                (List<Friend> friends) => { SetFriends(friends); },
                (Exception e) => { }, () => { });
        }

        private void AttachAdapter(FriendRecycleListAdapter adapter, ViewGroup container)
        {
            _friends_scroll.SetLayoutManager(new LinearLayoutManager(container.Context, 0, false));
            _friends_scroll.SetAdapter(adapter);
        }

        private async Task RequestUpdateProfileLiveData() => await _profileViewModel.GetProfileAsync(_userId);
        private async Task RequestUpdateFriendsLiveData() => await _profileViewModel.GetFriendsAsync(_userId);


        public void OnClick(View v) => OnFriendClicked((int)v.Tag);
        private void OnFriendClicked(int userId) => this.Navigator().ShowProfilePage(userId);

        public static ProfileFragment NewInstance(int userId)
        {
            var fragment = new ProfileFragment();
            var bundle = new Bundle();
            bundle.PutInt(Constants.Fragments.USER_ID, userId);
            fragment.Arguments = bundle;
            return fragment;
        }

        public void UpdateUI(User.Data user)
        {
            if (user == null)
                return;

            _userData = user;
            SetAvatar(user.avatar);
            SetUsername(user.username);
            SetNickname(user.nickname);
            SetQuote(user.status);
        }

        private void SetAvatar(string path)
        {
            ImageService.Instance
            .LoadUrl(string.Format(Constants.WebApiUrls.FS_AVATAR, path))
            .FadeAnimation(true)
            .Transform(new CircleTransformation())
            .Into(_avatar);
        }

        private void SetUsername(string username) => _username.Text = $"@{username}";
        private void SetNickname(string nickname) => _nickname.Text = nickname;
        private void SetQuote(string quote) => _quote.Text = quote;
        private void SetFriends(List<Friend> friends) => _adapter.SetFriends(friends);
    }
}
