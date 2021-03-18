using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MobileApps.Interfaces;
using MobileApps.Models;
using MobileApps.Views;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        private Page _ownPage;
        public ObservableCollection<IReport> Reports => App.CurrentUser.Reports;
        public IReport SelectedReport { get; set; }

        private BackgroundWorker _bwUpdater;

        public ReportsViewModel(Page page)
        {
            _ownPage = page;
            _bwUpdater = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };
            _bwUpdater.DoWork += BwUpdaterOnDoWork;
            _bwUpdater.ProgressChanged += BwUpdaterOnProgressChanged;
            _bwUpdater.RunWorkerCompleted += BwUpdaterOnRunWorkerCompleted;
            IsBusy = true;
            _bwUpdater.RunWorkerAsync();
        }

        private void BwUpdaterOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
            OnPropertyChanged(nameof(Reports));

        }

        private void BwUpdaterOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void BwUpdaterOnDoWork(object sender, DoWorkEventArgs e)
        {
            App.CurrentUser.UpdateReports("http://188.225.83.42:7000");
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        public ICommand RefreshInfoCommand => new Command(() =>
        {
            //App.CurrentUser.UpdateReports("http://188.225.83.42:7000");
            _bwUpdater.RunWorkerAsync();
        });

        public ICommand MoreInfoReportCommand => new Command(() =>
        {
            _ownPage.Navigation.PushModalAsync(new DetailReportInfoPage(SelectedReport));
            SelectedReport = null;
        });

        public ICommand OpenNewReportPageCommand => new Command(() =>
        {
            _ownPage.Navigation.PushModalAsync(new NewReportPage());
        });

    }
}