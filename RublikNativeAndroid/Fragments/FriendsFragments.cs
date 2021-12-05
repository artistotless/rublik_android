using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid.Fragments
{


    public class FriendsFragment : Fragment, IHasToolbarTitle
    {
        public string GetTitle() => GetString(Resource.String.friends);

        public override void OnCreate(Bundle savedInstanceState) { base.OnCreate(savedInstanceState); }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_friends, container, false);
            return rootView;
        }
    }
}
