using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using RublikNativeAndroid.Adapters;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid.Fragments
{
    public class MyprofileFragment : Fragment, IHasToolbarTitle
    {
        private FriendRecycleListAdapter adapter;
        private Profile myProfile;

        public string GetTitle()
        {
            return GetString(Resource.String.myprofile);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_myprofile, container, false);

            adapter = new FriendRecycleListAdapter(new List<Friend> { Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake() });
            RecyclerView scrolling = view.FindViewById<RecyclerView>(Resource.Id.profile_friends_scroll_view);
            scrolling.SetLayoutManager(new LinearLayoutManager(container.Context, 0, false));
            scrolling.SetAdapter(adapter);

            ImageView avatar = view.FindViewById<ImageView>(Resource.Id.profile_image);
            ImageService.Instance
            .LoadUrl("https://images.unsplash.com/photo-1638104191847-3868d225bf93?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1075&q=80")
            .FadeAnimation(true)
            .Transform(new CornersTransformation(5, CornerTransformType.AllRounded))
            .Into(avatar);

            return view;
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


