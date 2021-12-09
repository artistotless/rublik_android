using System;
using System.Collections.Generic;
using RublikNativeAndroid.Adapters;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{
    public class LocalCacheService
    {
        public static Dictionary<int, User.Data> usersData { get; set; }
        public static Dictionary<int, FriendRecycleListAdapter> userFriendsAdapter { get; set; }

        public LocalCacheService()
        {
            usersData = new Dictionary<int, User.Data>();
            userFriendsAdapter = new Dictionary<int, FriendRecycleListAdapter>();
        }

        public User.Data GetUsersData(int userId)
        {
            if (usersData.ContainsKey(userId))
                return usersData[userId];
            return null;
        }

        public void AddUsersData(int userId, User.Data data) => usersData[userId] = data;

        public FriendRecycleListAdapter GetUserFriendsAdapter(int userId)
        {
            if (userFriendsAdapter.ContainsKey(userId))
                return userFriendsAdapter[userId];
            return null;
        }

        public void AddUserFriendsAdapter(int userId, FriendRecycleListAdapter friendsAdapter) => userFriendsAdapter[userId] = friendsAdapter;

    }
}
