using System.Collections.Generic;
using AndroidX.Fragment.App;
using RublikNativeAndroid.Fragments;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid
{
    public enum GamesId
    {
        ShellGame = 1,
    }

    public delegate Fragment NewInstance(ServerEndpoint endpoint, uint award);

    public class GameFragmentFactory
    {
        private GameFragmentFactory() { }

        static Dictionary<GamesId, NewInstance> _references = new Dictionary<GamesId, NewInstance>
        {
            { GamesId.ShellGame, ShellGameFragment.NewInstance},
        };

        public static Fragment CreateNew(int gameId, ServerEndpoint endpoint, uint award)
        {
            return _references[(GamesId)gameId].Invoke(endpoint, award);
        }
    }
}