﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AndroidX.Lifecycle;
using CrossPlatformLiveData;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.ViewModels
{
    public class ProfileViewModel : ViewModel
    {
        public ILiveData<User.Data> liveDataProfile = new LiveData<User.Data>();
        public ILiveData<List<Friend>> liveDataFriends = new LiveData<List<Friend>>();


        public async Task GetProfileAsync(int userId)
        {

            try
            {
                var data = await UsersService.GetUser(userId);
                liveDataProfile.PostValue(data.extraData);
                Console.WriteLine(1);
            }
            catch
            {
                Console.WriteLine((Resource.String.user_not_found));
            }
        }


        public async Task GetFriendsAsync(int userId)
        {

            try
            {
                var data = await UsersService.GetFriends(userId);
                liveDataFriends.PostValue(data);
            }
            catch
            {
                Console.WriteLine((Resource.String.user_not_found));
            }
        }
    }
}