using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;

namespace RublikNativeAndroid
{
    public static class SupportFragmentManagerExtension
    {
        public static void ShowFragment(this FragmentManager manager, int containerViewId, Fragment fragment, bool addToBackStack = true)
        {
            if (!addToBackStack)
                manager.BeginTransaction().Replace(containerViewId, fragment).Commit();
            else
                manager.BeginTransaction().Replace(containerViewId, fragment).AddToBackStack(null).Commit();
        }
    }

    public static class FragmentNavigatorExtension
    {
        public static Contracts.INavigator Navigator(this Fragment fragment) => fragment.RequireActivity() as Contracts.INavigator;
    }

    public static class FindWidgetIdExtension
    {
        public static Button FindButton(this View view, int resId) => view.FindViewById<Button>(resId);

        public static TextView FindTextView(this View view, int resId) => view.FindViewById<TextView>(resId);

        public static EditText FindEditText(this View view, int resId) => view.FindViewById<EditText>(resId);

        public static ImageView FindImageView(this View view, int resId) => view.FindViewById<ImageView>(resId);

        public static ImageButton FindImageButton(this View view, int resId) => view.FindViewById<ImageButton>(resId);

        public static CheckBox FindCheckBox(this View view, int resId) => view.FindViewById<CheckBox>(resId);

        public static RadioButton FindRadioButton(this View view, int resId) => view.FindViewById<RadioButton>(resId);

        public static ProgressBar FindProgressBar(this View view, int resId) => view.FindViewById<ProgressBar>(resId);

        public static RecyclerView FindRecyclerView(this View view, int resId) => view.FindViewById<RecyclerView>(resId);

        public static SwipeRefreshLayout FindRefreshLayout(this View view, int resId) => view.FindViewById<SwipeRefreshLayout>(resId);



    }
}
