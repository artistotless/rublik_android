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
using RublikNativeAndroid.Services;
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
        private Button _balance_btn;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private RecyclerView _friends_scroll;

        private FriendRecycleListAdapter _adapter;
        private MyProfileViewModel _myProfileViewModel;

        public string GetTitle()
        {
            return GetString(Resource.String.myprofile);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _myProfileViewModel = new MyProfileViewModel();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_myprofile, container, false);

            _balance_btn = view.FindButton(Resource.Id.profile_balance_btn);
            _username = view.FindTextView(Resource.Id.profile_username);
            _nickname = view.FindTextView(Resource.Id.profile_nickname);
            _avatar = view.FindImageView(Resource.Id.profile_image);
            _friends_scroll = view.FindRecyclerView(Resource.Id.profile_friends_scroll_view);
            _swipeRefreshLayout = view.FindRefreshLayout(Resource.Id.profile_refresh_swipe);

            _friends_scroll.AddOnScrollListener(new RefreshLayoutConflictFixer(_swipeRefreshLayout));

            _adapter = new FriendRecycleListAdapter(new List<Friend> { Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake() });
            RecyclerView scrolling = view.FindViewById<RecyclerView>(Resource.Id.profile_friends_scroll_view);
            scrolling.SetLayoutManager(new LinearLayoutManager(container.Context, 0, false));
            scrolling.SetAdapter(_adapter);



            return view;
        }

        private void SetAvatar(string url)
        {
            ImageService.Instance
            .LoadUrl("https://images.unsplash.com/photo-1638104191847-3868d225bf93?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1075&q=80")
            .FadeAnimation(true)
            .Transform(new CornersTransformation(5, CornerTransformType.AllRounded))
            .Into(_avatar);
        }

        private void SetUsername(string username) => _username.Text = username;
        private void SetNickname(string nickname) => _nickname.Text = nickname;
        private void SetBalance(int balance) => _balance_btn.Text = $"{balance} RUB";
        private void SetFriends(List<Friend> friends) => _adapter = friends;


        private async void _balance_btn_Click(object sender, System.EventArgs e)
        {
            System.Console.WriteLine("_balance_btn_Click");
            var model = await UsersService.GetUser(1);
            System.Console.WriteLine(1);
        }

        public static MyprofileFragment NewInstance(string accessKey)
        {
            var fragment = new MyprofileFragment();
            var bundle = new Bundle();
            bundle.PutString(Constants.Fragments.ACCESS_KEY, accessKey);
            fragment.Arguments = bundle;
            return fragment;
        }

    }
}


