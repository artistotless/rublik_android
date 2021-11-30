using System;
using System.Net.Http;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid
{
    internal class LoginFragment : Fragment, IHasToolbarTitle
    {
        public string GetTitle()
        {
            return GetString(Resource.String.login);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_login, container, false);
            
            Button btn_login = rootView.FindButton(Resource.Id.btn_login);
            Button btn_to_register = rootView.FindButton(Resource.Id.btn_to_register);
            EditText username_field = rootView.FindEditText(Resource.Id.et_email);
            EditText password_field = rootView.FindEditText(Resource.Id.et_password);
            ProgressBar bar = rootView.FindProgressBar(Resource.Id.login_progress);

            btn_to_register.Click += (object obj, EventArgs args) => { this.Navigator().ShowRegisterPage(); };

            btn_login.Click += async (object sender, EventArgs e) =>
            {
                btn_login.Enabled = false;
                bar.Visibility = ViewStates.Visible;

                HttpClient client = new HttpClient();
                var body = string.Format("{{ \"username\":\"{0}\" , \"password\":\"{1}\"}}", username_field.Text, password_field.Text);

                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(Constants.WebApiUrls.API_LOGIN);
                request.Method = HttpMethod.Post;
                request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.SendAsync(request);


                string content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var accessKey = content;

                    this.Navigator().ShowMyProfilePage(accessKey);
                }
                else
                {
                    username_field.Error = string.IsNullOrEmpty(content) ? GetString(Resource.String.novalid_login) : content;
                    btn_login.Enabled = true;
                    bar.Visibility = ViewStates.Gone;
                }

            };

            return rootView;
        }
    }
}