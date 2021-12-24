using Android.Content;
using Android.Widget;
using RublikNativeAndroid.UI.Views;

namespace RublikNativeAndroid
{
    public class BottomRoomInfoSheet : BaseBottomSheetDialog
    {
        public TextView gameTitle { get; private set; }
        public TextView hostName { get; private set; }
        public TextView award { get; private set; }
        public TextView playersCount { get; private set; }
        public Button joinOrLeaveBtn { get; private set; }

        public BottomRoomInfoSheet(Context context) : base(context, Resource.Layout.bottom_sheet_room_info) { }

        public override void AttachViews()
        {
            gameTitle = content.FindTextView(Resource.Id.room_info_game);
            hostName = content.FindTextView(Resource.Id.room_info_host);
            award = content.FindTextView(Resource.Id.room_info_award);
            playersCount = content.FindTextView(Resource.Id.room_info_playersCount);
            joinOrLeaveBtn = content.FindButton(Resource.Id.room_info_joinOrLeave);
        }

    }
}
