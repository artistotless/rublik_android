using Android.Content;

namespace RublikNativeAndroid.Utils
{
    public class SharedPreferencesWrapper
    {
        private Context _context;
        public ISharedPreferences Reader;
        public ISharedPreferencesEditor Writer;

        public SharedPreferencesWrapper(Context context, string name = Constants.Preferences.DEFAULT)
        {
            _context = context;
            Reader = _context.GetSharedPreferences(name, FileCreationMode.Private);
            Writer = Reader.Edit();
        }
    }
}
