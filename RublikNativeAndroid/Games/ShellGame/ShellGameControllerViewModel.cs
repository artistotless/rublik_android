using AndroidX.Lifecycle;
using LiteNetLib;
using LiteNetLib.Utils;

namespace RublikNativeAndroid.Games
{

    enum ControllerAction : ushort
    {
        Hide,
        Select,
        Chat
    }
    public class ShellGameControllerViewModel:ViewModel
    {
        private GameInstance _instance;

        public ShellGameControllerViewModel(GameInstance instance)
        {
            _instance = instance;
        }

        public void HideBall(ushort idPlace)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ControllerAction.Hide);
            writer.Put(idPlace);
            _instance.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void SelectBall(ushort idPlace)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((ushort)ControllerAction.Select);
            writer.Put(idPlace);
            _instance.Send(writer, DeliveryMethod.ReliableOrdered);
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
