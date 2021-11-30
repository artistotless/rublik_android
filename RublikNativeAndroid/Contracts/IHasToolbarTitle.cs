using System;

namespace RublikNativeAndroid.Contracts
{
    public interface IHasToolbarAction
    {
        CustomToolbarAction GetAction();
    }

    public struct CustomToolbarAction
    {
        public int iconDrawableId;
        public int titleStringId;
        public Action action;
    }
}
