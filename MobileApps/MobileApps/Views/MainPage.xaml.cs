using MobileApps.ViewModels;
using Xamarin.Forms;

namespace MobileApps
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            BindingContext = new MainViewModel();
            InitializeComponent();
        }
    }
}
