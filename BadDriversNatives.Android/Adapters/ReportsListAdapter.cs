using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace BadDriversNatives.Android.Adapters
{
    public class ReportsListAdapter : RecyclerView.Adapter
    {
        private List<Report> _reports;
        private Context _context;

        public ReportsListAdapter(List<Report> reports)
        {
            _reports = reports;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ReportsListHolder reportHolder)
            {
                reportHolder.Bind(_reports[position]);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            _context = parent.Context;

            var reportItemLayoutId = Resource.Layout.report_item_layout;
            var inflater = LayoutInflater.From(_context);
            var viewReport = inflater.Inflate(reportItemLayoutId, parent, false);

            var holder = new ReportsListHolder(viewReport);

            return holder;
        }

        public override int ItemCount => _reports.Count;

        public class ReportsListHolder : RecyclerView.ViewHolder
        {
            private readonly TextView _tvNumberCar;
            private readonly TextView _tvRegionCar;
            private readonly TextView _tvCountryCar;
            private readonly TextView _tvDateReport;

            public ReportsListHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public ReportsListHolder(View itemView) : base(itemView)
            {
                _tvNumberCar = itemView.FindViewById<TextView>(Resource.Id.number_car);
                _tvRegionCar = itemView.FindViewById<TextView>(Resource.Id.region_car);
                _tvCountryCar = itemView.FindViewById<TextView>(Resource.Id.country_car);
                _tvDateReport = itemView.FindViewById<TextView>(Resource.Id.date_report);
            }

            public void Bind(Report report)
            {
                var car = report.BadCar;

                _tvNumberCar.Text = car.Number;
                _tvCountryCar.Text = car.CountryCode;
                _tvRegionCar.Text = car.RegionCode.ToString();
                _tvDateReport.Text = report.DateCreated.ToString("dd.MM.yyyy hh:mm");
            }
        }

        public class Report
        {
            public DateTime DateCreated { get; set; }
            public Car BadCar { get; set; }

            public class Car
            {
                public string Number { get; set; }
                public int RegionCode { get; set; }
                public string CountryCode { get; set; }
            }
        }
    }
}