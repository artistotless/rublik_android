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

namespace RublikNativeAndroid.Fragments
{
    public class RoomsFragment : Fragment, IHasToolbarTitle, IOnClickListener, IRoomEventListener
    {
        private RecyclerView _rooms_scroll;
        private RoomsRecycleViewAdapter _adapter;
        private RoomEventsParserViewModel _roomEvents;
        private RoomNetRequestViewModel _roomRequest;

        public string GetTitle() => GetString(Resource.String.lobby);


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _adapter = new RoomsRecycleViewAdapter(this);
            _roomEvents = new RoomEventsParserViewModel(this);
            _roomRequest = new RoomNetRequestViewModel(Services.LobbyService.currentInstance,_adapter.GetElements());
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_rooms, container, false);

            _rooms_scroll = rootView.FindRecyclerView(Resource.Id.rooms_recycler_view);
            
            AttachAdapter(_adapter, container);

            return rootView;
        }

        private void AttachAdapter(RoomsRecycleViewAdapter adapter, ViewGroup container)
        {
            _rooms_scroll.SetLayoutManager(new LinearLayoutManager(container.Context, (int)Orientation.Vertical, false));
            _rooms_scroll.SetAdapter(adapter);
        }

        public void OnClick(View v)
        {
            _roomRequest.JoinRoom(0, string.Empty);
        }

        public void OnGotRooms(List<Room> rooms) => _adapter.SetElements(rooms);

        public void OnDeletedRoom(int idRoom) => _adapter.DeleteElement(idRoom);

        public void OnHostedRoom(Room room) => _adapter.AddElement(room);

        public void OnLeavedRoom(int idRoom, string userName) => _adapter.RemoveUserFromRoom(idRoom, userName);

        public void OnJoinedRoom(int idRoom, string userName) => _adapter.AddUserToRoom(idRoom, userName);

        public void OnMessagedRoom(Room room)
        {
            throw new NotImplementedException();
        }

        public void OnGameStarted(string ip, int port)
        {
            ParentFragmentManager.BeginTransaction().Replace(Resource.Id.viewPager, ShellGameFragment.NewInstance(ip,port)).AddToBackStack(null).Commit();
        }

        public void OnSubscribedOnLobbyService(LiveData<NetPacketReader> liveData)
        {
            liveData.Subscribe(
                delegate (NetPacketReader reader)
                {
                    Console.WriteLine($"RoomsFragment : OnSubscribedOnLobbyService THREAD # {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    _roomEvents.ParseNetPacketReader(reader);
                },
                delegate (Exception e) { },
                delegate { }
                );
        }


    }
}
