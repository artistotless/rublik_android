using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid
{
    internal class RegisterFragment : Fragment, IHasToolbarTitle, IHideBottomNav, ITaskListener<User.Data, string>
    {
        private RegisterViewModel _registerViewModel;

        private Button btn_register;
        private EditText username_field, password_field, email_field;


        public string GetTitle()
        {
            return GetString(Resource.String.register);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _registerViewModel = new RegisterViewModel(this);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            base.OnCreateView(inflater, container, savedInstanceState);

            var rootView = inflater.Inflate(Resource.Layout.fragment_register, container, false);
            btn_register = rootView.FindButton(Resource.Id.btn_register);
            username_field = rootView.FindEditText(Resource.Id.et_username);
            password_field = rootView.FindEditText(Resource.Id.et_password);
            email_field = rootView.FindEditText(Resource.Id.et_email);

            btn_register.Click += async (object sender, EventArgs e) => await _registerViewModel.RegisterAsync(
                new RegisterData(username_field.Text, email_field.Text, password_field.Text));



            return rootView;
        }

        public void OnError(string error)
        {
            username_field.Error = string.IsNullOrEmpty(error) ? GetString(Resource.String.novalid_register) : error;

        }

        public void OnPrepare()
        {
            return;
        }

        public void OnSuccess(User.Data data)
        {
            this.Navigator().ShowMyProfilePage();
        }
    }
}