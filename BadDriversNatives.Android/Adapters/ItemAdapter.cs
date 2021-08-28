using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Adapter;
using BadDriversNatives.Android.Fragments;

namespace BadDriversNatives.Android.Adapters
{
    public class ItemAdapter : FragmentStateAdapter
    {
        private readonly Context _context;

        public override int ItemCount => 4;

        public ItemAdapter(FragmentManager fragmentManager, Lifecycle lifecycle) : base(fragmentManager, lifecycle)
        {
        }

        public override Fragment CreateFragment(int position)
        {
            return position switch
            {
                0 => new ReportsFragment(),
                1 => new AchievementsFragment(),
                2 => new CarsFragment(),
                3 => new ProfileFragment(),
                _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
            };
        }
    }
}