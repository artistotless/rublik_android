using System;
using Android.Content;
using Android.Util;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;

namespace RublikNativeAndroid.UI.Behaviors
{
    public class PushUpBehavior : CoordinatorLayout.Behavior
    {
        public PushUpBehavior() : base() { }
    
        public PushUpBehavior(Context context, IAttributeSet attributeSet) : base(context, attributeSet) { }

        public override bool OnDependentViewChanged(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        {
            Console.WriteLine($"BottomSheet Y ------- {dependency.TranslationY}");
            return true;
        }
    }
}
