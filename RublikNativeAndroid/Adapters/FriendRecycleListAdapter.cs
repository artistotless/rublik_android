using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using RublikNativeAndroid.Services;
using static Android.Views.View;

namespace RublikNativeAndroid.Adapters
{
    public class FriendViewHolder : RecyclerView.ViewHolder
    {
        public ImageView image { get; set; }
        public Friend friend { get; set; }

        public FriendViewHolder(View view) : base(view)
        {
            image = view.FindViewById<ImageView>(Resource.Id.friend_image);
        }
    }

    public class FriendRecycleListAdapter : RecyclerView.Adapter, IOnClickListener
    {
        private List<Friend> friends { get; set; }
        private AndroidX.Fragment.App.Fragment _context { get; set; }

        public FriendRecycleListAdapter(AndroidX.Fragment.App.Fragment context)
        {
            _context = context;
            friends = new List<Friend>();
        }

        public void SetFriends(List<Friend> fr)
        {
            friends = fr;
            NotifyDataSetChanged();
        }

        public void AddFriend(Friend fr)
        {
            if (friends.Contains(fr))
                return;

            friends.Add(fr);
            int position = friends.IndexOf(fr);
            NotifyItemInserted(position);
        }


        public override int ItemCount => friends.Count;

        public override long GetItemId(int position) => friends[position].id;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var friendHolder = (FriendViewHolder)holder;
            var image = friendHolder.image;
            var friend = friends[position];
            friendHolder.ItemView.Tag = friend.userId;
            holder.ItemView.ViewAttachedToWindow += async (object sender, ViewAttachedToWindowEventArgs e) =>
            {
                try
                {
                    string avatarUrl = await UsersService.GetAvatarUrl(friends[position].userId);
                    ImageService.Instance
                    .LoadUrl(avatarUrl)
                    .FadeAnimation(true)
                    .Transform(new RoundedTransformation(50))
                    .Into(image);
                }
                catch { }

            };

        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            var item = inflater.Inflate(Resource.Layout.friend_circle_item, parent, false);
            item.SetOnClickListener(this);
            return new FriendViewHolder(item);
        }

        public void OnClick(View v)
        {
            int userId = (int)v.Tag;
            _context.Navigator().ShowProfilePage(userId);
        }

    }
}
