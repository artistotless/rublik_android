using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using FFImageLoading;
using FFImageLoading.Transformations;
using Google.Android.Material.BottomNavigation;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Fragments;
using RublikNativeAndroid.Models;
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
    public class MainActivity : AppCompatActivity, INavigator, IFragmentViewCreateListener, ICacheServiceAccessor, IMenuItemOnMenuItemClickListener, IMessengerInteractor
    {
        public TextView textMessage { get; set; }

        public static MessengerService messengerService { get; private set; }
        public static LobbyService lobbyService { get; private set; }

        private FragmentLifecycleListener _fragmentLifecycleListener { get; set; }
        private BottomNavigationView _bottomNav { get; set; }
        private AndroidX.Fragment.App.Fragment _currentFragment => SupportFragmentManager.FindFragmentById(Resource.Id.viewPager);
        private Dictionary<IMenuItem, EventHandler> _menuItems = new Dictionary<IMenuItem, EventHandler>();
        private static AndroidX.AppCompat.Widget.Toolbar _toolbar { get; set; }
        private static LocalCacheService cacheService { get; set; }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (cacheService == null)
                cacheService = new LocalCacheService();
            _fragmentLifecycleListener = new FragmentLifecycleListener(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            messengerService = messengerService == null ? new MessengerService() : messengerService;
            lobbyService = lobbyService == null ? new LobbyService() : lobbyService;

            _toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.main_toolbar);
            _bottomNav = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);

            if (SupportActionBar == null)
                SetSupportActionBar(_toolbar);
            SupportFragmentManager.RegisterFragmentLifecycleCallbacks(_fragmentLifecycleListener, false);

            if (savedInstanceState == null) // TODO: проверка на сохраненные данные о входе
                ShowLoginPage();
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            DrawToolbarElements(_currentFragment);
            return true;
        }

        private void DrawToolbarElements(AndroidX.Fragment.App.Fragment fragment)
        {
            _toolbar.Menu.Clear();
            _menuItems.Clear();

            if (fragment is IHasCustomToolbarMenu hasToolbarAction)
            {
                CustomToolbarItemsBag menuItemsBag = hasToolbarAction.GetBag();
                foreach (var item in menuItemsBag.GetItems())
                {
                    var menuItem = _toolbar.Menu.Add(item.titleResId);
                    menuItem.SetIcon(item.iconResId);
                    menuItem.SetShowAsAction(item.showAsAction);

                    if (item is CustomToolbarItemImage itemImage)
                    {
                        var viewContainer = LayoutInflater.Inflate(itemImage.layoutResId, _toolbar, false);
                        menuItem.SetActionView(viewContainer);
                        menuItem.SetCheckable(true);
                        var image = viewContainer.FindImageView(itemImage.imageResId);
                        image.Click += itemImage.onClick;
                        ImageService.Instance
                            .LoadUrl(string.Format(Constants.WebApiUrls.FS_AVATAR, itemImage.imagePath))
                            .FadeAnimation(true)
                            .Transform(new CircleTransformation())
                            .Error(delegate
                            {
                                Console.WriteLine("avatar's url is not valid!");
                            })
                            .IntoAsync(image);
                    }

                    _menuItems[menuItem] = item.onClick;
                    menuItem.SetOnMenuItemClickListener(this);
                }
            }
        }

        public void UpdateUI(AndroidX.Fragment.App.Fragment fragment)
        {
            bool needForBackButton = SupportFragmentManager.BackStackEntryCount > 0;

            SupportActionBar.SetDisplayHomeAsUpEnabled(needForBackButton);
            SupportActionBar.SetDisplayShowHomeEnabled(needForBackButton);

            DrawToolbarElements(fragment);

            if (fragment is IHideBottomNav)
                _bottomNav.Visibility = ViewStates.Gone;
            else
                _bottomNav.Visibility = ViewStates.Visible;



            if (fragment is IHasToolbarTitle hasToolbarTitle)
            {
                string title = hasToolbarTitle.GetTitle();
                SupportActionBar.Title = title;

            }


            if (fragment is IMessengerListener messengerListener)
            {
                messengerService.Connect(UsersService.myUser.extraData.accessKey);
                SubscribeOnMessenger(messengerListener);
            }

            if(fragment is IRoomEventListener roomEventListener)
            {
                lobbyService.Connect(UsersService.myUser.extraData.accessKey);
            }
        }

        public void GoBack()
        {
            OnBackPressed();
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            if (_menuItems.ContainsKey(item))
                _menuItems[item](null, null);
            return true;
        }

        public void ShowMyProfilePage()
        {
            SupportFragmentManager.PopBackStack(null, 1);
            SupportFragmentManager.ShowFragment(MyprofileFragment.NewInstance(), false);

        }

        public void ShowMessenger(int userId)
        {
            SupportFragmentManager.ShowFragment(MessengerFragment.NewInstance(userId, "test"));
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
            messengerService.SetListener(listener);
        }

        public LocalCacheService GetCacheService()
        {
            return cacheService;
        }
    }
}

