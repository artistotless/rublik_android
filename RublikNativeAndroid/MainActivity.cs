using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.BottomNavigation;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Fragments;

namespace RublikNativeAndroid
{

    public class FragmentLifecycleListener : AndroidX.Fragment.App.FragmentManager.FragmentLifecycleCallbacks
    {
        public IFragmentViewCreateListener _listener { get; private set; }
        public FragmentLifecycleListener(IFragmentViewCreateListener listener) { _listener = listener; }
        public override void OnFragmentViewCreated(AndroidX.Fragment.App.FragmentManager fm, AndroidX.Fragment.App.Fragment f, View v, Bundle savedInstanceState)
        {
            base.OnFragmentViewCreated(fm, f, v, savedInstanceState);
            _listener.UpdateActivityUI(f);
        }
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener, INavigator, IFragmentViewCreateListener
    {
        private FragmentLifecycleListener _fragmentLifecycleListener { get; set; }
        private AndroidX.AppCompat.Widget.Toolbar _toolbar { get; set; }
        public TextView textMessage { get; set; }
        public INavigator mainNavigator { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            mainNavigator = this;
            _fragmentLifecycleListener = new FragmentLifecycleListener(this);
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            if (savedInstanceState == null)
                ShowLoginPage();
            _toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.main_toolbar);
            SetSupportActionBar(_toolbar);
            SupportFragmentManager.RegisterFragmentLifecycleCallbacks(_fragmentLifecycleListener, false);

        }

        //textMessage = FindViewById<TextView>(Resource.Id.message);
        //BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
        //navigation.SetOnNavigationItemSelectedListener(this);        

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    textMessage.Text="title_home";
                    return true;
                case Resource.Id.navigation_dashboard:
                    textMessage.Text = "title_dashboard";
                    return true;
                case Resource.Id.navigation_notifications:
                    textMessage.Text = "title_notifications";
                    return true;
            }

            return false;
        }

        public void UpdateActivityUI(AndroidX.Fragment.App.Fragment fragment)
        {
            bool needForBackButton = SupportFragmentManager.BackStackEntryCount > 0;
            SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(needForBackButton);
            SupportActionBar.SetDisplayShowHomeEnabled(needForBackButton);

            if (fragment is IHasToolbarAction hasToolbarAction)
            {
                CustomToolbarAction customAction = hasToolbarAction.GetAction();

            }

            if (fragment is IHasToolbarTitle hasToolbarTitle)
            {
                string title = hasToolbarTitle.GetTitle();
                SupportActionBar.Title = title;
            }
        }


        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void ShowMyProfilePage(string accessKey,int userId)
        {
            SupportFragmentManager.BeginTransaction().
                Replace(Resource.Id.viewPager, MyprofileFragment.NewInstance(accessKey, userId)).
                Commit();
        }

        public void ShowMessenger(int userId)
        {
            throw new NotImplementedException();
        }

        public void ShowProfilePage(int userId)
        {
            throw new NotImplementedException();
        }

        public void ShowSettingsPage()
        {
            throw new NotImplementedException();
        }

        public void ShowFriendsPage()
        {
            throw new NotImplementedException();
        }

        public void ShowServicesPage()
        {
            throw new NotImplementedException();
        }

        public void ShowLobbyPage(int gameId)
        {
            throw new NotImplementedException();
        }

        public void ShowLoginPage()
        {
            SupportFragmentManager.ShowFragment(Resource.Id.viewPager, new LoginFragment());
        }

        public void ShowRegisterPage()
        {
            SupportFragmentManager.ShowFragment(Resource.Id.viewPager, new RegisterFragment());
        }

    }
}

