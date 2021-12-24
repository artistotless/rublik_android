using Android.Content;
using Android.Widget;
using RublikNativeAndroid.UI.Views;

namespace RublikNativeAndroid
{
    public class BottomCreateRoomSheet : BaseBottomSheetDialog
    {
        public Button gameSelectBtn { get; private set; }
        public EditText passwordEditText { get; private set; }
        public TextView isPrivateText { get; private set; }
        public CheckBox isPrivateCheckbox { get; private set; }
        public EditText awardEditText { get; private set; }
        public Button submitBtn { get; private set; }

        public BottomCreateRoomSheet(Context context) : base(context, Resource.Layout.bottom_sheet_create_room) { }

        public override void AttachViews()
        {
            gameSelectBtn = content.FindButton(Resource.Id.room_create_idGame);
            passwordEditText = content.FindEditText(Resource.Id.room_create_password);
            isPrivateText = content.FindTextView(Resource.Id.room_create_isPrivateText);
            isPrivateCheckbox = content.FindCheckBox(Resource.Id.room_create_isPrivate);
            awardEditText = content.FindEditText(Resource.Id.room_create_award);
            submitBtn = content.FindButton(Resource.Id.room_create_submit);
        }

    }
}
