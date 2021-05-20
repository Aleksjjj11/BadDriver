using System.Collections.ObjectModel;
using MobileApps.Interfaces;
using MobileApps.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class DetailReportInfoViewModel : BaseViewModel
    {
        private readonly IReport _report;
        private readonly Page _ownPage;

        public IReport Report => _report;
        public ObservableCollection<ImageSource> ImageSources => Report?.ImagesSources;

        public DetailReportInfoViewModel(IReport report, Page ownPage)
        {
            _report = report;
            _ownPage = ownPage;

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