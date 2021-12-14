using System;
namespace RublikNativeAndroid
{
    public static class Constants
    {
        public static class Fragments
        {
            public const string ACCESS_KEY = "ACCESS_KEY";
            public const string USER_ID = "USER_ID";
            public const string USER_NAME = "USER_NAME";
            public const string IP = "IP";
            public const string PORT = "PORT";
        }

        public static class Numbers
        {
            public static class GameEnumCodeStart
            {
                public const int ShellGame = 7;
            }
        }

        public static class Services
        {
            public const string LOBBY_IP = "192.168.43.14";
            public const int LOBBY_PORT = 9053;
            public const string MESSENGER_IP = "192.168.43.44";
            public const int MESSENGER_PORT = 9052;
        }

        public static class WebApiUrls
        {
            private const string HTTPS_SCHEME = "https://";
            private const string HTTP_SCHEME = "http://";
            private const string API_DOMAIN = "server.bozieff.ru";
            private const string FS_DOMAIN = "unity3ddd.ru";

            public const string FS_AVATAR = HTTP_SCHEME + FS_DOMAIN + "{0}";
            public const string API_LOGIN = HTTPS_SCHEME + API_DOMAIN + "/api/Login";
            public const string API_GET_USER = HTTPS_SCHEME + API_DOMAIN + "/api/user/Profile/{0}";
            public const string API_GET_FRIENDS = HTTPS_SCHEME + API_DOMAIN + "/api/Friend?userId={0}&accessKey={1}";
            public const string API_GET_AVATAR = HTTPS_SCHEME + API_DOMAIN + "/api/user/GetAvatar?userId={0}";
            public const string API_REGISTER = HTTPS_SCHEME + API_DOMAIN + "/api/Register";
            //public const string API_GET_FRIENDS = API_SCHEME + API_DOMAIN + "/api/Friend";
        }
    }
}
