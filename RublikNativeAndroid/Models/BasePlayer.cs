namespace RublikNativeAndroid.Models
{
    public class BasePlayer : User
    {
        public int teamId;

        public BasePlayer(string username) : base(username) { }
        public BasePlayer(Data userData) : base(userData) { }
        public override string ToString()
        {
            return
                $"id: {extraData.id}\n" +
                $"username: {extraData.username}\n" +
                $"teamId: {teamId}";
        }
    }
}
