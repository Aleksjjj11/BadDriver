using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MobileApps.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MobileApps.Models
{
    public class User: INotifyPropertyChanged, IUser
    {
        private string _accessToken;
        private string _refreshToken;
        private readonly HttpClient _httpClient;

        public User()
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
            });

            Reports = new ObservableCollection<IReport>();
            Achievements = new ObservableCollection<IAchievement>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
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

        public async Task Update(string ipUrl)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{ipUrl}/api/User/about"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await CheckAndUpdateAccessToken();
                response = await _httpClient.SendAsync(request);
            }

            var stringResponse = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            var jsonResponse = JObject.Parse(stringResponse);

            try
            {
                Id = jsonResponse["id"].Value<int>();
                Username = jsonResponse["username"].ToString();
                Email = jsonResponse["email"].ToString();
                FirstName = jsonResponse["firstName"].ToString();
                LastName = jsonResponse["lastName"].ToString();
            }
            catch (Exception ex)
            {
                Log.Warning("Update", $"Info wasn't updated.\n{ex.Message}");
            }
        }

        public int CountProcessing => Reports.Count(report => report.Status == StatusReport.Processing);

        public async Task<bool> Login()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{App.IpAddress}/api/User/tokens/"), Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    username = Username,
                    password = Password
                }), Encoding.UTF8, "application/json")
            };

            try
            {
                _accessToken = string.Empty;
                _refreshToken = string.Empty;

                var response = await _httpClient.SendAsync(request);
                var stringResponse = await response.Content.ReadAsStringAsync();
                ReadResponseAndSetTokens(stringResponse);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            } 
            
            return string.IsNullOrWhiteSpace(_accessToken) == false && string.IsNullOrWhiteSpace(_refreshToken) == false;
        }

        public async Task UpdateReports(string ipUrl)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{ipUrl}/api/User/reports/"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");

            try
            {
                var response = _httpClient.SendAsync(request).Result;

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await CheckAndUpdateAccessToken();
                    response = _httpClient.SendAsync(request).Result;
                }

                var stringResponse = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"Status code: {response.StatusCode}\n" +
                                                   $"Message: {stringResponse}");
                }

                var jsonResponseReports = JArray.Parse(stringResponse);

                Reports.Clear();

                var cars = new List<Car>();

                foreach (var jsonResponseReport in jsonResponseReports)
                {
                    var description = jsonResponseReport["description"].ToString();
                    var imageUrl1 = jsonResponseReport["imageUrl1"].ToString();
                    var imageUrl2 = jsonResponseReport["imageUrl2"].ToString();
                    var imageUrl3 = jsonResponseReport["imageUrl3"].ToString();
                    var status = (StatusReport)jsonResponseReport["status"].Value<int>();
                    var dateCreated = jsonResponseReport["dateCreated"].Value<DateTime>();
                    var carId = int.Parse(jsonResponseReport["carId"].ToString());

                    var car = cars.FirstOrDefault(x => x.Id == carId);

                    if (car == null)
                    {
                        var getCarRequest = new HttpRequestMessage
                        {
                            RequestUri = new Uri($"{ipUrl}/api/Car?id={carId}"),
                            Method = HttpMethod.Get,
                        };
                        getCarRequest.Headers.Add("Authorization", $"Bearer {_accessToken}");

                        response = _httpClient.SendAsync(getCarRequest).Result;
                        stringResponse = await response.Content.ReadAsStringAsync();

                        car = JsonConvert.DeserializeObject<Car>(stringResponse);
                    }

                    var report = new Report(car, new ObservableCollection<string>
                    {
                        imageUrl1, imageUrl2, imageUrl3
                    }, dateCreated, description, status);

                    Reports.Add(report);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public ObservableCollection<IAchievement> Achievements { get; set; }

        public async Task<IEnumerable<IAchievement>> GetAllAchievements(string ipUrl)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{ipUrl}/api/Achievement/achievements/"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");

            var response = _httpClient.SendAsync(request).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await CheckAndUpdateAccessToken();
                response = _httpClient.SendAsync(request).Result;
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            try
            {
                var achievements = JsonConvert.DeserializeObject<List<Achievement>>(stringResponse);
                return achievements;
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayToastAsync(e.Message);
                return new List<IAchievement>();
            }
        }

        public async Task GetGivenAchievements(string ipUrl)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{ipUrl}/api/User/achievements/"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");

            var response = _httpClient.SendAsync(request).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await CheckAndUpdateAccessToken();
                response = _httpClient.SendAsync(request).Result;
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            try
            {
                var achievements = JsonConvert.DeserializeObject<List<Achievement>>(stringResponse);
                Achievements.Clear();
                Achievements = new ObservableCollection<IAchievement>(achievements);

                OnPropertyChanged(nameof(Achievements));
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayToastAsync(e.Message);
            }
        }

        public async Task<HttpResponseMessage> SendReport(IReport report, string ipUrl)
        {
            var car = report.BadCar;

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{ipUrl}/api/Report/create"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    userId = Id,
                    car = new
                    {
                        id = car.Id,
                        number = car.Number,
                        regionCode = car.Region,
                        countryCode = car.Country,
                        userId = 0
                    },
                    description = report.Description,
                    imageUrl1 = report.ImagesPaths[0],
                    imageUrl2 = report.ImagesPaths[1],
                    imageUrl3 = report.ImagesPaths[2],
                }), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await CheckAndUpdateAccessToken();
                    response = _httpClient.SendAsync(request).Result;
                }

                return response;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void SaveTokensAndUserInfoInProperty()
        {
            Preferences.Set("username", Username);
            Preferences.Set("password", Password);
            Preferences.Set("accessToken", _accessToken);
            Preferences.Set("refreshToken", _refreshToken);
        }

        public async Task CheckAndUpdateAccessToken()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{App.IpAddress}/api/User/access"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    accessToken = _accessToken,
                    refreshToken = _refreshToken
                }), Encoding.UTF8, "application/json")
            };

            _accessToken = string.Empty;
            _refreshToken = string.Empty;

            var response = await _httpClient.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();

            ReadResponseAndSetTokens(stringResponse);
        }

        private void ReadResponseAndSetTokens(string stringResponse)
        {
            var jsonResponse = JObject.Parse(stringResponse);

            _accessToken = jsonResponse["accessToken"]?.ToString();
            _refreshToken = jsonResponse["refreshToken"]?["tokenString"]?.ToString();
        }
    }
}