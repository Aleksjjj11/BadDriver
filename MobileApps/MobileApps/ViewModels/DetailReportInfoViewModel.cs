using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using MobileApps.Interfaces;
using MobileApps.Models;
using MobileApps.Popups;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class DetailReportInfoViewModel : BaseViewModel
    {
        private readonly IReport _report;
        private readonly Page _ownPage;
        private ObservableCollection<ImageSource> _imageSources;

        public IReport Report => _report;
        public ObservableCollection<ImageSource> ImageSources => Report?.ImagesSources;

        public DetailReportInfoViewModel(IReport report, Page ownPage)
        {
            _report = report;
            _ownPage = ownPage;
        }

        public ImageSource CountryFlag
        {
            get
            {
                return Report.BadCar.Country switch
                {
                    "RUS" => (ImageSource)(new ImageSourceConverter().ConvertFromInvariantString("russia.png")),
                    "UA" => (ImageSource)(new ImageSourceConverter().ConvertFromInvariantString("ukraine.png")),
                    _ => (ImageSource)(new ImageSourceConverter().ConvertFromInvariantString("NonameFlag.png"))
                };
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public Command OpenImageFullScreenCommand => new Command<ImageSource>(image =>
        {
            _ownPage.Navigation.ShowPopup(new ImageFullScreenPopup(image, _ownPage));
        });
    }
}