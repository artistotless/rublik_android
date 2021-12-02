using System.Collections.Generic;
using System.Threading.Tasks;
using AndroidX.Lifecycle;
using CrossPlatformLiveData;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.ViewModels
{
    public class MyProfileViewModel : ViewModel
    {

        public ILiveData<Models.User> liveDataProfile = new LiveData<Models.User>();
        public ILiveData<List<Friend>> liveDataFriends = new LiveData<List<Friend>>();


        public async Task GetProfileAsync(int userId)
        {

            try
            {
                var data = await UsersService.GetUser(userId);
                liveDataProfile.PostValue(data);
            }
            catch
            {
                System.Console.WriteLine((Resource.String.user_not_found));
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
                System.Console.WriteLine((Resource.String.user_not_found));
            }
        }
    }
}

