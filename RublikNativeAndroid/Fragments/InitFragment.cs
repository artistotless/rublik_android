using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Utils;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Fragments
{
    public class InitFragment : Fragment, IHasToolbarTitle, IHideBottomNav, ITaskListener<User.Data, string>
    {
        private SharedPreferencesWrapper _preferences;
        private LoginViewModel _loginViewModel;
        private SimpleCryptor _cryptor;
        private string _username, _password;

        public string GetTitle() => GetString(Resource.String.empty);

        public override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            _preferences = new SharedPreferencesWrapper(Context, Constants.Preferences.LOGIN);
            if (!_preferences.Reader.Contains("username"))
                this.Navigator().ShowLoginPage();

            _cryptor = new SimpleCryptor(Context);
            _loginViewModel = new LoginViewModel(this);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_init, container, false);
            rootView.ViewAttachedToWindow += RootView_ViewAttachedToWindow;
            return rootView;
        }

        private async void RootView_ViewAttachedToWindow(object sender, View.ViewAttachedToWindowEventArgs e)
        {
            _username = _cryptor.Decrypt(_preferences.Reader.GetString("username", string.Empty));
            _password = _cryptor.Decrypt(_preferences.Reader.GetString("password", string.Empty));
            await _loginViewModel.LoginAsync(_username, _password);
        }

        public void OnError(string error)
        {
            _preferences.Writer.Clear().Apply();
            this.Navigator().ShowLoginPage();
        }

        public void OnPrepare() { }

        public void OnSuccess(User.Data result) => this.Navigator().ShowMyProfilePage();

    }
}
