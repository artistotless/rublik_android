using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid
{

    internal class LoginFragment : Fragment, IHasToolbarTitle, IHideBottomNav, ITaskListener<User.Data, string>
    {
        private LoginViewModel _loginViewModel;

        private Button btn_login, btn_to_register;
        private EditText username_field, password_field;
        private ProgressBar bar { get; set; }

        public string GetTitle() => GetString(Resource.String.login);


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _loginViewModel = new LoginViewModel(this);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_login, container, false);

            btn_login = rootView.FindButton(Resource.Id.btn_login);
            btn_to_register = rootView.FindButton(Resource.Id.btn_to_register);
            username_field = rootView.FindEditText(Resource.Id.et_email);
            password_field = rootView.FindEditText(Resource.Id.et_password);
            bar = rootView.FindProgressBar(Resource.Id.login_progress);

            btn_to_register.Click += (object obj, EventArgs args) => { this.Navigator().ShowRegisterPage(); };

            btn_login.Click += async (object sender, EventArgs e) => await _loginViewModel.LoginAsync(username_field.Text, password_field.Text);

            return rootView;
        }


        public void OnPrepare()
        {
            btn_login.Enabled = false;
            bar.Visibility = ViewStates.Visible;
        }

        public void OnError(string error)
        {
            username_field.Error = string.IsNullOrEmpty(error) ? GetString(Resource.String.novalid_login) : error;
            btn_login.Enabled = true;
            bar.Visibility = ViewStates.Gone;
        }

        public void OnSuccess(User.Data data)
        {

            this.Navigator().ShowMyProfilePage();
        }

    }


}