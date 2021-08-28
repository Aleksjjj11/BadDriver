using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace BadDriversNatives.Android.Fragments
{
    public class ItemHolder : RecyclerView.ViewHolder
    {
        private static readonly string ITEM_FRAGMENT_TITLE = "fragment_title";

        private RelativeLayout _rl_View;

        public ItemHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ItemHolder(View itemView) : base(itemView)
        {
            _rl_View = itemView.FindViewById<RelativeLayout>(Resource.Id.relativeLayout);
        }

        public void Bind(View view)
        {
            
        }
    }
}