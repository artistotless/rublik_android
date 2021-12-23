using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using RublikNativeAndroid.Models;
using static Android.Views.View;

namespace RublikNativeAndroid.Adapters
{
    public class GameViewHolder : RecyclerView.ViewHolder
    {
        public ImageView image { get; set; }
        public TextView title { get; set; }
        public TextView description { get; set; }
        public TextView minPlayers { get; set; }
        public TextView maxPlayers { get; set; }
        public TextView minBid { get; set; }
        public View divider { get; set; }

        public GameViewHolder(View view) : base(view)
        {
            image = view.FindImageView(Resource.Id.games_game_item_image);
            title = view.FindTextView(Resource.Id.games_gameTitle);
            description = view.FindTextView(Resource.Id.games_gameDescr);
            divider = view.FindViewById<View>(Resource.Id.games_divider);
        }
    }

    public class GamesRecycleViewAdapter : BaseRecycleViewAdapter<Game>
    {
        public GamesRecycleViewAdapter(IOnClickListener listener) : base(listener) { }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Game game = GetElements()[position];
            GameViewHolder gameViewholder = holder as GameViewHolder;

            gameViewholder.ItemView.Tag = (int)game.id;
            gameViewholder.title.Text = game.title;
            gameViewholder.description.Text = game.description;
            gameViewholder.divider.Visibility = ItemCount == (position + 1) ? ViewStates.Invisible : ViewStates.Visible;
            try
            {
                ImageService.Instance
                .LoadUrl(game.image)
                .FadeAnimation(true)
                .Transform(new RoundedTransformation(50))
                .Into(gameViewholder.image);
            }
            catch { }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            var item = inflater.Inflate(Resource.Layout.holder_game_item, parent, false);
            item.SetOnClickListener(_listener);
            return new GameViewHolder(item);
        }
    }
}
