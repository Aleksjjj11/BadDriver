using System;
using System.Collections.ObjectModel;
using Android.Widget;
using MobileApps.Interfaces;
using MobileApps.Models;
using MobileApps.Views;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
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
                        Username = Preferences.Get("username", "Annonim"),
                        Password = Preferences.Get("password", "123")
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
                        Toast.MakeText(Android.App.Application.Context, ex.Message, ToastLength.Long);
                    }
                }
            }
            else
            {
                CurrentUser = null;
                var page = new AuthorizationPage();
                MainPage.Navigation.PushModalAsync(page);
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
