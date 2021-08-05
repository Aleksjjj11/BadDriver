using MobileApps.Interfaces;
using MobileApps.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApps.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailReportInfoPage : ContentPage
    {
        public DetailReportInfoPage(IReport report)
        {
            BindingContext = new DetailReportInfoViewModel(this, report);
            InitializeComponent();
        }
    }
}