using Android.Content;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomSheet;

namespace RublikNativeAndroid.UI.Views
{
    public abstract class BaseBottomSheetDialog
    {
        public View content;

        private BottomSheetDialog _bottomSheetDialog;
        private Context _context;


        public BaseBottomSheetDialog(Context context, int layoutResId)
        {
            _context = context;

            _bottomSheetDialog = new BottomSheetDialog(_context);
            content = LayoutInflater.From(context).Inflate(layoutResId, new LinearLayout(context), false);
            AttachViews();
        }

        public abstract void AttachViews();

        private void RemoveAllViewChilds(IViewParent parent)
        {
            if (parent != null)
                (parent as ViewGroup).RemoveAllViews();
        }

        public void Show()
        {
            _bottomSheetDialog = new BottomSheetDialog(_context);
            RemoveAllViewChilds(content.Parent);
            _bottomSheetDialog.SetContentView(content);
            _bottomSheetDialog.Show();
        }

        public void Hide() => _bottomSheetDialog.Hide();
    }
}
