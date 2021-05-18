using MobileApps.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApps.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizationPage : ContentPage
    {
        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        public void BackPressed()
        {
            base.OnBackButtonPressed();
        }

        public AuthorizationPage()
        {
            BindingContext = new AuthorizationViewModel(this);
            InitializeComponent();
        }
    }
}