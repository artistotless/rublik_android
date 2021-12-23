using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RublikNativeAndroid.Exceptions;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{

    internal static class ApiService
    {

        public static User myUser { get; set; }

        private static LocalCacheService _localCacheService = new LocalCacheService();


        public static async Task<User> GetUserAsync(int userId, bool sync = false, bool ignoreCache = false)
        {
            if (!ignoreCache)
                if (_localCacheService.GetUsersData(userId) is User.Data dataCache)
                    return new User(dataCache);

            var extraData = await Get<User.Data>(string.Format(Constants.WebApiUrls.API_GET_USER, userId));
            _localCacheService.AddUsersData(userId, extraData);
            return new User(extraData);
        }

        public static async Task<List<Friend>> GetFriendsAsync(int userId, string accessKey = "", bool ignoreCache = false)
        {
            if (!ignoreCache)
                if (_localCacheService.GetUserFriends(userId) is List<Friend> friendsCache)
                    return friendsCache;

            var friends = await Get<List<Friend>>(string.Format(Constants.WebApiUrls.API_GET_FRIENDS, userId, accessKey));
            _localCacheService.AddUserFriends(userId, friends);
            return friends;
        }

        public static async Task<List<Friend>> GetFriendsAsync(int userId) { return await GetFriendsAsync(userId, string.Empty); }

        public static async Task<string> GetAvatarUrlAsync(int userId)
        {
            HttpResponseMessage response = await BaseHttpRequest.Get(string.Format(Constants.WebApiUrls.API_GET_AVATAR, userId));
            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return string.Format(Constants.WebApiUrls.FS_AVATAR, content);
            throw new UserNotFoundException($"User with the id - {userId} was not found");
        }

        public static async Task<Game> GetGameInfoAsync(int id, bool ignoreCache = false)
        {
            if (!ignoreCache)
                if (_localCacheService.GetGame(id) is Game dataCache)
                    return dataCache;

            var games = await Get<List<Game>>(string.Format(Constants.WebApiUrls.API_GET_GAME_INFO, id, Constants.languageCode));
            return games.FirstOrDefault();
        }

        public static async Task<List<Game>> GetGamesAsync(bool ignoreCache = false)
        {
            if (!ignoreCache)
                if (_localCacheService.IsAllGamesLoaded)
                    return _localCacheService.GetAllGames();

            List<Game> games = await Get<List<Game>>(string.Format(Constants.WebApiUrls.API_GET_GAMES, Constants.languageCode));
            _localCacheService.SetGames(games);
            return games;
        }

        public static async Task<T> Get<T>(string url)
        {
            HttpResponseMessage response = await BaseHttpRequest.Get(url);
            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<T>(content) ??
                    throw new DeserializeException();
            throw new ItemNotFoundException($"Item was not found");
        }
    }
}

public class BaseHttpRequest
{
    public static async Task<HttpResponseMessage> Get(string url)
    {
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage();
        request.RequestUri = new Uri(url);
        request.Method = HttpMethod.Get;
        HttpResponseMessage response = await client.SendAsync(request);
        return response;
    }
}

