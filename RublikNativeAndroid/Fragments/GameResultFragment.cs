using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;
using Android.Widget;
using Com.Airbnb.Lottie;
using System;

namespace RublikNativeAndroid.Fragments
{

    public enum GameResult
    {
        Win,
        Lose,
        Cancel
    }

    public class GameResultFragment : Fragment, IHasToolbarTitle, IHideBottomNav
    {
        private Button _goHome;
        private TextView _sumView;
        private TextView _header;
        private LottieAnimationView _confetti;

        private int _sum;
        private GameResult _resultStatus;

        public string GetTitle() => GetString(Resource.String.empty);

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _sum = Arguments.GetInt(Constants.Fragments.SUM);
            _resultStatus = (GameResult)Arguments.GetInt(Constants.Fragments.GAME_RESULT_STATUS);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_game_result, container, false);
            _goHome = rootView.FindButton(Resource.Id.game_result_goHome);
            _sumView = rootView.FindTextView(Resource.Id.game_result_win_sum);
            _header = rootView.FindTextView(Resource.Id.game_result_header);
            _confetti = rootView.FindLottie(Resource.Id.game_result_confetti);
            _confetti.SetAnimation("confetti.json");
            _confetti.SetMaxProgress(0.66f);
            _confetti.Loop(false);
            _goHome.Click += delegate (object sender, EventArgs e) { this.Navigator().ShowMyProfilePage(); };

            switch (_resultStatus)
            {
                case GameResult.Win:
                    _sumView.Text = $"+ {_sum} {Constants.Currency.MAIN}";
                    _sumView.SetTextColor(Android.Graphics.Color.ParseColor("#ff7cb342"));
                    _header.Text = GetString(Resource.String.youwon);
                    
                    _confetti.PlayAnimation();
                    break;

                case GameResult.Lose:
                    _sumView.Text = $"- {_sum} {Constants.Currency.MAIN}";
                    _sumView.SetTextColor(Android.Graphics.Color.ParseColor("#ffe53935"));
                    _header.Text = GetString(Resource.String.wasted);
                    break;

                case GameResult.Cancel:
                    _sumView.Text = string.Empty;
                    _header.Text = GetString(Resource.String.gamewascanceled);
                    break;
            }

            return rootView;
        }

        public static GameResultFragment NewInstance(uint sum, GameResult status)
        {
            Bundle bundle = new Bundle();
            bundle.PutInt(Constants.Fragments.SUM, (int)sum);
            bundle.PutInt(Constants.Fragments.GAME_RESULT_STATUS, (int)status);
            GameResultFragment fragment = new GameResultFragment();
            fragment.Arguments = bundle;
            return fragment;
        }
    }
}
