using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using System.Collections.Generic;


namespace RublikNativeAndroid
{

   
    }

    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : Activity
    {
        private FriendRecycleListAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_profile);

            RecyclerView scrolling = FindViewById<RecyclerView>(Resource.Id.profile_friends_scroll_view);
            scrolling.SetLayoutManager(new LinearLayoutManager(this, 0, false));
            ImageView avatar = FindViewById<ImageView>(Resource.Id.profile_image);
            adapter = new FriendRecycleListAdapter(new List<Friend> { Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(), Friend.Fake(),Friend.Fake(), Friend.Fake(), Friend.Fake()},this);
            scrolling.SetAdapter(adapter);

            ImageService.Instance
       .LoadUrl("https://images.unsplash.com/photo-1638104191847-3868d225bf93?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1075&q=80")
       .FadeAnimation(true)
       .Transform(new CornersTransformation(5,CornerTransformType.AllRounded))
       .Into(avatar);
        }
    }
}