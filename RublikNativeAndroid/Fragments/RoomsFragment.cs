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
using static Android.Widget.CompoundButton;
using System.Threading.Tasks;

namespace RublikNativeAndroid.Fragments
{
    public class RoomsFragment : Fragment, IHasToolbarTitle, IOnClickListener, IRoomEventListener, IHasCustomToolbarMenu, IDisposable
    {
        private BottomCreateRoomSheet _createRoomSheet;
        private BottomRoomInfoSheet _roomInfoSheet;

        private ViewEventListener _viewEventListener;
        private RecyclerView _roomsRecycler;
        private RoomsRecycleViewAdapter _adapter;
        private RoomEventsParserViewModel _eventParser;
        private RoomNetRequestViewModel _roomRequest;

        private Room _selectedRoom;
        private Game _selectedGame;
        private IDisposable _eventsUnsubscriber;

        public string GetTitle() => GetString(Resource.String.lobby);

        public override void OnDestroy()
        {
            base.OnDestroy();
            User.myUser.currentRoom = null;
            _eventsUnsubscriber.Dispose();
            _viewEventListener.Dispose();
        }

        public override void OnDestroyView()
        {
            _viewEventListener.Dispose();
            base.OnDestroyView();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;

            _viewEventListener = new ViewEventListener();
            _createRoomSheet = new BottomCreateRoomSheet(Context);
            _roomInfoSheet = new BottomRoomInfoSheet(Context);
            _adapter = new RoomsRecycleViewAdapter(this);
            _eventParser = new RoomEventsParserViewModel(this);
            _roomRequest = new RoomNetRequestViewModel(_adapter.GetElements());
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_rooms, container, false);
            _roomsRecycler = rootView.FindRecyclerView(Resource.Id.rooms_recycler_view);

            AttachAdapter(_adapter, container);
            ConfigureButtonSheetCallbacks();
            return rootView;
        }

        private void AttachAdapter(RoomsRecycleViewAdapter adapter, ViewGroup container)
        {
            _roomsRecycler.SetLayoutManager(new LinearLayoutManager(container.Context, (int)Orientation.Vertical, false));
            _roomsRecycler.SetAdapter(adapter);
        }

        private void ConfigureButtonSheetCallbacks()
        {
            _viewEventListener.AddListener(new OnCheckedChange(_createRoomSheet.isPrivateCheckbox, TogglePasswordEditText));
            _viewEventListener.AddListener(new OnClick(_roomInfoSheet.joinOrLeaveBtn, JoinToSelectedRoom));
            _viewEventListener.AddListener(new OnClick(_createRoomSheet.submitBtn, HostRoom));
            _viewEventListener.AddListener(new OnClick(_createRoomSheet.gameSelectBtn, SubscribeGamesPageResult));
        }

        private void TogglePasswordEditText(object sender, CheckedChangeEventArgs e)
        {
            _createRoomSheet.passwordEditText.Visibility = e.IsChecked ? ViewStates.Visible : ViewStates.Gone;
            _createRoomSheet.isPrivateText.Visibility = e.IsChecked ? ViewStates.Gone : ViewStates.Visible;
        }

        private void SubscribeGamesPageResult(object sender, EventArgs e)
        {
            _createRoomSheet.Hide();
            this.Navigator().ShowGameSelectPage(OnGetGameFromGameSelectPage);
        }

        private void OnGetGameFromGameSelectPage(Game game)
        {
            _selectedGame = game;
            _createRoomSheet.Show();
            _createRoomSheet.gameSelectBtn.Text = game.title;
        }

        private void HostRoom(object sender, EventArgs e)
        {
            if (_selectedGame == null)
                return;
            _roomRequest.HostRoom(_selectedGame.id, uint.Parse(_createRoomSheet.awardEditText.Text), _createRoomSheet.passwordEditText.Text);
            _createRoomSheet.Hide();
        }

        private void JoinToSelectedRoom(object sender, EventArgs e)
        {
            if (User.myUser.inRoom)
                _roomRequest.LeaveRoom();
            else
                _roomRequest.JoinRoom(_selectedRoom.id, _createRoomSheet.passwordEditText.Text);

            _roomInfoSheet.Hide();
        }

        private async Task DisplayRoomInfoSheet(int roomId)
        {
            _roomInfoSheet.joinOrLeaveBtn.Text = Room.EqualById(roomId, Room.myRoom) ? GetString(Resource.String.leave) : GetString(Resource.String.join);
            _roomInfoSheet.joinOrLeaveBtn.Enabled = false;
            _roomInfoSheet.Show();
            Room room = _adapter.GetElementById(roomId);
            Game game = await ApiService.GetGameInfoAsync(room.game.id);
            User host = await ApiService.GetUserAsync(room.host.extraData.id);
            _selectedRoom = room;
            _roomInfoSheet.gameTitle.Text = $"Игра: {game.title}";
            _roomInfoSheet.hostName.Text = $"Хост: {host.extraData.nickname}";
            _roomInfoSheet.award.Text = $"Ставка: {room.award} {Constants.Currency.MAIN}";
            _roomInfoSheet.playersCount.Text = $"Игроков в лобби: {room.members.Count}/{game.minPlayers}";
            _roomInfoSheet.joinOrLeaveBtn.Enabled = true;
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

        public void ShowCreateRoomSheet(object sender, EventArgs e) => _createRoomSheet.Show();

        public CustomToolbarItemsBag GetBag()
        {
            CustomToolbarItemsBag bag = new CustomToolbarItemsBag();
            bag.AddItem(new CustomToolbarItem(Resource.Drawable.baseline_add_20, Resource.String.create, ShowAsAction.Always,
                ShowCreateRoomSheet));
            return bag;
        }
    }
}
