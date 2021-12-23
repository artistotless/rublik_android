using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Adapters;
using AndroidX.RecyclerView.Widget;
using Android.Widget;
using static Android.Views.View;
using System.Collections.Generic;
using RublikNativeAndroid.Models;
using CrossPlatformLiveData;
using LiteNetLib;
using System;
using RublikNativeAndroid.ViewModels;
using RublikNativeAndroid.Services;
using Google.Android.Material.BottomSheet;
using static Android.Widget.CompoundButton;
using System.Threading.Tasks;
using RublikNativeAndroid.UI.Behaviors;
using AndroidX.CoordinatorLayout.Widget;

namespace RublikNativeAndroid.Fragments
{
    public class RoomsFragment : Fragment, IHasToolbarTitle, IOnClickListener, IRoomEventListener, IHasCustomToolbarMenu, IDisposable
    {


        /* Bottom sheet Views for creating room */
        private BottomSheetDialog _bottomCreateRoomDialog;
        private View _bottomCreateRoomView;
        private EditText _idGameEditText;
        private EditText _passwordEditText;
        private TextView _isPrivateText;
        private CheckBox _isPrivateCheckbox;
        private EditText _awardEditText;
        private Button _submitBtn;
        ///

        /* Bottom sheet Views for showing room's info */
        private BottomSheetDialog _bottomRoomInfoDialog;
        private View _bottomRoomInfoView;
        private TextView _gameTitle;
        private TextView _hostName;
        private TextView _award;
        private TextView _playersCount;
        private Button _joinOrLeaveBtn;
        ///

        private ViewEventListener _viewEventListener;
        private RecyclerView _roomsRecycler;
        private RoomsRecycleViewAdapter _adapter;
        private RoomEventsParserViewModel _eventParser;
        private RoomNetRequestViewModel _roomRequest;

        private Room _selectedRoom;
        private IDisposable _eventsUnsubscriber;

        public string GetTitle() => GetString(Resource.String.lobby);

        public override void OnDestroy()
        {
            base.OnDestroy();
            User.myUser.currentRoom = null;
            _eventsUnsubscriber.Dispose();
            _viewEventListener.Dispose();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            _viewEventListener = new ViewEventListener();
            _adapter = new RoomsRecycleViewAdapter(this);
            _eventParser = new RoomEventsParserViewModel(this);
            _roomRequest = new RoomNetRequestViewModel(_adapter.GetElements());
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_rooms, container, false);

            _roomsRecycler = rootView.FindRecyclerView(Resource.Id.rooms_recycler_view);

            /*
             PushUpBehavior pushUpBehavior = new PushUpBehavior();
            CoordinatorLayout.LayoutParams @params =
                (CoordinatorLayout.LayoutParams)_roomsRecycler.LayoutParameters;
            @params.Behavior = pushUpBehavior;
            */
            AttachAdapter(_adapter, container);
            AttachBottomSheetViews();
            ConfigureButtonSheetCallbacks();

            return rootView;
        }

        private void AttachAdapter(RoomsRecycleViewAdapter adapter, ViewGroup container)
        {
            _roomsRecycler.SetLayoutManager(new LinearLayoutManager(container.Context, (int)Orientation.Vertical, false));
            _roomsRecycler.SetAdapter(adapter);
            _roomsRecycler.Enabled = false;
            _roomsRecycler.TouchscreenBlocksFocus = true;
            _roomsRecycler.Focusable = false;
            
        }

        private void AttachBottomSheetViews()
        {
            _bottomCreateRoomDialog = new BottomSheetDialog(Context);
            _bottomRoomInfoDialog = new BottomSheetDialog(Context);

            _bottomCreateRoomView = LayoutInflater.From(Context).Inflate(Resource.Layout.bottom_sheet_create_room,
                new LinearLayout(Context));

            _bottomRoomInfoView = LayoutInflater.From(Context).Inflate(Resource.Layout.bottom_sheet_room_info,
                new LinearLayout(Context));

            _bottomCreateRoomDialog.SetContentView(_bottomCreateRoomView);
            _bottomRoomInfoDialog.SetContentView(_bottomRoomInfoView);

            _idGameEditText = _bottomCreateRoomView.FindEditText(Resource.Id.room_create_idGame);
            _passwordEditText = _bottomCreateRoomView.FindEditText(Resource.Id.room_create_password);
            _isPrivateText = _bottomCreateRoomView.FindTextView(Resource.Id.room_create_isPrivateText);
            _isPrivateCheckbox = _bottomCreateRoomView.FindCheckBox(Resource.Id.room_create_isPrivate);
            _awardEditText = _bottomCreateRoomView.FindEditText(Resource.Id.room_create_award);
            _submitBtn = _bottomCreateRoomView.FindButton(Resource.Id.room_create_submit);

            _gameTitle = _bottomRoomInfoView.FindTextView(Resource.Id.room_info_game);
            _hostName = _bottomRoomInfoView.FindTextView(Resource.Id.room_info_host);
            _award = _bottomRoomInfoView.FindTextView(Resource.Id.room_info_award);
            _playersCount = _bottomRoomInfoView.FindTextView(Resource.Id.room_info_playersCount);
            _joinOrLeaveBtn = _bottomRoomInfoView.FindButton(Resource.Id.room_info_joinOrLeave);
        }

