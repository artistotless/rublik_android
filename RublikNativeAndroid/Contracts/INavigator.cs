using System;
using RublikNativeAndroid.Fragments;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    public interface INavigator
    {
        void ShowMessenger(int userId);
        void ShowProfilePage(int userId);
        void ShowGamePage(Room room, ServerEndpoint endpoint);
        void ShowGameResultPage(uint sum, GameResult status);
        void ShowLobbyPage(int gameId);
        void ShowGameSelectPage(Action<Game> callback);
        void ShowLoginPage();
        void ShowRegisterPage();
        void ShowMyProfilePage();
        void ShowSettingsPage();
        void ShowFriendsPage();
        void ShowServicesPage();
        void ShowRoomsPage();
        void GoBack();
    }
}
