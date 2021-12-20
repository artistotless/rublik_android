using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using RublikNativeAndroid.Adapters;
using RublikNativeAndroid.Utils;
using static Android.Views.View;

namespace RublikNativeAndroid
{
    public class MessengerChatView
    {
        private Button _scrollDownBtn, _msgSubmit;
        private EditText _msgField;
        private RecyclerView _chatRecyclerView;
        private MessengerRecycleListAdapter _adapter;
        private Action<string> _submitAction;
        private Context _context;

        public MessengerChatView(EditText msgField, Button msgSubmit, RecyclerView chatRecyclerView, Button scrollDownBtn)
        {
            _scrollDownBtn = scrollDownBtn;
            _msgSubmit = msgSubmit;
            _msgField = msgField;
            _chatRecyclerView = chatRecyclerView;

            _msgField.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                if (string.IsNullOrEmpty(_msgField.Text))
                    _msgSubmit.Visibility = ViewStates.Gone;
                else
                    _msgSubmit.Visibility = ViewStates.Visible;
            };

            _chatRecyclerView.ScrollChange += (object sender, ScrollChangeEventArgs e) =>
            {
                if (_adapter.ItemCount - GetCurrentRecyclerViewPos() > 3)
                    _scrollDownBtn.Visibility = ViewStates.Visible;
                else
                    _scrollDownBtn.Visibility = ViewStates.Gone;
            };

            _scrollDownBtn.Click += (object sender, EventArgs e) => SmoothScroolDown();

            _msgSubmit.Click += (object sender, EventArgs e) =>
           {
               if (_context != null)
                   Vibro.Instance.Peep(_context, 10, 10);
               if (_submitAction != null)
                   _submitAction(_msgField.Text);
               _msgField.SetText(string.Empty, TextView.BufferType.Normal);
           };
        }

        private void SmoothScroolDown()
        {
            _scrollDownBtn.Visibility = ViewStates.Gone;
            _chatRecyclerView.SmoothScrollToPosition(_adapter.ItemCount - 1);
        }

        public MessengerChatView SetAdapter(MessengerRecycleListAdapter adapter, ViewGroup container)
        {
            _adapter = adapter;
            AttachAdapter(_adapter, container);
            return this;
        }

        private void AttachAdapter(MessengerRecycleListAdapter adapter, ViewGroup container)
        {
            LinearLayoutManager layoutManager = new LinearLayoutManager(container.Context, (int)Orientation.Vertical, false);

            layoutManager.StackFromEnd = true;

            _chatRecyclerView.SetLayoutManager(layoutManager);
            _chatRecyclerView.SetAdapter(adapter);
        }

        public MessengerChatView SetSubmitAction(Action<string> action)
        {
            _submitAction = action;
            return this;
        }

        public MessengerChatView VibroEnable(Context context)
        {
            _context = context;
            return this;
        }
        private int GetCurrentRecyclerViewPos() => (_chatRecyclerView.GetLayoutManager() as LinearLayoutManager).FindLastVisibleItemPosition();


    }
}
