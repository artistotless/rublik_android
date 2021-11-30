namespace RublikNativeAndroid.Contracts
{
    public interface INavigator
    {
        void GoBack();
        void ShowLoginPage();
        void ShowRegisterPage();
        void ShowMyProfilePage(string accessKey);
        void ShowMessenger(int userId);
        void ShowProfilePage(int userId);
        void ShowSettingsPage();
        void ShowFriendsPage();
        void ShowServicesPage();
        void ShowLobbyPage(int gameId);
    }
}
