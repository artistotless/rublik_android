using System.Linq;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;
using static Android.Views.View;

namespace RublikNativeAndroid.Adapters
{
    public class RoomViewHolder : RecyclerView.ViewHolder
    {
        public ImageView hasPaswordIco { get; set; }
        public TextView countPlayers { get; set; }
        public ImageView gameImage { get; set; }
        public TextView award { get; set; }
        public TextView host { get; set; }

        public View view;


        public RoomViewHolder(View view) : base(view)
        {
            this.view = view;
            hasPaswordIco = view.FindImageView(Resource.Id.room_isPrivate);
            award = view.FindTextView(Resource.Id.room_award);
            gameImage = view.FindImageView(Resource.Id.room_game_image);
            host = view.FindTextView(Resource.Id.room_host);
            countPlayers = view.FindTextView(Resource.Id.room_players_count);
        }

    }

    public class RoomsRecycleViewAdapter : BaseRecycleViewAdapter<Room>
    {

        public RoomsRecycleViewAdapter(IOnClickListener listener) : base(listener) { }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var roomViewHolder = (RoomViewHolder)holder;
            Room room = GetElements()[position];
            try
            {
                Game game = await ApiService.GetGameInfoAsync(room.game.id);
                User host = await ApiService.GetUserAsync(room.host.extraData.id);
     
                roomViewHolder.hasPaswordIco.Visibility = room.hasPassword ? ViewStates.Visible : ViewStates.Invisible;
                roomViewHolder.countPlayers.Text = $"{room.members.Count}/{game.maxPlayers}";
                ImageService.Instance
                   .LoadUrl(game.image)
                   .FadeAnimation(true)
                   .Transform(new RoundedTransformation(5))
                   .Into(roomViewHolder.gameImage);

                roomViewHolder.award.Text = $"{room.award} {Constants.Currency.MAIN}";
                roomViewHolder.host.Text = host.extraData.nickname;
                roomViewHolder.view.Tag = room.id;

            }
            catch (System.Exception e) { System.Console.WriteLine(e); }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            var item = inflater.Inflate(Resource.Layout.holder_room_item, parent, false);
            item.SetOnClickListener(_listener);
            return new RoomViewHolder(item);
        }


        public void ChangeElement(Room newRoom)
        {
            var position = elements.IndexOf(newRoom);
            var room = elements[position];

            room.award = newRoom.award;
            room.game = newRoom.game;
            room.hasPassword = newRoom.hasPassword;
            room.members = newRoom.members;
            NotifyItemChanged(position);
        }

        public void RemoveUserFromRoom(int idRoom, int userId)
        {
            var position = elements.IndexOf(elements.FirstOrDefault(x => x.id == idRoom));
            User member = userId == User.myUser.extraData.id ? User.myUser : new User(userId);
            elements[position].RemoveMember(member);
            NotifyItemChanged(position);
        }

        public void AddUserToRoom(int idRoom, int userId)
        {
            var position = elements.IndexOf(elements.FirstOrDefault(x => x.id == idRoom));
            User member = userId == User.myUser.extraData.id ? User.myUser : new User(userId);
            elements[position].AddMember(member);
            NotifyItemChanged(position);
        }

        public void DeleteElement(int id)
        {
            var roomIndex = elements.IndexOf(elements.FirstOrDefault(x => x.id == id));
            elements.RemoveAt(roomIndex);
            NotifyItemRemoved(roomIndex);
        }


    }
}
