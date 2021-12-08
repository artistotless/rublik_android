using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading;
using CrossPlatformLiveData;
using AndroidX.Fragment.App;
using RublikNativeAndroid.Contracts;
using RublikNativeAndroid.Models;
using System.Collections.Generic;

namespace RublikNativeAndroid.Fragments
{
    public class MessengerFragment : Fragment, IHasToolbarTitle, IMessengerListener, IHasCustomToolbarMenu
    {
        public CustomToolbarItemsBag GetBag()
        {
            var bag = new CustomToolbarItemsBag();
            bag.AddItems(new List<CustomToolbarItem>(){

                new CustomToolbarItemImage(Resource.Layout.messenger_avatar,Resource.Id.friend_image,"/Files/33kb.jpg", Resource.Drawable.mail_ico, Resource.String.profile, ShowAsAction.IfRoom,
                delegate { Console.WriteLine($"Callback method from {Class.SimpleName}"); }),

                new CustomToolbarItem(Resource.Drawable.mail_ico, Resource.String.messages, ShowAsAction.IfRoom,
                delegate { Console.WriteLine($"Callback method from {Class.SimpleName}"); }),

                new CustomToolbarItem(Resource.Drawable.games_ico, Resource.String.games, ShowAsAction.IfRoom,
                delegate { Console.WriteLine($"Callback method from {Class.SimpleName}"); }),

                new CustomToolbarItem(Resource.Drawable.graph_ico, Resource.String.stats, ShowAsAction.Never,
                delegate { Console.WriteLine($"Callback method from {Class.SimpleName}"); }),

                new CustomToolbarItem(Resource.Drawable.settings_ico, Resource.String.settings, ShowAsAction.WithText,
                delegate { Console.WriteLine($"Callback method from {Class.SimpleName}"); }),

            });
            return bag;
        }

        public string GetTitle() => GetString(Resource.String.messenger);

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.fragment_messenger, container, false);

            var msg_userId = rootView.FindEditText(Resource.Id.messenger_userId);
            var msg_field = rootView.FindEditText(Resource.Id.messenger_text_field);
            var msg_submit = rootView.FindButton(Resource.Id.messenger_submit);



            msg_submit.Click += (object sender, EventArgs e) =>
            {

                int userId = int.Parse(msg_userId.Text);
                string message = msg_field.Text;

                this.Messenger().SendPrivateMessage(userId, message);
            };

            return rootView;
        }


        public void OnSubscribedOnMessenger(LiveData<ChatMessage> liveData)
        {
            liveData.Subscribe(
                (ChatMessage message) =>
                {
                    Console.WriteLine($"MessengerFragment:OnSubscribedOnMessenger THREAD # {Thread.CurrentThread.ManagedThreadId}");

                    Toast.MakeText(Context, $"Message: {message.text}", ToastLength.Short).Show();
                },
                (Exception e) => { },
                () => { });
        }
    }
}
