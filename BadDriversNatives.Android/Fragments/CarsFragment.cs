using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace BadDriversNatives.Android.Fragments
{
    public class CarsFragment : Fragment
    {
        public CarsFragment()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return inflater.Inflate(Resource.Layout.cars_layout, container, true);
        }
    }
}