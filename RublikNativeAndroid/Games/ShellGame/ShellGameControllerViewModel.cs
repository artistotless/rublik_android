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
    public class ShellGameControllerViewModel:ViewModel
    {
        private GameServer _instance;
        public int currentEggIndex;

        public ShellGameControllerViewModel(GameServer instance)
        {
            _instance = instance;
        }


        public void Move(ushort idPlace)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ControllerAction.Move);
            writer.Put(idPlace);
            _instance.Send(writer, DeliveryMethod.ReliableOrdered);
            currentEggIndex = idPlace - 1;
        }


        public void Chat(string msg)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ControllerAction.Chat);
            writer.Put(msg);
            _instance.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}
