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
        private static LocalCacheService _localCacheService = LocalCacheService.NewInstance();

        public static async Task<User> GetUserAsync(int userId, bool sync = false, bool ignoreCache = false)
        {
            /* // Offline mode

             switch (id)
             {
                 case 1:
                     return new User(new User.Data()
                     {
                         accessKey = "user1",
                         avatar = "/Files/126kb.jpg",
                         balance = 1350,
                         id = 1,
                         isOnline = true,
                         nickname = "Artistotless",
                         quote = "just quote",
                         username = "admin"
                     });

                 case 4:
                     return new User(new User.Data()
                     {
                         accessKey = "user4",
                         avatar = "/Files/33kb.jpg",
                         balance = 15,
                         id = 4,
                         isOnline = true,
                         nickname = "KennyS",
                         quote = "bebra",
                         username = "kenya"
                     });


                 case 6:
                     return new User(new User.Data()
                     {
                         accessKey = "user6",
                         avatar = "/Files/admin.jpg",
                         balance = 800,
                         id = 6,
                         isOnline = true,
                         nickname = "BoomBoombI4",
                         quote = "cmon lulz",
                         username = "suser"
                     });
             }
            */
            if (!ignoreCache)
                if (_localCacheService.GetUsersData(userId) is User.Data dataCache)
                    return new User(dataCache);

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(string.Format(Constants.WebApiUrls.API_GET_USER, userId));
            request.Method = HttpMethod.Get;
            HttpResponseMessage response;
            if (sync)
                response = client.SendAsync(request).Result;
            else
                response = await client.SendAsync(request);

            Console.WriteLine($"statusCode - {response.StatusCode}");
            string content;
            if (sync)
                content = response.Content.ReadAsStringAsync().Result;
            else
                content = await response.Content.ReadAsStringAsync();
            User.Data extraData = null;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                extraData = JsonConvert.DeserializeObject<User.Data>(content);
                Console.WriteLine("Deserialize is done!");
                if (extraData == null)
                    throw new UserDataDeserializeException();
                _localCacheService.AddUsersData(userId, extraData);
                return new User(extraData);
            }

            throw new UserNotFoundException($"User with the id - {userId} was not found");
        }



        public static async Task<List<Friend>> GetFriendsAsync(int userId, string accessKey="", bool ignoreCache = false)
        {
            /*// Offline mode

            List<Friend> result = new List<Friend>();

            switch (userId)
            {
                case 1:
                    result.Add(new Friend() { avatarUrl = "/Files/33kb.jpg", id = 1, status = 0, userId = 4 });
                    result.Add(new Friend() { avatarUrl = "/Files/admin.jpg", id = 2, status = 0, userId = 6 });
                    return result;

                case 4:
                    result.Add(new Friend() { avatarUrl = "/Files/126kb.jpg", id = 1, status = 0, userId = 1 });
                    return result;

                case 6:
                    result.Add(new Friend() { avatarUrl = "/Files/126kb.jpg", id = 2, status = 0, userId = 1 });
                    return result;
            }*/


            if (!ignoreCache)
                if (_localCacheService.GetUserFriends(userId) is List<Friend> friendsCache)
                    return friendsCache;

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
                _localCacheService.AddUserFriends(userId, friends);
                return friends;
            }

            throw new UserNotFoundException($"User with the id - {userId} was not found");
        }

        public static async Task<List<Friend>> GetFriendsAsync(int userId) { return await GetFriendsAsync(userId, string.Empty); }

        public static async Task<string> GetAvatarUrlAsync(int userId)
        {

            /*// Offline mode


            switch (userId)
            {
                case 1:
                    return string.Format(Constants.WebApiUrls.FS_AVATAR, "/Files/126kb.jpg");

                case 4:
                    return string.Format(Constants.WebApiUrls.FS_AVATAR, "/Files/33kb.jpg");

                case 6:
                    return string.Format(Constants.WebApiUrls.FS_AVATAR, "/Files/admin.jpg");
            }*/


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
