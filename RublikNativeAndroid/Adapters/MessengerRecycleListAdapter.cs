using Android.Content.Res;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using RublikNativeAndroid.Models;
using RublikNativeAndroid.Services;
using static Android.Views.View;

namespace RublikNativeAndroid.Adapters
{
    public class MessageViewHolder : RecyclerView.ViewHolder
    {
        public TextView message { get; set; }
        public TextView dateTime { get; set; }
        public LinearLayout container { get; set; }
        public LinearLayout parent { get; set; }

        public MessageViewHolder(View view) : base(view)
        {
            message = view.FindTextView(Resource.Id.dialog_message);
            dateTime = view.FindTextView(Resource.Id.dialog_datetime);
            container = view.FindLinearLayout(Resource.Id.dialog_container);
            parent = view.FindLinearLayout(Resource.Id.dialog_parent);
        }
    }

    public class MessengerRecycleListAdapter : BaseRecycleViewAdapter<ChatMessage>
    {
        public MessengerRecycleListAdapter(IOnClickListener listener) : base(listener) { }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var messageHolder = (MessageViewHolder)holder;
            var messageItem = GetElements()[position];

            messageHolder.message.Text = messageItem.text;
            messageHolder.dateTime.Text = messageItem.timeStamp;

            if (messageItem.authorId == ApiService.myUser.extraData.id)
            {
                messageHolder.parent.SetGravity(GravityFlags.End);
                messageHolder.container.BackgroundTintList = ColorStateList.ValueOf(new Android.Graphics.Color(227, 242, 253));
            }
            else
            {
                messageHolder.parent.SetGravity(GravityFlags.Start);
                messageHolder.container.BackgroundTintList = ColorStateList.ValueOf(new Android.Graphics.Color(236, 239, 241));
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            var item = inflater.Inflate(Resource.Layout.messenger_dialog_item, parent, false);
            item.SetOnClickListener(_listener);
            return new MessageViewHolder(item);
        }

    }
}
