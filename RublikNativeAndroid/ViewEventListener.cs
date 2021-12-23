using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using static Android.Views.View;
using static Android.Widget.CompoundButton;

namespace RublikNativeAndroid
{
    public abstract class BaseViewEvent : IDisposable
    {
        public abstract void Dispose();
    }

    public class OnClick : BaseViewEvent
    {
        public View view;
        public EventHandler handler;
        public OnClick(View clickable, EventHandler handler)
        {
            this.view = clickable;
            this.handler = handler;
            this.view.Click += handler;
        }

        public override void Dispose()
        {
            view.Click -= handler;
            handler = null;
            view = null;
        }
    }

    public class OnViewAttached : BaseViewEvent
    {
        public View view;
        public EventHandler<ViewAttachedToWindowEventArgs> handler;
        public OnViewAttached(View attachable, EventHandler<ViewAttachedToWindowEventArgs> handler)
        {
            this.view = attachable;
            this.handler = handler;
            this.view.ViewAttachedToWindow += handler;
        }

        public override void Dispose()
        {
            view.ViewAttachedToWindow -= handler;
            handler = null;
            view = null;
        }
    }

    public class OnCheckedChange : BaseViewEvent
    {
        public CompoundButton view;
        public EventHandler<CheckedChangeEventArgs> handler;
        public OnCheckedChange(CompoundButton checkable, EventHandler<CheckedChangeEventArgs> handler)
        {
            this.view = checkable;
            this.handler = handler;
            this.view.CheckedChange += handler;
        }

        public override void Dispose()
        {
            view.CheckedChange -= handler;
            handler = null;
            view = null;
        }
    }

    public class ViewEventListener : IDisposable
    {
        private List<BaseViewEvent> _listeners;

        public ViewEventListener() => _listeners = new List<BaseViewEvent>();

        public void AddListener(BaseViewEvent eventContainer)
        {
            if (!_listeners.Contains(eventContainer))
                _listeners.Add(eventContainer);
        }

        public void Dispose()
        {
            foreach (BaseViewEvent listener in _listeners) listener.Dispose();
            _listeners = null;
        }
    }
}
