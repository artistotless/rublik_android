using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;
using System;

namespace RublikNativeAndroid.Fragments
{
    public class MessengerFragment : Fragment, IHasToolbarTitle, IMessengerListener
    {
        public string GetTitle() => GetString(Resource.String.messenger);

        private IDisposable _messengerUnsubscriber;

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _messengerUnsubscriber.Dispose();
        }

        public override void OnCreate(Bundle savedInstanceState) { base.OnCreate(savedInstanceState); }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_messenger, container, false);

            var msg_userId = rootView.FindEditText(Resource.Id.messenger_userId);
            var msg_field = rootView.FindEditText(Resource.Id.messenger_text_field);
            var msg_submit = rootView.FindButton(Resource.Id.messenger_submit);

            

            msg_submit.Click += (object sender, EventArgs e) => {

                int userId = int.Parse(msg_userId.Text);
                string message = msg_field.Text;

                this.Messenger().SendPrivateMessage(userId, message);
            };

            return rootView;
        }

        public void OnHandleMessage(ChatMessage message)
        {
            //throw new NotImplementedException();
        }

        public void OnSubscribedOnMessenger(IDisposable unsubscriber) => _messengerUnsubscriber = unsubscriber;
    }
}
