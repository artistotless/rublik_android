
using System.Threading.Tasks;
using RublikNativeAndroid.Models;

namespace RublikNativeAndroid.Contracts
{
    internal interface IServer
    {
        Task ConnectAsync(ServerEndpoint endpoint);
        void SetListener(IServerListener listener);
    }
}
