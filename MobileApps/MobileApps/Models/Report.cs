using System;
using System.Collections.ObjectModel;
using MobileApps.Interfaces;

namespace MobileApps.Models
{
    public class Report : IReport
    {
        public DateTime DateReported { get; }
        public ICar BadCar { get; set; }
        public ObservableCollection<string> ImagesPaths { get; }
        public StatusReport Status { get; set; }

        public Report(ICar car, ObservableCollection<string> imagesPaths)
        {
            DateReported = DateTime.Now;
            BadCar = car;
            ImagesPaths = imagesPaths;
            Status = StatusReport.Processing;
        }
    }
}