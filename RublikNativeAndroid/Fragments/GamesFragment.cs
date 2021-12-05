using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid.Fragments
{
    public class GamesFragment : Fragment, IHasToolbarTitle
    {
        public string GetTitle() => GetString(Resource.String.games);
    
        public override void OnCreate(Bundle savedInstanceState) { base.OnCreate(savedInstanceState); }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}
