using MobileApps.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApps.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewReportPage : ContentPage
    {
        public NewReportPage()
        {
            BindingContext = new NewReportViewModel(this);
            InitializeComponent();
        }
    }
}