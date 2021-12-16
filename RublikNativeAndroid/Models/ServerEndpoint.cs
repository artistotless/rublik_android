
namespace RublikNativeAndroid.Models
{
    public struct ServerEndpoint
    {
        public string ip;
        public int port;

        public ServerEndpoint(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
    }
}
