using System;
using System.ComponentModel;
using System.Windows.Input;
using Android.Widget;
using MobileApps.Interfaces;
using MobileApps.Models;
using MobileApps.Views;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Application = Android.App.Application;

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
        private BackgroundWorker _bwAuth;

        private bool _isAuthorization = true;

        public bool IsAuthorization
        {
            get => _isAuthorization;
            set
            {
                _isAuthorization = value;
                OnPropertyChanged(nameof(IsAuthorization));
                OnPropertyChanged(nameof(IsRegistration));
            }
        }

        public bool IsRegistration => !IsAuthorization;

        public AuthorizationViewModel(Page page)
        {
            _ownPage = page;
            _bwAuth = new BackgroundWorker();
            _bwAuth.DoWork += BwAuthOnDoWork;
            _bwAuth.RunWorkerCompleted += BwAuthOnRunWorkerCompleted;
        }

        private void BwAuthOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                {
                    if (e?.Result is User)
                    {
                        var user = e.Result as IUser;
                        (user as User)?.Update();
                        App.CurrentUser = user;
                        _ownPage.SendBackButtonPressed();
                        Toast.MakeText(Application.Context, "Вы успешно зарегистрировались", ToastLength.Long)?.Show();
                    }

                    if (e?.Result is string)
                        Toast.MakeText(Application.Context, e.Result as string, ToastLength.Long)?.Show();
                    
                    break;
                }
                case Device.iOS:
                {
                    break;
                }
                default: break;
            }
        }

        private void BwAuthOnDoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = IsAuthorization ? AuthorizationRequest() : RegistrationRequest();
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

        public ICommand ChangeForm => new Command(() =>
        {
            IsAuthorization = !IsAuthorization;
        });

        public ICommand SendForm => new Command(() =>
        {
            //Добавить отправку в background worker
            _bwAuth.RunWorkerAsync();
        });

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
                App.CurrentUser = user;
                _ownPage.SendBackButtonPressed();
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