using System;
namespace RublikNativeAndroid
{
    public static class Constants
    {
        public static class Fragments
        {
            public const string ACCESS_KEY = "ACCESS_KEY";
        }

        public static class WebApiUrls
        {
            private const string API_SCHEME = "https://";
            private const string API_DOMAIN = "server.bozieff.ru";

            public const string API_LOGIN =  API_SCHEME + API_DOMAIN + "/api/Login";
            public const string API_GET_USER = API_SCHEME + API_DOMAIN + "/api/user/Profile";
            public const string API_REGISTER = API_SCHEME + API_DOMAIN + "/api/Register";
            //public const string API_GET_FRIENDS = API_SCHEME + API_DOMAIN + "/api/Friend";
        }
    }
}
