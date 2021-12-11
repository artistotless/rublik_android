using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using RublikNativeAndroid.Models;
using static Android.Views.View;

namespace RublikNativeAndroid.Adapters
{
    public class RoomViewHolder : RecyclerView.ViewHolder
    {
        public ImageView hasPaswordIco { get; set; }
        public TextView countPlayers { get; set; }
        public TextView gameTitle { get; set; }
        public TextView award { get; set; }
        public TextView host { get; set; }
        public TextView id { get; set; }

        public RoomViewHolder(View view) : base(view)
        {
            this.hasPaswordIco = view.FindImageView(Resource.Id.room_isPasswordIco);
            this.award = view.FindTextView(Resource.Id.room_award);
            this.gameTitle = view.FindTextView(Resource.Id.room_gameTitle);
            this.host = view.FindTextView(Resource.Id.room_host);
            this.id = view.FindTextView(Resource.Id.room_id);
            this.countPlayers = view.FindTextView(Resource.Id.room_playersCount);
        }
    }

    public class RoomsRecycleViewAdapter : BaseRecycleViewAdapter<Room>
    {

        public RoomsRecycleViewAdapter(IOnClickListener listener) : base(listener) { }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var roomViewHolder = (RoomViewHolder)holder;
            var room = GetElements()[position];
            try
            {
                roomViewHolder.hasPaswordIco.Visibility = room.hasPassword ? ViewStates.Visible : ViewStates.Gone;
                roomViewHolder.countPlayers.Text = room.members.Count.ToString();
                roomViewHolder.gameTitle.Text = room.game.id.ToString();
                roomViewHolder.award.Text = room.award.ToString();
                roomViewHolder.host.Text = room.host.extraData.username;
                roomViewHolder.id.Text = room.id.ToString();
            }
            catch { }
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

        public void RemoveUserFromRoom(int idRoom, string username)
        {
            var roomIndex = elements.IndexOf(elements.FirstOrDefault(x => x.id == idRoom));
            elements[roomIndex].members.RemoveAll(x => x.extraData.username == username);  
        }

        public void AddUserToRoom(int idRoom, string username)
        {
            var roomIndex = elements.IndexOf(elements.FirstOrDefault(x => x.id == idRoom));
            elements[roomIndex].members.Add(new User(username));
        }

        public void DeleteElement(int id)
        {
            var roomIndex = elements.IndexOf(elements.FirstOrDefault(x => x.id == id));
            elements.RemoveAt(roomIndex);
            NotifyItemRemoved(roomIndex);
        }


    }
}
