using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MobileApps.Interfaces;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xamarin.Forms.Internals;

namespace MobileApps.Models
{
    public class User: INotifyPropertyChanged, IUser
    {
        public User()
        {
            Reports = new ObservableCollection<IReport>();
            Achievements = new ObservableCollection<IAchievement>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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

        public ObservableCollection<IReport> Reports { get; }

        public int CountDeclined => Reports.Count(report => report.Status == StatusReport.Declined);
        public int CountAccepted => Reports.Count(report => report.Status == StatusReport.Accepted);
        public int CountProcessing => Reports.Count(report => report.Status == StatusReport.Processing);

        public void Update(string ipUrl = "http://188.225.83.42:7000")
        {
            string accessToken = GetAccessesToken(ipUrl);

            if (accessToken is null)
                throw new Exception("Ошибка авторизации");

            var client = new RestClient($"{ipUrl}/reports/user-info/")
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {accessToken}");

            var response = client.Execute(request);

            if (response.StatusCode == 0)
                throw new Exception("Сервер не отвечает");

            string content = response.Content.Replace("\\", "");

            var userInfoJson = new JObject(JObject.Parse(JArray.Parse(content)[0].ToString()));

            try
            {
                Username = userInfoJson["username"]?.ToString();
                Email = userInfoJson["email"]?.ToString();
                FirstName = userInfoJson["first_name"]?.ToString();
                LastName = userInfoJson["last_name"]?.ToString();
            } 
            catch (Exception ex)
            {
                Log.Warning("Update", $"Info wasn't updated.\n{ex.Message}");
            }

        }

        public void UpdateReports(string ipUrl = "http://188.225.83.42:7000")
        {
            string accessToken = GetAccessesToken(ipUrl);

            if (accessToken is null)
                throw new Exception("Ошибка авторизации");

            var client = new RestClient($"{ipUrl}/reports/user-reports/")
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {accessToken}");

            var response = client.Execute(request);

            if (response.StatusCode == 0)
                throw new Exception("Сервер не отвечает");
            
            var reportsJArray = new JArray(JArray.Parse(response.Content));
            
            try
            {
                Reports.Clear();

                for (int i = 0; i < reportsJArray.Count; i++)
                {
                    var carNumber = reportsJArray[i]["car_number"]?.ToString();
                    var carRegion = reportsJArray[i]["car_region"]?.ToString();
                    var carCountry = reportsJArray[i]["car_country"]?.ToString();
                    var description = reportsJArray[i]["description"]?.ToString();
                    var dateTime = DateTime.Parse(reportsJArray[i]["data"]?.ToString());
                    var images = new ObservableCollection<string>();
                    var statusReport = (StatusReport)reportsJArray[i]["status"]?.ToObject<int>();

                    images.Add(reportsJArray[i]["image_1"]?.ToString());
                    images.Add(reportsJArray[i]["image_2"]?.ToString());
                    images.Add(reportsJArray[i]["image_3"]?.ToString());

                    var car = new Car(carNumber, carRegion, carCountry);
                    var report = new Report(car, images, dateTime, description, statusReport);

                    Reports.Add(report);
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Update", $"Reports weren't updated.\n{ex.Message}");
            }
        }

        public ObservableCollection<IAchievement> Achievements { get; set; }

        public void GetAllAchievements(string ipUrl = "http://188.225.83.42:7000")
        {
            string accessToken = GetAccessesToken();
            
            if (accessToken is null)
                throw new Exception("Ошибка авторизации");

            var achievementCollection = new ObservableCollection<IAchievement>();

            var client = new RestClient($"{ipUrl}/achivments/all_achivments/")
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            
            var response = client.Execute(request);
            
            if (response.StatusCode == 0)
                throw new Exception("Сервер не отвечает");
            
            var achievementsJArray = JArray.Parse(response.Content);
            
            for (int i = 0; i < achievementsJArray.Count; i++)
            {
                var name = achievementsJArray[i]["achivment_name"]?.ToString();
                
                if (name is null) continue;

                var description = achievementsJArray[i]["achivment_description"]?.ToString();
                
                if (description is null) continue;

                var bigImage = ipUrl + achievementsJArray[i]["big_image"];
                
                if (bigImage is null) continue;

                var smallImage = ipUrl + achievementsJArray[i]["small_image"];
                
                if (smallImage is null) continue;

                var achieve = new Achievement
                {
                    Name = name,
                    Description = description,
                    SmallImage = smallImage,
                    BigImage = bigImage
                };
                
                achievementCollection.Add(achieve);
            }

            Achievements = achievementCollection;
        }

        public void SendReport(IReport report, string ipUrl = "http://188.225.83.42:7000")
        {
            string accessToken = GetAccessesToken(ipUrl);

            if (accessToken is null)
                throw new Exception("Access token is null!");

            var client = new RestClient($"{ipUrl}/reports/send/")
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.POST);
            
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
            
            var response = client.Execute(request);
        }
        /// <summary>
        /// Метод отправляет запрос на авторизацию и получает токен доступа. При провальной авторизации возвращает null.
        /// </summary>
        /// <param name="ipUrl">Ссылка на сервер формата http://xxxx.xxxx.xxxx.xxxx:xxxx</param>
        /// <returns></returns>
        private string GetAccessesToken(string ipUrl = "http://188.225.83.42:7000")
        {
            var client = new RestClient($"{ipUrl}/auth/login/")
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.POST)
            {
                AlwaysMultipartFormData = true
            };

            request.AddParameter("username", this.Username);
            request.AddParameter("password", this.Password);

            var response = client.Execute(request);

            if (response.StatusCode == 0)
                throw new Exception("Сервер не отвечает");

            var responseJson = new JObject(JObject.Parse(response.Content));

            var accessToken = responseJson["access"]?.ToString();

            return accessToken;
        }
    }
}