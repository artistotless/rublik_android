using System.Collections.Generic;
using System.Linq;
using AndroidX.Lifecycle;
using LiteNetLib;
using LiteNetLib.Utils;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.ViewModels
{
    public class RoomNetRequestViewModel : ViewModel
    {
        private List<Room> _rooms;

        public RoomNetRequestViewModel(List<Room> rooms)
        {
            _rooms = rooms;
        }

        internal void GetRooms()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ActionRequest.GetRooms);
            LobbyService.currentInstance.Send(writer, DeliveryMethod.Unreliable);
        }

        internal void HostRoom(ushort gameId, uint award = 10, string password = "")
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ActionRequest.HostRoom);
            writer.Put(gameId);
            writer.Put(award);
            if (password != string.Empty)
                writer.Put(password);
            LobbyService.currentInstance.Send(writer, DeliveryMethod.ReliableUnordered);
        }

        internal void RequestStartGame()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ActionRequest.StartGame);
            LobbyService.currentInstance.Send(writer, DeliveryMethod.ReliableUnordered);
        }

        internal void JoinRoom(int idRoom, string password = "")
        {
            Room room = _rooms.Where(x => x.id == idRoom).SingleOrDefault();
            if (room != null)
            {
                NetDataWriter writer = new NetDataWriter();
                writer.Put((ushort)ActionRequest.JoinRoom);
                writer.Put(room.id);
                if (room.hasPassword)
                    writer.Put(password);
                LobbyService.currentInstance.Send(writer, DeliveryMethod.ReliableUnordered);

            }
        }

        internal void LeaveRoom()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ActionRequest.LeaveRoom);
            LobbyService.currentInstance.Send(writer, DeliveryMethod.ReliableUnordered);
        }

        internal void MessageToRoom(string text)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ActionRequest.MessageToRoom);
            writer.Put(text);
            LobbyService.currentInstance.Send(writer, DeliveryMethod.ReliableUnordered);
        }
    }
}
