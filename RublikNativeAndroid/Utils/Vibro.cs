using Android.Content;
using Android.OS;

namespace RublikNativeAndroid.Utils
{
    public class Vibro
    {
        private static Vibro _instance;
        private static Vibrator _vibrator;

        public static Vibro Instance
        {
            get
            {
                _instance = _instance ?? new Vibro();
                return _instance;
            }
        }

        private Vibro() { }

        public void Peep(Context context, long milliseconds, int amplitude)
        {
            _vibrator = Vibrator.FromContext(context);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                _vibrator.Vibrate(VibrationEffect.CreateOneShot(milliseconds, amplitude));
            else
                //deprecated in API 26 
                _vibrator.Vibrate(milliseconds);
        }

    }
}
