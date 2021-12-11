using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading;
using CrossPlatformLiveData;
using AndroidX.Fragment.App;
using static Android.Views.View;
using RublikNativeAndroid.Models;
using System.Collections.Generic;
using RublikNativeAndroid.Contracts;
using AndroidX.RecyclerView.Widget;
using RublikNativeAndroid.Adapters;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.Fragments
{
    public class MessengerFragment : Fragment, IHasToolbarTitle, IMessengerListener, IHasCustomToolbarMenu, IHideBottomNav, IOnClickListener
    {
        private EditText _msgField { get; set; }
        private Button _msgSubmit { get; set; }
        private Button _scrollDownBtn { get; set; }
        private RecyclerView _dialogList { get; set; }

        private User.Data _conversator { get; set; }
        private User.Data _conversatorCache { get; set; }

        private MessengerRecycleListAdapter _adapter;
        private IDisposable _unsubscriberMessenger;

        public CustomToolbarItemsBag GetBag()
        {
            var bag = new CustomToolbarItemsBag();
            bag.AddItems(new List<CustomToolbarItem>(){

                new CustomToolbarItemImage(Resource.Layout.messenger_avatar,Resource.Id.friend_image, _conversator.avatar, Resource.Drawable.mail_ico, Resource.String.profile, ShowAsAction.Always,
                delegate { Console.WriteLine($"Callback method from {Class.SimpleName}"); }),

            });
            return bag;
        }

        public string GetTitle() => _conversator.nickname;

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            if (_unsubscriberMessenger != null)
                _unsubscriberMessenger.Dispose();
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            var _conversatorId = Arguments.GetInt(Constants.Fragments.USER_ID);

            _conversatorCache = this.Cache().GetCacheService().GetUsersData(_conversatorId);
            if (_conversatorCache != null)
            {
                _conversator = _conversatorCache;
            }
            else
            {
                _conversator = UsersService.GetUserAsync(_conversatorId, true).Result.extraData;
                this.Cache().GetCacheService().AddUsersData(_conversatorId, _conversator);
            }

            _adapter = new MessengerRecycleListAdapter(this);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_messenger, container, false);

            _msgField = rootView.FindEditText(Resource.Id.messenger_text_field);
            _scrollDownBtn = rootView.FindButton(Resource.Id.messenger_scrolldown_btn);
            _msgSubmit = rootView.FindButton(Resource.Id.messenger_send_btn);
            _dialogList = rootView.FindRecyclerView(Resource.Id.messenger_dialog_list);

            AttachAdapter(_adapter, container);

            _msgField.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                if (string.IsNullOrEmpty(_msgField.Text))
                    _msgSubmit.Visibility = ViewStates.Gone;
                else
                    _msgSubmit.Visibility = ViewStates.Visible;
            };

            _scrollDownBtn.Click += (object sender, EventArgs e) => { SmoothScroolDown(); };

            _dialogList.ScrollChange += (object sender, ScrollChangeEventArgs e) =>
            {
                if (_adapter.ItemCount - GetCurrentRecyclerViewPos() > 3)
                    _scrollDownBtn.Visibility = ViewStates.Visible;
                else
                    _scrollDownBtn.Visibility = ViewStates.Gone;
            };

            _msgSubmit.Click += (object sender, EventArgs e) =>
            {
                Vibrator v = Vibrator.FromContext(Context);

                // Vibrate for 20 milliseconds
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    v.Vibrate(VibrationEffect.CreateOneShot(20, 10));

                else
                    //deprecated in API 26 
                    v.Vibrate(20);

                string message = _msgField.Text;
                this.Messenger().SendPrivateMessage(_conversator.id, message);
                _msgField.SetText(string.Empty, TextView.BufferType.Normal);
                HandleIncommingMessage(new ChatMessage(_conversator.id, message, UsersService.myUser.extraData.id, DateTime.Now));

            };

            return rootView;
        }

        private int GetCurrentRecyclerViewPos() => (_dialogList.GetLayoutManager() as LinearLayoutManager).FindLastVisibleItemPosition();

        private void AttachAdapter(MessengerRecycleListAdapter adapter, ViewGroup container)
        {
            LinearLayoutManager layoutManager = new LinearLayoutManager(container.Context, (int)Orientation.Vertical, false);

            layoutManager.StackFromEnd = true;

            _dialogList.SetLayoutManager(layoutManager);
            _dialogList.SetAdapter(adapter);
        }

        public void OnSubscribedOnMessenger(LiveData<ChatMessage> liveData)
        {
            _unsubscriberMessenger = liveData.Subscribe(
                (ChatMessage message) =>
                {
                    if (message.authorId == _conversator.id)
                        HandleIncommingMessage(message);
                    else
                        Console.WriteLine($"MessengerFragment:OnSubscribedOnMessenger THREAD # {Thread.CurrentThread.ManagedThreadId}");
                },
                (Exception e) => { },
                delegate { });
        }

        private void HandleIncommingMessage(ChatMessage message)
        {
            var currentRecyclerViewPos = GetCurrentRecyclerViewPos();
            _adapter.AddElement(message);

            if (_adapter.ItemCount - currentRecyclerViewPos < 3)
                SmoothScroolDown();

        }

        private void SmoothScroolDown()
        {
            _scrollDownBtn.Visibility = ViewStates.Gone;
            _dialogList.SmoothScrollToPosition(_adapter.ItemCount - 1);
        }

        public void OnClick(View v)
        {
            Console.WriteLine("Нажал на текст");
        }

        public static MessengerFragment NewInstance(int conversatorId, string conversatorName)
        {
            var fragment = new MessengerFragment();
            var bundle = new Bundle();
            bundle.PutInt(Constants.Fragments.USER_ID, conversatorId);
            bundle.PutString(Constants.Fragments.USER_NAME, conversatorName);
            fragment.Arguments = bundle;
            return fragment;
        }
    }
}
