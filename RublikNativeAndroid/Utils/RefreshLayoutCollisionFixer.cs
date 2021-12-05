using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;

namespace RublikNativeAndroid.Utils
{
    public class RefreshLayoutCollisionFixer : RecyclerView.OnScrollListener
    {
        private SwipeRefreshLayout _layout;
        public RefreshLayoutCollisionFixer(SwipeRefreshLayout layout) => _layout = layout;

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            if (_layout.Refreshing)
                return;

            _layout.Enabled = newState == 0;

        }
    }
}
