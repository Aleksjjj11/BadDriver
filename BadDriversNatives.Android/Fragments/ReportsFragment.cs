using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using BadDriversNatives.Android.Adapters;

namespace BadDriversNatives.Android.Fragments
{
    public class ReportsFragment : Fragment
    {
        private RecyclerView _reportsRecyclerView;

        public ReportsFragment()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.reports_layout, container, true);

            _reportsRecyclerView = view?.FindViewById<RecyclerView>(Resource.Id.reports_list);

            var reports = new List<ReportsListAdapter.Report>
            {
                new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "A348BB",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },
                new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "C739BC",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },
                new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "C296PO",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },
                new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "P820PP",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },
                new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "P334OP",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },
                new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "O328OO",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },
                new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "A235OO",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "A238CE",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "E834EE",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },
                new ReportsListAdapter.Report
                {
                    DateCreated = DateTime.Now.AddDays(-20),
                    BadCar = new ReportsListAdapter.Report.Car
                    {
                        Number = "E943BC",
                        CountryCode = "RUS",
                        RegionCode = 186
                    }
                },
            };

            var adapter = new ReportsListAdapter(reports);

            _reportsRecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            _reportsRecyclerView.HasFixedSize = false;
            _reportsRecyclerView.SetAdapter(adapter);

            return view;
        }
    }
}