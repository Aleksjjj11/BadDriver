using MobileApps.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApps.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportsPage : ContentPage
    {
        public ReportsPage()
        {
            BindingContext = new ReportsViewModel(this);
            InitializeComponent();
        }
    }
}