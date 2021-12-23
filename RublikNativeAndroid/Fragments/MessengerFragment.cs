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
using LiteNetLib;
using RublikNativeAndroid.ViewModels;
using RublikNativeAndroid.Utils;

namespace RublikNativeAndroid.Fragments
{
    public class MessengerFragment : Fragment, IHasToolbarTitle, IMessengerEventListener, IHasCustomToolbarMenu, IHideBottomNav, IOnClickListener
    {
        private EditText _msgField { get; set; }
        private Button _msgSubmit { get; set; }
        private Button _scrollDownBtn { get; set; }
        private RecyclerView _dialogList { get; set; }

        private User.Data _conversator { get; set; }

        private MessengerRecycleListAdapter _adapter;
        private IDisposable _eventsUnsubscriber;
        private MessengerRequestsViewModel _controller;
        private MessengerEventsParserViewModel _eventParser;

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
            try { _eventsUnsubscriber.Dispose(); }
            catch { }
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            var _conversatorId = Arguments.GetInt(Constants.Fragments.USER_ID);
            _conversator = ApiService.GetUserAsync(_conversatorId, true).Result.extraData;
            _adapter = new MessengerRecycleListAdapter(this);
            _eventParser = new MessengerEventsParserViewModel(this);
            _controller = new MessengerRequestsViewModel();
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
                Vibro.Instance.Peep(Context, 10, 10);
                string message = _msgField.Text;
                _controller.SendPrivateMessage(_conversator.id, message);
                _msgField.SetText(string.Empty, TextView.BufferType.Normal);
                HandleIncommingMessage(new ChatMessage(_conversator.id, message, ApiService.myUser.extraData.id, DateTime.Now));

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

        public static MessengerFragment NewInstance(int conversatorId)
        {
            var fragment = new MessengerFragment();
            var bundle = new Bundle();
            bundle.PutInt(Constants.Fragments.USER_ID, conversatorId);
            fragment.Arguments = bundle;
            return fragment;
        }

        public void OnComingMessage(ChatMessage message)
        {
            if (message.authorId == _conversator.id)
                HandleIncommingMessage(message);
            else
                Console.WriteLine($"MessengerFragment:OnSubscribedOnMessenger THREAD # {Thread.CurrentThread.ManagedThreadId}");
            // TODO: добавить сообщение в базу данных 
        }

        public void OnSubscribedOnServer(LiveData<NetPacketReader> liveData)
        {
            var liveDataDisposable = liveData.Subscribe(
              (NetPacketReader reader) => _eventParser.ParseNetPacketReader(reader),
              delegate (Exception e) { },
              delegate { }
              );

            _eventsUnsubscriber = new UnsubscriberService(liveDataDisposable);
        }


        public ServerEndpoint GetServerEndpoint() => new ServerEndpoint(
            ip: Constants.Services.MESSENGER_IP,
            port: Constants.Services.MESSENGER_PORT,
            serverType: ServerType.Messenger);
    }
}
