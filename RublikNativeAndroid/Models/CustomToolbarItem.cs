using System;
using System.Collections.Generic;
using Android.Views;

namespace RublikNativeAndroid.Models
{
    public class CustomToolbarItem
    {
        public int iconResId;
        public int titleResId;
        public EventHandler onClick;
        public ShowAsAction showAsAction;

        public CustomToolbarItem(int iconResId, int titleResId, ShowAsAction showAsAction, EventHandler onClick)
        {
            this.iconResId = iconResId;
            this.titleResId = titleResId;
            this.onClick = onClick;
            this.showAsAction = showAsAction;
        }
    }

    public class CustomToolbarItemImage : CustomToolbarItem
    {
        public int layoutResId;
        public int imageResId;
        public string imagePath;

        public CustomToolbarItemImage(int layoutResId, int imageResId, string imagePath, int iconResId, int titleResId, ShowAsAction showAsAction, EventHandler onClick) : base(iconResId, titleResId, showAsAction, onClick)
        {
            this.layoutResId = layoutResId;
            this.imageResId = imageResId;
            this.imagePath = imagePath;
        }
    }

    public class CustomToolbarItemButton : CustomToolbarItem
    {
        public int layoutResId;
        public int buttonResId;

        public CustomToolbarItemButton(int layoutResId, int buttonResId, int iconResId, int titleResId, ShowAsAction showAsAction, EventHandler onClick) : base(iconResId, titleResId, showAsAction, onClick)
        {
            this.layoutResId = layoutResId;
            this.buttonResId = buttonResId;
        }
    }

    public class CustomToolbarItemsBag
    {
        private IList<CustomToolbarItem> items = new List<CustomToolbarItem>();
        public void AddItem(CustomToolbarItem item) => items.Add(item);
        public void AddItems(List<CustomToolbarItem> items) { foreach (var item in items) { AddItem(item); } }
        public IList<CustomToolbarItem> GetItems() => items;
    }
}
