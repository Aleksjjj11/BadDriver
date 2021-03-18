using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MobileApps.Interfaces;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

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
        public void Update(string ipUrl = "http://188.225.83.42:7000")
        {
            //Получеам токены 
            var client = new RestClient($"{ipUrl}/auth/login/");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("username", this.Username);
            request.AddParameter("password", this.Password);
            IRestResponse response = client.Execute(request);
            JObject responseJson = new JObject(JObject.Parse(response.Content));
            string accessToken = responseJson["access"]?.ToString();

            if (accessToken is null)
                throw new Exception("Access token is null!");
            //Если токены успешно получены, необходимо получить полную информацию о юзере
            client = new RestClient($"{ipUrl}/reports/user-info/");
            client.Timeout = -1;
            request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            response = client.Execute(request);
            string content = response.Content.Replace("\\", "");
            JObject userInfoJson = new JObject(JObject.Parse(JArray.Parse(content)[0].ToString()));
            try
            {
                Username = userInfoJson["username"].ToString();
                Email = userInfoJson["email"].ToString();
                FirstName = userInfoJson["first_name"].ToString();
                LastName = userInfoJson["last_name"].ToString();
            } catch (Exception ex)
            {
                Log.Warning("Update", $"Info wasn't updated.\n{ex.Message}");
            }

        }

        public void UpdateReports(string ipUrl = "http://188.225.83.42:7000")
        {
            //Получеам токены 
            var client = new RestClient($"{ipUrl}/auth/login/");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("username", this.Username);
            request.AddParameter("password", this.Password);
            IRestResponse response = client.Execute(request);
            JObject responseJson = new JObject(JObject.Parse(response.Content));
            string accessToken = responseJson["access"]?.ToString();

            if (accessToken is null)
                throw new Exception("Access token is null!");

            client = new RestClient($"{ipUrl}/reports/user-reports/");
            client.Timeout = -1;
            request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            response = client.Execute(request);
            JArray reportsJArray = new JArray(JArray.Parse(response.Content));
            try
            {
                Reports.Clear();
                for (int i = 0; i < reportsJArray.Count; i++)
                {
                    string carNumber = reportsJArray[i]["car_number"].ToString();
                    string carRegion = reportsJArray[i]["car_region"].ToString();
                    string carCountry = reportsJArray[i]["car_country"].ToString();
                    string description = reportsJArray[i]["description"].ToString();
                    var dateTime = DateTime.Parse(reportsJArray[i]["data"].ToString());
                    var images = new ObservableCollection<string>();
                    StatusReport statusReport = (StatusReport)reportsJArray[i]["status"].ToObject<int>();
                    images.Add(reportsJArray[i]["image_1"].ToString());
                    images.Add(reportsJArray[i]["image_2"].ToString());
                    images.Add(reportsJArray[i]["image_3"].ToString());
                    var report = new Report(new Car(carNumber, carRegion, carCountry), images, dateTime, description, statusReport);
                    Reports.Add(report);
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Update", $"Reports weren't updated.\n{ex.Message}");
            }
        }

        public void SendReport(IReport report, string ipUrl = "http://188.225.83.42:7000")
        {
            //Получеам токены 
            var client = new RestClient($"{ipUrl}/auth/login/");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("username", this.Username);
            request.AddParameter("password", this.Password);
            IRestResponse response = client.Execute(request);
            JObject responseJson = new JObject(JObject.Parse(response.Content));
            string accessToken = responseJson["access"]?.ToString();

            if (accessToken is null)
                throw new Exception("Access token is null!");

            client = new RestClient($"{ipUrl}/reports/send/");
            client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddParameter("user_name", this.Username);
            request.AddParameter("car_number", report.BadCar.Number);
            request.AddParameter("car_region", report.BadCar.Region);
            request.AddParameter("car_country", report.BadCar.Country);
            request.AddParameter("data", report.DateReported.ToString("s"));
            request.AddParameter("status", (int)report.Status);
            request.AddParameter("description", report.Description);
            request.AddFile("image_1", report.ImagesPaths[0]);
            request.AddFile("image_2", report.ImagesPaths[1]);
            request.AddFile("image_3", report.ImagesPaths[2]);
            response = client.Execute(request);
        }
    }
}