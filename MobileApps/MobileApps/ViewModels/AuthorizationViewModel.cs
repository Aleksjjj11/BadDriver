using MobileApps.Views;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private string _username;
        private Page _ownPage;
        private string _password;
        private string _repeatPassword;
        private string _email;
        private string _firstName;
        private string _lastName;

        public AuthorizationViewModel(Page page)
        {
            _ownPage = page;
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
        public string RepeatPassword
        {
            get => _repeatPassword;
            set
            {
                _repeatPassword = value;
                OnPropertyChanged(nameof(RepeatPassword));
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
    }
}