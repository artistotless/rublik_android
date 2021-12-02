using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RublikNativeAndroid.Exceptions;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{
    internal static class UsersService
    {
        public static async Task<User> GetUser(int id)
        {
            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(Constants.WebApiUrls.API_GET_USER + "/" + id.ToString());
            request.Method = HttpMethod.Get;
            HttpResponseMessage response = await client.SendAsync(request);

            Console.WriteLine($"statusCode - {response.StatusCode}");
            string content = await response.Content.ReadAsStringAsync();
            User.Data extraData = null;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await Task.Factory.StartNew(delegate { extraData = JsonConvert.DeserializeObject<User.Data>(content); });
                Console.WriteLine("Deserialize is done!");
                if (extraData == null)
                    throw new UserDataDeserializeException();
                return new User(extraData);
            }

            throw new UserNotFoundException($"User with the id - {id} was not found");
        }


        public static User GetUser(string accessKey) { return null; }
    }
}
