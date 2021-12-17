
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.Models
{
    public struct ServerEndpoint
    {
        public string ip;
        public int port;
        public ServerType type;

        public ServerEndpoint(string ip, int port, ServerType serverType)
        {
            this.ip = ip;
            this.port = port;
            this.type = serverType;
        }
    }
}
