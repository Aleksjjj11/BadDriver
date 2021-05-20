using System;
using System.ComponentModel;
using MobileApps.Interfaces;
using MobileApps.Models;
using MobileApps.Views;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MobileApps.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private readonly Page _ownPage;
        private readonly BackgroundWorker _bwAuth;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(VisibleTitle));
            }
        }

        public bool VisibleTitle => !IsBusy;

        private bool _isAuthorization = true;
        public bool IsAuthorization
        {
            get => _isAuthorization;
            set
            {
                _isAuthorization = value;
                OnPropertyChanged(nameof(IsAuthorization));
                OnPropertyChanged(nameof(IsRegistration));
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title => IsAuthorization switch
        {
            true => "Авторизация",
            false => "Регистрация"
        };

        public bool IsRegistration => !IsAuthorization;

        public AuthorizationViewModel(Page page)
        {
            _ownPage = page;
            Username = App.CurrentUser?.Username;
            Password = App.CurrentUser?.Password;

            InitCommands();

            _bwAuth = new BackgroundWorker();
            _bwAuth.DoWork += BwAuthOnDoWork;
            _bwAuth.RunWorkerCompleted += BwAuthOnRunWorkerCompleted;
        }

        private void InitCommands()
        {
            ChangeFormCommand = new Command(() => { IsAuthorization = !IsAuthorization; });

            SendFormCommand = new Command(() =>
            {
                IsBusy = true;
                _bwAuth.RunWorkerAsync();
            }, () => IsBusy == false);

            PropertyChanged += (_, __) =>
            {
                SendFormCommand.ChangeCanExecute();
            };
        }

        private void BwAuthOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                    {
                        if (e?.Result is User)
                        {
                            var user = e.Result as IUser;
                            App.CurrentUser = user;
                            (_ownPage as AuthorizationPage).BackPressed();
                            Preferences.Set("username", App.CurrentUser?.Username);
                            Preferences.Set("password", App.CurrentUser?.Password);
                            _ownPage.DisplayToastAsync("Вы успешно зарегистрировались");
                        }

                        if (e?.Result is string)
                        {
                            TextError = e?.Result.ToString();
                        }

                        if (e?.Result is string result)
                        {
                            _ownPage.DisplayToastAsync(result);
                        }

                        break;
                    }
                    case Device.iOS:
                    {
                        break;
                    }
                    default: break;
                }
            }
            catch (Exception exception)
            {
                _ownPage.DisplayToastAsync(exception.Message);
            }
        }

        private void BwAuthOnDoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = IsAuthorization ? AuthorizationRequest() : RegistrationRequest();
            if (e.Result is User user)
            {
                user?.Update();
                user?.UpdateReports();
                user?.GetAllAchievements();
            }
        }

        private string _username;
        public string Username  
        {
            get => _username;
            set 
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            } 
        }

        private string _repeatPassword;
        public string RepeatPassword
        {
            get => _repeatPassword;
            set
            {
                _repeatPassword = value;
                OnPropertyChanged(nameof(RepeatPassword));
            } 
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string _lastName;
        public string LastName  
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public Command ChangeFormCommand { get; private set; }

        public Command SendFormCommand { get; private set; }

        private string _textError;
        public string TextError
        {
            get => _textError;
            set
            {
                _textError = value;
                OnPropertyChanged(nameof(TextError));
            }
        }

        private bool IsCorrectData()
        {
            if (IsAuthorization)
            {
                return string.IsNullOrWhiteSpace(Username) == false && 
                       string.IsNullOrWhiteSpace(Password) == false;
            }

            return string.IsNullOrWhiteSpace(Username) == false &&
                   string.IsNullOrWhiteSpace(Password) == false &&
                   string.IsNullOrWhiteSpace(RepeatPassword) == false &&
                   string.IsNullOrWhiteSpace(FirstName) == false &&
                   string.IsNullOrWhiteSpace(LastName) == false &&
                   string.IsNullOrWhiteSpace(Email) == false &&
                   Password == RepeatPassword;
        }

        private string AuthorizationRequest()
        {
            if (IsCorrectData() == false)
            {
                return "Проверьте логин и пароль";
            }
            
            IUser user = new User
            {
                Username = Username,
                Password = Password
            };
            try
            {
                (user as User)?.Update();
                (user as User)?.UpdateReports();
                App.CurrentUser = user;
                (_ownPage as AuthorizationPage).BackPressed();
                Preferences.Set("username", App.CurrentUser?.Username);
                Preferences.Set("password", App.CurrentUser?.Password);
                return "Вы успешно авторизировались!";
            }
            catch (InvalidOperationException ex)
            {
                Preferences.Set("username", App.CurrentUser?.Username);
                Preferences.Set("password", App.CurrentUser?.Password);
                return "Вы успешно авторизировались!";
            }
            catch (Exception ex)
            {
                Log.Warning("AuthError", ex.Message);
                return "Проверьте логин и пароль";
            }
        }

        private object RegistrationRequest(string ipUrl = "http://188.225.83.42:7000")
        {
            if (IsCorrectData() == false)
            {
                return "Проверьте введённые данные";
            }
            
            var client = new RestClient($"{ipUrl}/auth/register/");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("username", Username);
            request.AddParameter("password", Password);
            request.AddParameter("password2", RepeatPassword);
            request.AddParameter("email", Email);
            request.AddParameter("first_name", FirstName);
            request.AddParameter("last_name", LastName);
            IRestResponse response = client.Execute(request);
            JObject jObjectResponse = new JObject(JObject.Parse(response.Content));
            string username = jObjectResponse["username"]?.ToString();
            
            if (username != Username)
                return username;

            JArray passwordErrors = null;
            if (jObjectResponse["password"]?.ToString() is not null)
            {
                passwordErrors = new JArray(JArray.Parse(jObjectResponse["password"]?.ToString()));
            }
            
            string password = "";
            for (int i = 0; i < passwordErrors?.Count; i++)
            {
                password = passwordErrors[i].ToString();
                if (password != "")
                    return password;
            }
            
            string password2 = jObjectResponse["password2"]?.ToString();
            if (password2 != RepeatPassword && password2 is not null)
                return password2;

            string email = jObjectResponse["email"]?.ToString();
            if (email != Email)
                return email;

            string firstName = jObjectResponse["first_name"]?.ToString();
            if (firstName != FirstName)
                return firstName;

            string lastName = jObjectResponse["last_name"]?.ToString();
            if (lastName != LastName)
                return lastName;

            if (username == Username)
            {
                IUser user = new User
                {
                    Username = Username,
                    Password = Password
                };
                return user;
            }

            return "Какие-то шоколадки";
        }
    }
}