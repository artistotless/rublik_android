using System;
using System.Net.Http;
using System.Threading.Tasks;
using RublikNativeAndroid.Contracts;

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
        private ITaskListener<string, string> _listener;

        public RegisterViewModel(ITaskListener<string, string> listener)
        {
            _listener = listener;
        }

        public async Task RegisterAsync(RegisterData regData)
        {
            _listener.OnPrepare();
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
                _listener.OnSuccess(content);
            }
            else
            {
                _listener.OnError(content);
            }
        }

    }

}

