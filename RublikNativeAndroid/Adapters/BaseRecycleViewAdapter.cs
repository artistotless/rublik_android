using System.Collections.Generic;
using Android.Views;
using System;
using AndroidX.RecyclerView.Widget;
using static Android.Views.View;
using RublikNativeAndroid.Contracts;

namespace RublikNativeAndroid.Adapters
{

    public class BaseRecycleViewAdapter<T> : RecyclerView.Adapter
    {
        public List<T> elements { get; set; }
        public IOnClickListener _listener { get; set; }

        public BaseRecycleViewAdapter(IOnClickListener listener)
        {
            _listener = listener;
            elements = new List<T>();
        }

        public void SetElements(List<T> elements)
        {
            this.elements = elements;
            NotifyDataSetChanged();
        }

        public List<T> GetElements() => elements;

        public void AddElement(T element)
        {
            if (elements.Contains(element))
                return;

            elements.Add(element);
            int position = elements.IndexOf(element);
            NotifyItemInserted(position);
        }

        public void DeleteElement(T element)
        {
            int position = elements.IndexOf(element);
            elements.Remove(element);
            NotifyItemRemoved(position);
        }


        public override int ItemCount => elements.Count;

        public override long GetItemId(int position)
        {
            if (elements[position] is IHasId element)
            {
                return element.GetId();
            }

            throw new InvalidCastException($"Not implemented IHasId");
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            throw new NotImplementedException("Override OnBindViewHolder method in your class");
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            throw new NotImplementedException("Override OnCreateViewHolder method in your class");
        }

    }
}
