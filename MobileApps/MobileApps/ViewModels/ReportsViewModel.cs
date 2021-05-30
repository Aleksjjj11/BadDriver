using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MobileApps.Interfaces;
using MobileApps.Views;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        private readonly Page _ownPage;
        private readonly BackgroundWorker _bwUpdater;

        public ObservableCollection<IReport> Reports => App.CurrentUser.Reports;

        public IReport[] ArrayReports => Reports.ToArray();
        public IReport SelectedReport { get; set; }

        public ReportsViewModel(Page page)
        {
            _ownPage = page;

            InitCommands();

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

        public LinearGradientBrush FrameBrush(StatusReport statusReport)
        {
            return statusReport switch
            {
                StatusReport.Processing => Application.Current.Resources["BlueGradientBrush"] as LinearGradientBrush,
                _ => Application.Current.Resources["BlueGradientBrush"] as LinearGradientBrush
            };
        }

        private void BwUpdaterOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;

            OnPropertyChanged(nameof(Reports));
            OnPropertyChanged(nameof(ArrayReports));
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

        public Command RefreshInfoCommand { get; private set; }
        public Command MoreInfoReportCommand { get; private set; }
        public Command OpenNewReportPageCommand { get; private set; }

        private void InitCommands()
        {
            RefreshInfoCommand = new Command(() =>
            {
                _bwUpdater.RunWorkerAsync();
            });

            MoreInfoReportCommand = new Command<IReport>(x =>
            {
                _ownPage.Navigation.PushModalAsync(new DetailReportInfoPage(x));
                SelectedReport = null;
            });

            OpenNewReportPageCommand = new Command(() =>
            {
                _ownPage.Navigation.PushModalAsync(new NewReportPage());
            });
        }
    }
}