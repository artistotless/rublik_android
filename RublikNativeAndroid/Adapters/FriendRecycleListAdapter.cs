using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using static Android.Views.View;

namespace RublikNativeAndroid.Adapters
{
    public class FriendViewHolder : RecyclerView.ViewHolder
    {
        public ImageView image { get; set; }

        public FriendViewHolder(View view) : base(view)
        {
            image = view.FindImageView(Resource.Id.friend_image);
        }
    }

    public class FriendRecycleListAdapter : BaseRecycleViewAdapter<Friend>
    {

        public FriendRecycleListAdapter(IOnClickListener listener) : base(listener) { }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var friendHolder = (FriendViewHolder)holder;
            var image = friendHolder.image;
            var friend = GetElements()[position];
            friendHolder.ItemView.Tag = friend.userId;
            try
            {
                ImageService.Instance
                .LoadUrl(string.Format(Constants.WebApiUrls.FS_AVATAR, friend.avatarUrl))
                .FadeAnimation(true)
                .Transform(new RoundedTransformation(50))
                .Into(image);
            }
            catch { }

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            var item = inflater.Inflate(Resource.Layout.friend_circle_item, parent, false);
            item.SetOnClickListener(_listener);
            return new FriendViewHolder(item);
        }

    }
}
