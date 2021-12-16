using AndroidX.Lifecycle;
using LiteNetLib;
using LiteNetLib.Utils;

namespace RublikNativeAndroid.Games
{

    enum ControllerAction : ushort
    {
        Move,
        Chat
    }
    public class ShellGameControllerViewModel : ViewModel
    {
        public int currentEggIndex;

        public void Move(ushort idPlace)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ControllerAction.Move);
            writer.Put(idPlace);
            GameServer.currentInstance.Send(writer, DeliveryMethod.ReliableOrdered);
            currentEggIndex = idPlace - 1;
        }


        public void Chat(string msg)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ControllerAction.Chat);
            writer.Put(msg);
            GameServer.currentInstance.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}
