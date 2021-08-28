using Android.App;
using Android.OS;
using AndroidX.ViewPager2.Widget;
using BadDriversNatives.Android.Adapters;
using Google.Android.Material.Tabs;
using System;
using Android.Graphics;
using AndroidX.Fragment.App;
using Google.Android.Material.BottomNavigation;

namespace BadDriversNatives.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : FragmentActivity
    {
        private BottomNavigationView _bottomNavigationView;
        private TabLayout _tabLayout;
        private ViewPager2 _viewPager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            
            _viewPager = FindViewById<ViewPager2>(Resource.Id.viewPager);
            _bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottomNavigationView);
            _tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout1);

            InitViewPager();
            InitTabLayout();
        }

        private void InitTabLayout()
        {
            _bottomNavigationView.ItemSelected += (sender, args) =>
            {
                _viewPager.CurrentItem = args.Item.ItemId switch
                {
                    Resource.Id.menu_journal_item => 0,
                    Resource.Id.menu_achievements_item => 1,
                    Resource.Id.menu_garage_item => 2,
                    Resource.Id.menu_profile_item => 3,
                    _ => throw new ArgumentOutOfRangeException()
                };
            };
        }

        private void InitViewPager()
        {
            _viewPager.Adapter = new ItemAdapter(SupportFragmentManager, Lifecycle);
            _viewPager.RegisterOnPageChangeCallback(new PageChangeCallback(this));
        }

        private class PageChangeCallback : ViewPager2.OnPageChangeCallback
        {
            private readonly BottomNavigationView _bottomNavigationView;
            private readonly TabLayout _tabLayout;

            public PageChangeCallback(Activity activity)
            {
                _bottomNavigationView = activity.FindViewById<BottomNavigationView>(Resource.Id.bottomNavigationView);
                _tabLayout = activity.FindViewById<TabLayout>(Resource.Id.tabLayout1);
            }

            public override void OnPageSelected(int position)
            {
                _bottomNavigationView.SelectedItemId = position switch
                {
                    0 => Resource.Id.menu_journal_item,
                    1 => Resource.Id.menu_achievements_item,
                    2 => Resource.Id.menu_garage_item,
                    3 => Resource.Id.menu_profile_item,
                    _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
                };
            }

            public override void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
                _tabLayout.SetScrollPosition(position, positionOffset, true);
            }
        }
    }
}