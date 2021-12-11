﻿using System;
using System.Collections.Generic;
using RublikNativeAndroid.Adapters;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Services
{
    public class LocalCacheService
    {
        public static Dictionary<int, User.Data> usersData { get; set; }
        public static Dictionary<int, List<Friend>> userFriends { get; set; }

        public LocalCacheService()
        {
            usersData = new Dictionary<int, User.Data>();
            userFriends = new Dictionary<int, List<Friend>>();
        }

        public User.Data GetUsersData(int userId)
        {
            if (usersData.ContainsKey(userId))
                return usersData[userId];
            return null;
        }

        public void AddUsersData(int userId, User.Data data) => usersData[userId] = data;

        public List<Friend> GetUserFriends(int userId)
        {
            if (userFriends.ContainsKey(userId))
                return userFriends[userId];
            return null;
        }

        public void AddUserFriends(int userId, List<Friend> friends) => userFriends[userId] = friends;

    }
}