﻿namespace RublikNativeAndroid.Contracts
{
    public interface INavigator
    {
        void GoBack();
        void ShowLoginPage();
        void ShowRegisterPage();
        void ShowMyProfilePage();
        void ShowMessenger(int userId);
        void ShowProfilePage(int userId);
        void ShowSettingsPage();
        void ShowFriendsPage();
        void ShowServicesPage();
        void ShowRoomsPage();
        void ShowLobbyPage(int gameId);
    }
}
