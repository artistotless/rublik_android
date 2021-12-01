using System;
using System.Net.Http;
using System.Threading.Tasks;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid
{
    public class LoginViewModel
    {
        private ITaskListener<string, string> _listener;

        public LoginViewModel(ITaskListener<string, string> listener)
        {
            _listener = listener;
        }

        public async Task LoginAsync(string username, string password)
        {
            _listener.OnPrepare();
            HttpClient client = new HttpClient();
            var body = string.Format("{{ \"username\":\"{0}\" , \"password\":\"{1}\"}}", username, password);

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(Constants.WebApiUrls.API_LOGIN);
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

