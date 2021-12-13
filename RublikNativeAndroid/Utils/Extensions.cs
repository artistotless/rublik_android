using System;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Com.Airbnb.Lottie;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid
{
    public static class SupportFragmentManagerExtension
    {
        public static void ShowFragment(this FragmentManager manager, Fragment fragment, bool addToBackStack = true)
        {
            if (!addToBackStack)
                manager.BeginTransaction().Replace(Resource.Id.viewPager, fragment).Commit();
            else
                manager.BeginTransaction().Replace(Resource.Id.viewPager, fragment).AddToBackStack(null).Commit();
        }
    }

    public static class FragmentNavigatorExtension
    {
        public static INavigator Navigator(this Fragment fragment) => fragment.RequireActivity() as INavigator;
        public static ICacheServiceAccessor Cache(this Fragment fragment) => fragment.RequireActivity() as ICacheServiceAccessor;
    }

    public static class FragmentMessengerExtension
    {
        public static IMessengerInteractor Messenger(this Fragment fragment)
        {
            if (fragment is IMessengerListener)
                return fragment.RequireActivity() as IMessengerInteractor;
            throw new InvalidCastException($"Fragment #{fragment.Id} does not have IMessengerListener interface");
        }
    }

    public static class FindWidgetIdExtension
    {
        public static Button FindButton(this View view, int resId) => view.FindViewById<Button>(resId);

        public static TextView FindTextView(this View view, int resId) => view.FindViewById<TextView>(resId);

        public static LinearLayout FindLinearLayout(this View view, int resId) => view.FindViewById<LinearLayout>(resId);

        public static EditText FindEditText(this View view, int resId) => view.FindViewById<EditText>(resId);

        public static ImageView FindImageView(this View view, int resId) => view.FindViewById<ImageView>(resId);

        public static ImageButton FindImageButton(this View view, int resId) => view.FindViewById<ImageButton>(resId);

        public static CheckBox FindCheckBox(this View view, int resId) => view.FindViewById<CheckBox>(resId);

        public static RadioButton FindRadioButton(this View view, int resId) => view.FindViewById<RadioButton>(resId);

        public static ProgressBar FindProgressBar(this View view, int resId) => view.FindViewById<ProgressBar>(resId);

        public static RecyclerView FindRecyclerView(this View view, int resId) => view.FindViewById<RecyclerView>(resId);

        public static SwipeRefreshLayout FindRefreshLayout(this View view, int resId) => view.FindViewById<SwipeRefreshLayout>(resId);

        public static LottieAnimationView FindLottie(this View view, int resId) => view.FindViewById<LottieAnimationView>(resId);

    }
}
