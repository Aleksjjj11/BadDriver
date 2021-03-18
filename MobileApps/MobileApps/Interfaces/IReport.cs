using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MobileApps.Interfaces
{
    public enum StatusReport
    {
        Processing,
        Accepted,
        Declined,
    }
    public interface IReport
    {
        DateTime DateReported { get; }
        ICar BadCar { get; set; }
        ObservableCollection<ImageSource> ImagesSources { get; }
        ObservableCollection<string> ImagesPaths { get; }
        StatusReport Status { get; set; }
        string TextStatus { get; }
        Color ColorStatus { get; }
        string Description { get; set; }
        ImageSource ImagePreview { get; }

    }
}