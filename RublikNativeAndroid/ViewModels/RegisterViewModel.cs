using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid
{
    public struct RegisterData
    {
        public string username;
        public string email;
        public string password;

        public RegisterData(string username, string email, string password)
        {
            this.username = username;
            this.email = email;
            this.password = password;
        }
    }

    public class RegisterViewModel
    {
        private ITaskListener<User.Data, string> _listener;

        public RegisterViewModel(ITaskListener<User.Data, string> listener)
        {
            _listener = listener;
        }

        public async Task RegisterAsync(RegisterData regData)
        {
            _listener.OnPrepare();

            /*// Offline mode

            var offlineUser = new User.Data()
            {
                accessKey = "user1",
                avatar = "/Files/126kb.jpg",
                balance = 1350,
                id = 1,
                isOnline = true,
                nickname = "Artistotless",
                quote = "just quote",
                username = "admin"
            };
            UsersService.myUser = new User(offlineUser);
            _listener.OnSuccess(offlineUser);
            return;*/


            HttpClient client = new HttpClient();
            var body = string.Format("{{ \"username\":\"{0}\" , \"password\":\"{1}\", \"email\":\"{1}\"}}",
                regData.username, regData.password, regData.email);

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(Constants.WebApiUrls.API_REGISTER);
            request.Method = HttpMethod.Post;
            request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userData = JsonConvert.DeserializeObject<User.Data>(content);

                UsersService.myUser = new User(userData);

                _listener.OnSuccess(userData);
            }
            else
            {
                _listener.OnError(content);
            }
        }

    }

}

