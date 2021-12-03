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
    public class MainActivity : AppCompatActivity, INavigator, IFragmentViewCreateListener, IMenuItemOnMenuItemClickListener
    {
        private FragmentLifecycleListener _fragmentLifecycleListener { get; set; }
        private AndroidX.AppCompat.Widget.Toolbar _toolbar { get; set; }
        public TextView textMessage { get; set; }
        public INavigator mainNavigator { get; set; }
        private AndroidX.Fragment.App.Fragment _currentFragment => SupportFragmentManager.FindFragmentById(Resource.Id.viewPager);

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

        public override bool OnSupportNavigateUp()
        {
            GoBack();
            return true;
        }


        public void UpdateActivityUI(AndroidX.Fragment.App.Fragment fragment)
        {
            bool needForBackButton = SupportFragmentManager.BackStackEntryCount > 0;

            SupportActionBar.SetDisplayHomeAsUpEnabled(needForBackButton);
            SupportActionBar.SetDisplayShowHomeEnabled(needForBackButton);

            _toolbar.Menu.Clear();

            if (fragment is IHasToolbarAction hasToolbarAction)
            {
                CustomToolbarAction customAction = hasToolbarAction.GetAction();

                var menuItem = _toolbar.Menu.Add(customAction.textStringId);
                menuItem.SetIcon(customAction.iconDrawableId);
                menuItem.SetShowAsAction(ShowAsAction.Always);
                menuItem.SetOnMenuItemClickListener(this);
            }

            if (fragment is IHasToolbarTitle hasToolbarTitle)
            {
                string title = hasToolbarTitle.GetTitle();
                SupportActionBar.Title = title;
            }
        }


        public void GoBack()
        {
            OnBackPressed();
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            (_currentFragment as IHasToolbarAction).GetAction().callback();
            return true;
        }

        public void ShowMyProfilePage(string accessKey, int userId)
        {
            SupportFragmentManager.PopBackStack();
            SupportFragmentManager.ShowFragment(Resource.Id.viewPager, MyprofileFragment.NewInstance(accessKey, userId), false);

        }

        public void ShowMessenger(int userId)
        {
            throw new NotImplementedException();
        }

        public void ShowProfilePage(int userId)
        {
            //throw new NotImplementedException();
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
            SupportFragmentManager.PopBackStack();
            SupportFragmentManager.ShowFragment(Resource.Id.viewPager, new LoginFragment(), false);
        }

        public void ShowRegisterPage()
        {
            SupportFragmentManager.ShowFragment(Resource.Id.viewPager, new RegisterFragment());
        }
    }
}

