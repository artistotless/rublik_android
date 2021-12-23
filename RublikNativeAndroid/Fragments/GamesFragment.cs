using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;
using AndroidX.RecyclerView.Widget;
using RublikNativeAndroid.Adapters;
using static Android.Views.View;
using Android.Widget;
using RublikNativeAndroid.Services;
using System.Threading.Tasks;
using System;

namespace RublikNativeAndroid.Fragments
{
    public class GamesFragment : Fragment, IHasToolbarTitle, IOnClickListener
    {
        public event Action<int> gameSelected;
        public const string TAG = "GAMES";

        private RecyclerView _gamesRecycler;
        private GamesRecycleViewAdapter _adapter;
        private ViewEventListener _viewEventListener;

        public string GetTitle() => GetString(Resource.String.games);

        public void OnClick(View v)
        {
            var gameId = (int)v.Tag;
            gameSelected(gameId);
            this.Navigator().GoBack();
        }

        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _viewEventListener = new ViewEventListener();
            _adapter = new GamesRecycleViewAdapter(this);
            await ApiService.GetGamesAsync();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_games, container, false);

            _gamesRecycler = rootView.FindRecyclerView(Resource.Id.games_recycler_view);
            _viewEventListener.AddListener(new OnViewAttached(_gamesRecycler,
                async delegate (object sender, ViewAttachedToWindowEventArgs e)
                {
                    await AttachAdapter(_adapter, container);
                }
                ));

            return rootView;
        }

        private async Task AttachAdapter(GamesRecycleViewAdapter adapter, ViewGroup container)
        {
            _gamesRecycler.SetLayoutManager(new LinearLayoutManager(container.Context, (int)Orientation.Vertical, false));
            _gamesRecycler.SetAdapter(adapter);
            _adapter.SetElements(await ApiService.GetGamesAsync());
        }
    }
}
