using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MobileApps.Interfaces;

namespace MobileApps.Models
{
    public class User: INotifyPropertyChanged, IUser
    {
        private string _username;
        private string _password;
        private string _email;
        private string _firstName;
        private string _lastName;

        public User()
        {
            Reports = new ObservableCollection<IReport>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public ObservableCollection<IReport> Reports { get; }
        public int CountDeclined => Reports.Count(report => report.Status == StatusReport.Declined);
        public int CountAccepted => Reports.Count(report => report.Status == StatusReport.Accepted);
        public int CountProcessing => Reports.Count(report => report.Status == StatusReport.Processing);
    }
}