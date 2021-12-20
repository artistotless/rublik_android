using System;
using System.Text;
using Android.Content;
using Java.Util;
using NETCore.Encrypt;

namespace RublikNativeAndroid.Utils
{
    public class SimpleCryptor
    {
        private SharedPreferencesWrapper _preferences;
        private const string _UUID_KEY = "UUID";
        private string _uuid;

        public SimpleCryptor(Context context)
        {
            _preferences = new SharedPreferencesWrapper(context, Constants.Preferences.CRYPTOR);
            if (!_preferences.Reader.Contains(_UUID_KEY))
                _preferences.Writer.PutString(_UUID_KEY, UUID.RandomUUID().ToString()).Apply();
            _uuid = ToLength(_preferences.Reader.GetString(_UUID_KEY, string.Empty),32);
        }

        private SimpleCryptor() { }

        public string Encrypt(string rawData)
        {
            Console.WriteLine(_uuid);
            Console.WriteLine(_uuid.Length);
            return EncryptProvider.AESEncrypt(rawData, _uuid);
        }

        public string Decrypt(string cypherData)
        {
            return EncryptProvider.AESDecrypt(cypherData, _uuid);
        }

        private string ToLength(string text, int newLength)
        {
            int lenghtText = text.Length;
            StringBuilder builder = new StringBuilder(newLength);
            for (int i = 0; i < newLength; i++)
            {
                if (i > lenghtText)
                    builder.Append("x");
                else
                    builder.Append(text[i].ToString());
            }
            string res = builder.ToString();
            return res;
        }
    }
}
