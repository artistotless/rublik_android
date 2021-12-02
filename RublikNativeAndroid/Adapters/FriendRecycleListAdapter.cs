using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace RublikNativeAndroid.Adapters
{
    public class FriendViewHolder : RecyclerView.ViewHolder
    {
        public ImageView image { get; set; }

        public FriendViewHolder(View view) : base(view)
        {
            image = view.FindViewById<ImageView>(Resource.Id.friend_image);
        }
    }

    public class FriendRecycleListAdapter : RecyclerView.Adapter
    {

        public List<Friend> friends = new List<Friend>();



        public override int ItemCount => friends.Count;

        public override long GetItemId(int position) => friends[position].id;


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var friendHolder = holder as FriendViewHolder;
            var image = friendHolder.image;
            var friend = friends[position];

            image.ContentDescription = friend.id.ToString();
            // ImageService.Instance
            //   .LoadUrl(friend.imageUrl)
            // .FadeAnimation(true)
            //.Transform(new BlurredTransformation())
            //.Into(image);

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            var item = inflater.Inflate(Resource.Layout.friend_circle_item, parent, false);
            return new FriendViewHolder(item);
        }
    }
}
