using System.Threading.Tasks;
using MobileApps.Interfaces;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApps
{
    public partial class App : Application
    {
        public static IUser CurrentUser;
        public static string IpAddress = "https://94.228.124.165:5001";

        public App()
        {
            InitializeComponent();

            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            MainPage = new AppShell();
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            App.Current.MainPage.DisplayToastAsync(e.Exception.Message);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            Preferences.Set("username", CurrentUser?.Username);
            Preferences.Set("password", CurrentUser?.Password);
        }

        protected override void OnResume()
        {
            
        }
    }
}