        private void ConfigureButtonSheetCallbacks()
        {
            _viewEventListener.AddListener(new OnCheckedChange(_isPrivateCheckbox, TogglePasswordEditText));
            _viewEventListener.AddListener(new OnClick(_joinOrLeaveBtn, JoinToSelectedRoom));
            _viewEventListener.AddListener(new OnClick(_submitBtn, HostRoom));
        }

        private void TogglePasswordEditText(object sender, CheckedChangeEventArgs e)
        {
            _passwordEditText.Visibility = e.IsChecked ? ViewStates.Visible : ViewStates.Gone;
            _isPrivateText.Visibility = e.IsChecked ? ViewStates.Gone : ViewStates.Visible;
        }

        private void HostRoom(object sender, EventArgs e)
        {
            _roomRequest.HostRoom(ushort.Parse(_idGameEditText.Text), uint.Parse(_awardEditText.Text), _passwordEditText.Text);
            _bottomCreateRoomDialog.Hide();
        }

        private void JoinToSelectedRoom(object sender, EventArgs e)
        {
            if (User.myUser.inRoom)
                _roomRequest.LeaveRoom();
            else
                _roomRequest.JoinRoom(_selectedRoom.id, _passwordEditText.Text);

            _bottomRoomInfoDialog.Hide();
        }

        private async Task DisplayRoomInfoSheet(int roomId)
        {
            _bottomRoomInfoDialog.Show();
            Room room = _adapter.GetElementById(roomId);
            Game game = await ApiService.GetGameInfoAsync(room.game.id);
            User host = await ApiService.GetUserAsync(room.host.extraData.id);
            _selectedRoom = room;
            _gameTitle.Text = $"Игра: {game.title}";
            _hostName.Text = $"Хост: {host.extraData.nickname}";
            _award.Text = $"Ставка: {room.award} {Constants.Currency.MAIN}";
            _playersCount.Text = $"Игроков в лобби: {room.members.Count}/{game.minPlayers}";
            _joinOrLeaveBtn.Text = _selectedRoom.Equals(Room.myRoom) ? GetString(Resource.String.leave) : GetString(Resource.String.join);
        }

        public async void OnClick(View v) => await DisplayRoomInfoSheet((int)v.Tag);

        public void OnGotRooms(List<Room> rooms) => _adapter.SetElements(rooms);

        public void OnDeletedRoom(int idRoom) => _adapter.DeleteElement(idRoom);

        public void OnHostedRoom(Room room) => _adapter.AddElement(room);

        public void OnLeavedRoom(int idRoom, int userId) => _adapter.RemoveUserFromRoom(idRoom, userId);

        public void OnJoinedRoom(int idRoom, int userId) => _adapter.AddUserToRoom(idRoom, userId);

        public void OnMessagedRoom(Room room)
        {
            throw new NotImplementedException();
        }

        public void OnGameStarted(ServerEndpoint endpoint)
        {
            this.Navigator().ShowGamePage(Room.myRoom, endpoint);
        }

        public void OnSubscribedOnServer(LiveData<NetPacketReader> liveData)
        {
            var liveDataDisposable = liveData.Subscribe(
                (NetPacketReader reader) =>
                {
                    Console.WriteLine($"RoomsFragment : OnSubscribedOnLobbyService THREAD # {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    _eventParser.ParseNetPacketReader(reader);
                },
                delegate (Exception e) { },
                delegate { }
                );

            _eventsUnsubscriber = new UnsubscriberService(liveDataDisposable);
        }

        public ServerEndpoint GetServerEndpoint() => new ServerEndpoint(
            ip: Constants.Services.LOBBY_IP,
            port: Constants.Services.LOBBY_PORT,
            serverType: ServerType.Lobby);

        public CustomToolbarItemsBag GetBag()
        {
            CustomToolbarItemsBag bag = new CustomToolbarItemsBag();
            bag.AddItem(new CustomToolbarItem(Resource.Drawable.baseline_add_20, Resource.String.create, ShowAsAction.Always,
                delegate (object sender, EventArgs e)
                {
                    _bottomCreateRoomDialog.Show();
                }));
            return bag;
        }
    }
}
