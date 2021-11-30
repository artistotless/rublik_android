using System;
using System.Net.Http;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid
{
    internal class RegisterFragment : Fragment, IHasToolbarTitle
    {
        public string GetTitle()
        {
            return GetString(Resource.String.register);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            base.OnCreateView(inflater, container, savedInstanceState);

            var rootView = inflater.Inflate(Resource.Layout.fragment_register, container, false);
            Button btn_register = rootView.FindButton(Resource.Id.btn_register);
            Button btn_to_login = rootView.FindButton(Resource.Id.btn_to_login);
            EditText username_field = rootView.FindEditText(Resource.Id.et_username);
            EditText password_field = rootView.FindEditText(Resource.Id.et_password);
            EditText email_field = rootView.FindEditText(Resource.Id.et_email);

            btn_to_login.Click += (object sender, EventArgs e) => { this.Navigator().ShowLoginPage(); };

            btn_register.Click += async (object sender, EventArgs e) =>
            {
                HttpClient client = new HttpClient();
                var body = string.Format("{{ \"username\":\"{0}\" , \"password\":\"{1}\", \"email\":\"{1}\"}}", username_field.Text, password_field.Text, email_field.Text);

                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(Constants.WebApiUrls.API_REGISTER);
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
                    username_field.Error = string.IsNullOrEmpty(content) ? GetString(Resource.String.novalid_register) : content;
                }

            };

            return rootView;
        }
    }
}