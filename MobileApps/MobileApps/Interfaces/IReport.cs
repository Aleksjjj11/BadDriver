using System;
using System.Collections.ObjectModel;

namespace MobileApps.Interfaces
{
    public enum StatusReport
    {
        Accepted,
        Declined,
        Processing
    }
    public interface IReport
    {
        DateTime DateReported { get; }
        ICar BadCar { get; set; }
        ObservableCollection<string> ImagesPaths { get; }
        StatusReport Status { get; set; }

    }
}