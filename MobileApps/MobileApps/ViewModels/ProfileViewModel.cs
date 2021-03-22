using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MobileApps.Interfaces;
using MobileApps.Models;
using MobileApps.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MobileApps.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public IUser User => App.CurrentUser;
        public bool IsBusy { get; set; }
        private BackgroundWorker _bwUpdater;
        private Page _ownPage;
        public ProfileViewModel(Page page)
        {
            _ownPage = page;
            _bwUpdater = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            _bwUpdater.DoWork += BwUpdaterOnDoWork;
            _bwUpdater.RunWorkerCompleted += BwUpdaterOnRunWorkerCompleted;
            _ownPage.Appearing += (sender, args) =>
            {
                OnPropertyChanged(nameof(User));
            };
            IsBusy = true;
            _bwUpdater.RunWorkerAsync();
        }

        private void BwUpdaterOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnPropertyChanged(nameof(User));
            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
        }

        private void BwUpdaterOnDoWork(object sender, DoWorkEventArgs e)
        {
            User.Update("http://188.225.83.42:7000");
            User.UpdateReports("http://188.225.83.42:7000");
            Log.Warning("INFO", "SCROLL!!!!!!!!!!!!!!!!!!");
        }

        public ICommand UpdateUserCommand => new Command(() =>
        {
            _bwUpdater.RunWorkerAsync();
        });

        public ICommand OpenSettings => new Command(() =>
        {
            _ownPage.Navigation.PushModalAsync(new AuthorizationPage());
        });

        public ICommand LogoutCommand => new Command(() =>
        {
            App.CurrentUser = null;
            
            if (Preferences.ContainsKey("username"))
                Preferences.Clear("username");
            
            if (Preferences.ContainsKey("password"))
                Preferences.Clear("password");
            
            App.Current.MainPage.Navigation.PushModalAsync(new AuthorizationPage());
        });
    }
}