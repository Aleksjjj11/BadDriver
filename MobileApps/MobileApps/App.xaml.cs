using System;
using System.Threading.Tasks;
using MobileApps.Interfaces;
using MobileApps.Models;
using MobileApps.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MobileApps
{
    public partial class App : Application
    {
        public static IUser CurrentUser;

        public App()
        {
            InitializeComponent();

            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            MainPage = new MainPage();
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            App.Current.MainPage.DisplayToastAsync(e.Exception.Message);
        }

        protected override void OnStart()
        {
            if (Preferences.ContainsKey("username") && Preferences.ContainsKey("password"))
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    CurrentUser = new User
                    {
                        Username = Preferences.Get("username", ""),
                        Password = Preferences.Get("password", "")
                    };

                    try
                    {
                        CurrentUser.Update("http://188.225.83.42:7000");
                        CurrentUser.UpdateReports("http://188.225.83.42:7000");
                        CurrentUser.GetAllAchievements("http://188.225.83.42:7000");
                    }
                    catch (Exception ex)
                    {
                        Log.Warning("Error Update User Info", ex.Message);

                        App.Current.MainPage.DisplayToastAsync(ex.Message);
                    }
                }
            }
            else
            {
                CurrentUser = null;

                var authorizationPage = new AuthorizationPage();

                MainPage.Navigation.PushModalAsync(authorizationPage);
            }
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
