using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using RublikNativeAndroid.Adapters;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.ViewModels;

namespace RublikNativeAndroid.Fragments
{

    public class RefreshLayoutConflictFixer : RecyclerView.OnScrollListener
    {
        private SwipeRefreshLayout _layout;
        public RefreshLayoutConflictFixer(SwipeRefreshLayout layout) => _layout = layout;

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            if (_layout.Refreshing)
                return;

            _layout.Enabled = newState == 0;

        }

    }

    public class MyprofileFragment : Fragment, IHasToolbarTitle
    {

        private ImageView _avatar;
        private TextView _nickname, _username;
        private EditText _quote;
        private Button _balance_btn;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private RecyclerView _friends_scroll;

        private FriendRecycleListAdapter _adapter;
        private MyProfileViewModel _myProfileViewModel;
        private IDisposable _unsubscriber;

        private static int _userId { get; set; }
        private static string _accessKey { get; set; }

        public string GetTitle()
        {
            return GetString(Resource.String.myprofile);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _myProfileViewModel = new MyProfileViewModel();

            _accessKey = Arguments.GetString(Constants.Fragments.ACCESS_KEY);
            _userId = Arguments.GetInt(Constants.Fragments.USER_ID);

        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _unsubscriber.Dispose();
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_myprofile, container, false);

            _balance_btn = view.FindButton(Resource.Id.profile_balance_btn);
            _username = view.FindTextView(Resource.Id.profile_username);
            _nickname = view.FindTextView(Resource.Id.profile_nickname);
            _avatar = view.FindImageView(Resource.Id.profile_image);
            _quote = view.FindEditText(Resource.Id.profile_status_value);
            _friends_scroll = view.FindRecyclerView(Resource.Id.profile_friends_scroll_view);
            _swipeRefreshLayout = view.FindRefreshLayout(Resource.Id.profile_refresh_swipe);
            _friends_scroll.AddOnScrollListener(new RefreshLayoutConflictFixer(_swipeRefreshLayout));

            SetUpAdapter(container);
            ListenObservableObjects();

            _swipeRefreshLayout.Refresh += async (object sender, System.EventArgs e) =>
            {
                await RequestUpdateLiveData();
                _swipeRefreshLayout.Refreshing = false;

            };

            Task.Run(async () =>
            {
                await RequestUpdateLiveData();
            });

            return view;
        }

        private void ListenObservableObjects()
        {
            _unsubscriber = _myProfileViewModel.liveDataProfile.Subscribe(
               (Models.User user) => { UpdateUI(user); },
               (System.Exception e) => { }, () => { });

            _myProfileViewModel.liveDataFriends.Subscribe(
                (List<Friend> friends) => { SetFriends(friends); },
                (System.Exception e) => { }, () => { });
        }



        private async Task RequestUpdateLiveData()
        {
            await _myProfileViewModel.GetProfileAsync(_userId);
            await _myProfileViewModel.GetFriendsAsync(_userId);
        }

        private void SetUpAdapter(ViewGroup container)
        {
            _adapter = new FriendRecycleListAdapter(this);
            _friends_scroll.SetLayoutManager(new LinearLayoutManager(container.Context, 0, false));
            _friends_scroll.SetAdapter(_adapter);

        }


        public static MyprofileFragment NewInstance(string accessKey, int userId)
        {
            var fragment = new MyprofileFragment();
            var bundle = new Bundle();
            bundle.PutString(Constants.Fragments.ACCESS_KEY, accessKey);
            bundle.PutInt(Constants.Fragments.USER_ID, userId);
            fragment.Arguments = bundle;
            return fragment;
        }


        public void UpdateUI(Models.User user)
        {
            SetAvatar(user.extraData.avatar);
            SetBalance(user.extraData.balance);
            SetUsername(user.extraData.username);
            SetNickname(user.extraData.nickname);
            SetQuote(user.extraData.status);
        }

        private void SetAvatar(string path)
        {
            ImageService.Instance
            .LoadUrl(string.Format(Constants.WebApiUrls.FS_AVATAR, path))
            .FadeAnimation(true)
            .Transform(new CircleTransformation())
            .Into(_avatar);
        }

        private void SetNickname(string nickname) => _nickname.Text = nickname;
        private void SetUsername(string username) => _username.Text = $"@{username}";
        private void SetBalance(int balance) => _balance_btn.Text = $"{balance} RUB";
        private void SetQuote(string quote) => _quote.Text = quote;
        private void SetFriends(List<Friend> friends) => _adapter.SetFriends(friends);


    }
}


