
namespace RublikNativeAndroid.Contracts
{
    public interface IShellGameEventListener: IGameEventListener
    {
        public void OnUpdateState(uint steps, int masterId, int[]scores);
    }
}
