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
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid
{

    public class FragmentLifecycleListener : AndroidX.Fragment.App.FragmentManager.FragmentLifecycleCallbacks
    {
        public IFragmentViewCreateListener _listener { get; private set; }
        public FragmentLifecycleListener(IFragmentViewCreateListener listener) { _listener = listener; }
        public override void OnFragmentViewCreated(AndroidX.Fragment.App.FragmentManager fm, AndroidX.Fragment.App.Fragment f, View v, Bundle savedInstanceState)
        {
            base.OnFragmentViewCreated(fm, f, v, savedInstanceState);
            _listener.UpdateUI(f);
        }
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, INavigator, IFragmentViewCreateListener, IMenuItemOnMenuItemClickListener, IMessengerInteractor
    {
        public TextView textMessage { get; set; }
        public INavigator mainNavigator { get; set; }
        public static MessengerService messengerService { get; private set; }

        private FragmentLifecycleListener _fragmentLifecycleListener { get; set; }
        private BottomNavigationView _bottomNav { get; set; }
        private AndroidX.AppCompat.Widget.Toolbar _toolbar { get; set; }
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

            messengerService = messengerService == null ? new MessengerService() : messengerService;

            _toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.main_toolbar);
            _bottomNav = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            SetSupportActionBar(_toolbar);
            SupportFragmentManager.RegisterFragmentLifecycleCallbacks(_fragmentLifecycleListener, false);

        }

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


        public void UpdateUI(AndroidX.Fragment.App.Fragment fragment)
        {
            bool needForBackButton = SupportFragmentManager.BackStackEntryCount > 0;

            SupportActionBar.SetDisplayHomeAsUpEnabled(needForBackButton);
            SupportActionBar.SetDisplayShowHomeEnabled(needForBackButton);

            _toolbar.Menu.Clear();

            if (fragment is IHideBottomNav)
                _bottomNav.Visibility = ViewStates.Gone;
            else
                _bottomNav.Visibility = ViewStates.Visible;

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

            if(fragment is IMessengerListener listener)
            {
                messengerService.Connect(UsersService.myUser.extraData.accessKey);
                SubscribeOnMessenger(listener);
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

        public void ShowMyProfilePage()
        {
            SupportFragmentManager.PopBackStack(null, 1);
            SupportFragmentManager.ShowFragment(MyprofileFragment.NewInstance(), false);

        }

        public void ShowMessenger(int userId)
        {
            SupportFragmentManager.ShowFragment(new MessengerFragment());
        }

        public void ShowProfilePage(int userId)
        {
            if (userId == UsersService.myUser.extraData.id)
            {
                ShowMyProfilePage();
                return;
            }
            SupportFragmentManager.ShowFragment(ProfileFragment.NewInstance(userId));
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
            SupportFragmentManager.ShowFragment(new LoginFragment(), false);
        }

        public void ShowRegisterPage()
        {
            SupportFragmentManager.ShowFragment(new RegisterFragment());
        }

        public void SendPrivateMessage(int destinationUserId, string message)
        {
            messengerService.SendPrivateMessage(destinationUserId, message);
        }

        public void SubscribeOnMessenger(IMessengerListener listener)
        {
            listener.OnSubscribedOnMessenger(messengerService.SetListener(listener));
        }
    }
}

