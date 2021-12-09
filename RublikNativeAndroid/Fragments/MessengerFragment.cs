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
        private RecyclerView _dialogList { get; set; }
        private Button _scrollDownBtn { get; set; }

        private User.Data _conversator { get; set; }
        private User.Data _conversatorCache { get; set; }

        private MessengerRecycleListAdapter _adapter;

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

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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
                _msgField.SetText(string.Empty, TextView.BufferType.Normal);
                _adapter.AddElement(new ChatMessage(_conversator.id, message, UsersService.myUser.extraData.id, DateTime.Now));
                this.Messenger().SendPrivateMessage(_conversator.id, message);
            };

            return rootView;
        }

        private void AttachAdapter(MessengerRecycleListAdapter adapter, ViewGroup container)
        {
            if (_dialogList.GetAdapter() != null)
                return;
            _dialogList.SetLayoutManager(new LinearLayoutManager(container.Context, (int)Orientation.Vertical, false));
            _dialogList.SetAdapter(adapter);
        }

        public void OnSubscribedOnMessenger(LiveData<ChatMessage> liveData)
        {
            liveData.Subscribe(
                async (ChatMessage message) =>
                {
                    if (message.authorId == _conversator.id)
                        _adapter.AddElement(message);
                    else
                        Console.WriteLine($"MessengerFragment:OnSubscribedOnMessenger THREAD # {Thread.CurrentThread.ManagedThreadId}");
                },
                (Exception e) => { },
                () => { });
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
