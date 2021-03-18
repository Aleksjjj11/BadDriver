using System;
using System.Collections.ObjectModel;
using MobileApps.Interfaces;
using MobileApps.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApps
{
    public partial class App : Application
    {
        public static IUser CurrentUser;
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            if (Preferences.ContainsKey("username") && Preferences.ContainsKey("password"))
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    CurrentUser = new User
                    {
                        Username = Preferences.Get("username", "nick"),
                        Password = Preferences.Get("password", "27062000")
                    };
                    CurrentUser.Update("http://188.225.83.42:7000");
                    CurrentUser.UpdateReports("http://188.225.83.42:7000");
                }
            }
            else
            {
                CurrentUser = new User
                {
                    Username = "nick",
                    Password = "27062000",
                };
                CurrentUser.Update("http://188.225.83.42:7000");
                CurrentUser.UpdateReports("http://188.225.83.42:7000");
            }
        }

        protected override void OnSleep()
        {
            Preferences.Set("username", CurrentUser.Username);
            Preferences.Set("password", CurrentUser.Password);
        }

        protected override void OnResume()
        {
            
        }
    }
}
