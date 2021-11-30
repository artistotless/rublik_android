using AndroidX.Fragment.App;

namespace RublikNativeAndroid.Utils
{
    public static class SupportFragmentManagerExtension
    {
        public static void ShowFragment(this FragmentManager manager, int containerViewId, Fragment fragment)
        {
            manager.BeginTransaction().Add(containerViewId, fragment).Commit();
        }
    }
}
