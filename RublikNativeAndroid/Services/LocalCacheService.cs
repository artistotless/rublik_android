using System.Collections.Generic;
using System.Linq;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{
    public class LocalCacheService
    {
        public Dictionary<int, User.Data> UsersData { get; private set; }
        public Dictionary<int, List<Friend>> UserFriends { get; private set; }
        public Dictionary<int, Game> Games { get; private set; }

        public LocalCacheService()
        {
            UsersData = new Dictionary<int, User.Data>();
            UserFriends = new Dictionary<int, List<Friend>>();
            Games = new Dictionary<int, Game>();
        }

        public User.Data GetUsersData(int userId) => (UsersData.ContainsKey(userId)) ? UsersData[userId] : null;
        public void AddUsersData(int userId, User.Data data) => UsersData[userId] = data;

        public List<Friend> GetUserFriends(int userId) => (UserFriends.ContainsKey(userId)) ? UserFriends[userId] : null;
        public void AddUserFriends(int userId, List<Friend> friends) => UserFriends[userId] = friends;

        public Game GetGame(int id) => (Games.ContainsKey(id)) ? Games[id] : null;
        public bool IsAllGamesLoaded { get; set; }
        public List<Game> GetAllGames() => Games.Values.ToList();
        public void AddGame(int id, Game data) => Games[id] = data;
        public void SetGames(List<Game> games)
        {
            foreach (var game in games)
                Games[game.id] = game;
        }


    }
}
