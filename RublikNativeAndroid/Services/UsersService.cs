using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RublikNativeAndroid.Exceptions;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{

    internal static class UsersService
    {

        public static User myUser { get; set; }


        public static async Task<User> GetUser(int id)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(string.Format(Constants.WebApiUrls.API_GET_USER, id));
            request.Method = HttpMethod.Get;
            HttpResponseMessage response = await client.SendAsync(request);

            Console.WriteLine($"statusCode - {response.StatusCode}");
            string content = await response.Content.ReadAsStringAsync();
            User.Data extraData = null;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                extraData = JsonConvert.DeserializeObject<User.Data>(content);
                Console.WriteLine("Deserialize is done!");
                if (extraData == null)
                    throw new UserDataDeserializeException();
                return new User(extraData);
            }

            throw new UserNotFoundException($"User with the id - {id} was not found");
        }



        public static async Task<List<Friend>> GetFriends(int userId, string accessKey)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(string.Format(Constants.WebApiUrls.API_GET_FRIENDS, userId, accessKey));
            request.Method = HttpMethod.Get;
            HttpResponseMessage response = await client.SendAsync(request);

            Console.WriteLine($"statusCode - {response.StatusCode}");
            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var friends = JsonConvert.DeserializeObject<List<Friend>>(content);
                Console.WriteLine("Deserialize is done!");
                if (friends == null)
                    throw new UserDataDeserializeException();
                return friends;
            }

            throw new UserNotFoundException($"User with the id - {userId} was not found");
        }

        public static async Task<List<Friend>> GetFriends(int userId) { return await GetFriends(userId, string.Empty); }

        public static async Task<string> GetAvatarUrl(int userId)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(string.Format(Constants.WebApiUrls.API_GET_AVATAR, userId));
            request.Method = HttpMethod.Get;
            HttpResponseMessage response = await client.SendAsync(request);

            Console.WriteLine($"statusCode - {response.StatusCode}");
            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return string.Format(Constants.WebApiUrls.FS_AVATAR, content);
            }

            throw new UserNotFoundException($"User with the id - {userId} was not found");
        }
    }
}
