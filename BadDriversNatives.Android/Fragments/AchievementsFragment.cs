﻿using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace BadDriversNatives.Android.Fragments
{
    public class AchievementsFragment : Fragment
    {
        public AchievementsFragment()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.achievements_layout, container, true);
        }
    }
}