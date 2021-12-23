
namespace RublikNativeAndroid.Models
{
    public class ShellGamePlayer : BasePlayer
    {
        public int score { get; set; }

        public ShellGamePlayer(int playerId) : base(playerId) { }
        public ShellGamePlayer(User.Data userData) : base(userData) { }
    }
}
