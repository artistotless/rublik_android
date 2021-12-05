using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid.Fragments
{
    public class LobbyFragment : Fragment,IHasToolbarTitle
    {
        public string GetTitle() => GetString(Resource.String.lobby);
    
        public override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_lobby, container, false);
            return rootView;
        }
    }
}
