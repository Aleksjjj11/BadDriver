using System;
using System.Collections.ObjectModel;
using System.Linq;
using MobileApps.Interfaces;
using MobileApps.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class DetailReportInfoViewModel : BaseViewModel
    {
        private readonly Page _ownPage;

        public IReport Report { get; private set; }

        public ObservableCollection<ImageSource> ImageSources { get; private set; }
        //TODO пофиксить наложение фотографий, когда кликаешь много раз на одну из них
        public DetailReportInfoViewModel(Page ownPage, IReport report)
        {
            _ownPage = ownPage;
            Report = report;

            ImageSources = new ObservableCollection<ImageSource>(report.ImagesPaths.Select(x => ImageSource.FromUri(new Uri(x))));

            InitCommands();
        }

        private void InitCommands()
        {
            OpenImageFullScreenCommand = new Command<ImageSource>(image =>
            {
                _ownPage.Navigation.ShowPopup(new ImageFullScreenPopup(image, _ownPage));
            });
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

        public Command OpenImageFullScreenCommand { get; private set; }
    }
}