using System.Collections.ObjectModel;
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
        public ObservableCollection<IReport> Reports { get; }

        public ReportsViewModel(Page page)
        {
            _ownPage = page;
            Reports ??= new ObservableCollection<IReport>();
            Reports.Add(new Report(new Car("A123BC", "186", "RUS"), new ObservableCollection<string>()));
            Reports.Add(new Report(new Car("A123BC", "186", "RUS"), new ObservableCollection<string>()));
            Reports.Add(new Report(new Car("A123BC", "186", "RUS"), new ObservableCollection<string>()));
            Reports.Add(new Report(new Car("A345BC", "86", "RUS"), new ObservableCollection<string>()));
            Reports.Add(new Report(new Car("A345BC", "86", "RUS"), new ObservableCollection<string>()));
        }

        public ICommand MoreInfoReportCommand => new Command<IReport>(x =>
        {
            
        }, x => true);

        public ICommand OpenNewReportPageCommand => new Command(() =>
        {
            _ownPage.Navigation.PushModalAsync(new NewReportPage());
        });
    }
}