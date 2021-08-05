using System.ComponentModel;
using MobileApps.Interfaces;
using MobileApps.Popups;
using MobileApps.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly BackgroundWorker _bwUpdater;
        private readonly Page _ownPage;

        public IUser User => App.CurrentUser;

        public bool IsBusy { get; set; }

        public ProfileViewModel(Page page)
        {
            _ownPage = page;

            InitCommands();

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

        private void InitCommands()
        {
            OpenSettingsCommand = new Command(() =>
            {
                
            });

            UpdateUserCommand = new Command(() =>
            {
                _bwUpdater.RunWorkerAsync();
            });

            LogoutCommand = new Command(() =>
            {
                App.CurrentUser = null;

                if (Preferences.ContainsKey("username"))
                {
                    Preferences.Clear("username");
                }

                if (Preferences.ContainsKey("password"))
                {
                    Preferences.Clear("password");
                }

                Application.Current.MainPage.Navigation.PushModalAsync(new AuthorizationPage());
            });

            OpenDuckPopupCommand = new Command(() =>
            {
                var widthPopup = _ownPage.Width - 30;

                var sizePopup = new Size(widthPopup, widthPopup * 0.75);

                _ownPage.Navigation.ShowPopup(new DuckPopup
                {
                    Size = sizePopup
                });
            });
        }

        private void BwUpdaterOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;

            OnPropertyChanged(nameof(User));
            OnPropertyChanged(nameof(IsBusy));
        }

        private void BwUpdaterOnDoWork(object sender, DoWorkEventArgs e)
        {
            User.Update("https://192.168.0.102:5001");
            User.UpdateReports("https://192.168.0.102:5001");
        }

        public Command UpdateUserCommand { get; private set; }

        public Command OpenSettingsCommand { get; private set; }

        public Command LogoutCommand { get; private set; }

        public Command OpenDuckPopupCommand { get; private set; }
    }
}