using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Utils;

namespace RublikNativeAndroid
{

    internal class LoginFragment : Fragment, IHasToolbarTitle, IHideBottomNav, ITaskListener<User.Data, string>
    {
        private Button _btnLogin, _btnToRegister;
        private EditText _usernameField, _passwordField;
        private ProgressBar _bar;

        private LoginViewModel _loginViewModel;
        private SimpleCryptor _cryptor;
        private SharedPreferencesWrapper _preferences;

        public string GetTitle() => GetString(Resource.String.login);


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _loginViewModel = new LoginViewModel(this);
            _cryptor = new SimpleCryptor(Context);
            _preferences = new SharedPreferencesWrapper(Context, Constants.Preferences.LOGIN);
        }

        private string EncryptData(string rawData) => _cryptor.Encrypt(rawData);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_login, container, false);

            _btnLogin = rootView.FindButton(Resource.Id.btn_login);
            _btnToRegister = rootView.FindButton(Resource.Id.btn_to_register);
            _usernameField = rootView.FindEditText(Resource.Id.et_email);
            _passwordField = rootView.FindEditText(Resource.Id.et_password);
            _bar = rootView.FindProgressBar(Resource.Id.login_progress);

            _btnToRegister.Click += (object obj, EventArgs args) => { this.Navigator().ShowRegisterPage(); };
            _btnLogin.Click += async (object sender, EventArgs e) => await _loginViewModel.LoginAsync(_usernameField.Text, _passwordField.Text);
            return rootView;
        }


        public void OnPrepare()
        {
            _btnLogin.Enabled = false;
            _bar.Visibility = ViewStates.Visible;
        }

        public void OnError(string error)
        {
            _usernameField.Error = string.IsNullOrEmpty(error) ? GetString(Resource.String.novalid_login) : error;
            _btnLogin.Enabled = true;
            _bar.Visibility = ViewStates.Gone;
        }

        public void OnSuccess(User.Data data)
        {
            _preferences.Writer.PutString("username", EncryptData(_usernameField.Text)).Apply();
            _preferences.Writer.PutString("password", EncryptData(_passwordField.Text)).Apply();
            this.Navigator().ShowMyProfilePage();
        }

    }


}