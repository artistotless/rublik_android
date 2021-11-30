using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace RublikNativeAndroid
{
    public static class SupportFragmentManagerExtension
    {
        public static void ShowFragment(this FragmentManager manager, int containerViewId, Fragment fragment)
        {
            manager.BeginTransaction().Replace(containerViewId, fragment).Commit();
        }
    }

    public static class FragmentNavigatorExtension
    {
        public static Contracts.INavigator Navigator(this Fragment fragment)=> fragment.RequireActivity() as Contracts.INavigator;
    }

    public static class FindWidgetIdExtension
    {
        public static Button FindButton(this View view, int resId) => view.FindViewById<Button>(resId);

        public static TextView FindTextView(this View view, int resId) => view.FindViewById<TextView>(resId);

        public static EditText FindEditText(this View view, int resId) => view.FindViewById<EditText>(resId);

        public static ImageButton FindImageButton(this View view, int resId) => view.FindViewById<ImageButton>(resId);

        public static CheckBox FindCheckBox(this View view, int resId) => view.FindViewById<CheckBox>(resId);

        public static RadioButton FindRadioButton(this View view, int resId) => view.FindViewById<RadioButton>(resId);

        public static ProgressBar FindProgressBar(this View view, int resId) => view.FindViewById<ProgressBar>(resId);

    }
}
