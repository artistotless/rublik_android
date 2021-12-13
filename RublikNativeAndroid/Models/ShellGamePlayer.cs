
namespace RublikNativeAndroid.Models
{
    public class ShellGamePlayer : BasePlayer
    {
        public int score { get; set; }

        public ShellGamePlayer(string nickname) : base(nickname) { }
        public ShellGamePlayer(User.Data userData) : base(userData) { }
    }
}
