using MobileApps.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AchievementsPage), typeof(AchievementsPage));
            Routing.RegisterRoute(nameof(AuthorizationPage), typeof(AuthorizationPage));
            Routing.RegisterRoute(nameof(DetailReportInfoPage), typeof(DetailReportInfoPage));
            Routing.RegisterRoute(nameof(NewReportPage), typeof(NewReportPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(ReportsPage), typeof(ReportsPage));
        }
    }
}