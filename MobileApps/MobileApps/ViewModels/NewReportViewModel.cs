using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using MobileApps.Interfaces;
using MobileApps.Models;
using MobileApps.Popups;
using RestSharp;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MobileApps.ViewModels
{
    public class NewReportViewModel : BaseViewModel
    {
        private readonly Page _ownPage;
        private readonly BackgroundWorker _bwSenderReport;
        private readonly IUser _user = App.CurrentUser;
        private RestClient _restClient;
        private readonly List<FileResult> _fileResults = new();
        
        public NewReportViewModel(Page page)
        {
            _ownPage = page;

            GetCaptcha();
            
            CompressedImagesPathsCollection = new ObservableCollection<ImageSource>{ null, null, null };
            ImagesPathsCollection = new ObservableCollection<string> { null, null, null };

            InitCommands();

            _bwSenderReport = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            _bwSenderReport.DoWork += BwSenderReportOnDoWork;
            _bwSenderReport.RunWorkerCompleted += BwSenderReportOnRunWorkerCompleted;
            _bwSenderReport.ProgressChanged += BwSenderReportOnProgressChanged;

            CompressedImagesPathsCollection.CollectionChanged += (sender, args) =>
            {
                OnPropertyChanged(nameof(IsFullBlank));
                SendReportCommand.ChangeCanExecute();
            };
        }

        private void GetCaptcha()
        {
            IsLoadingCaptcha = true;

            Task<string>.Factory.StartNew(() =>
            {
                _restClient = new RestClient("https://xn--90adear.xn--p1ai/request_main")
                {
                    Timeout = -1
                };

                var request = new RestRequest(Method.POST);
                request.AddParameter("agree", "on");
                request.AddParameter("step", 2);

                var response = _restClient.Execute(request);
                var htmlParse = new HtmlParser();
                var document = htmlParse.ParseDocument(response.Content);
                var firstOrDefault = document.QuerySelectorAll("img.captcha-img").FirstOrDefault();
                var shortCaptchaUrl = firstOrDefault?.GetAttribute("src");

                return shortCaptchaUrl;
            }).ContinueWith(result =>
            {
                CaptchaUrl = $"https://xn--90adear.xn--p1ai/{result.Result}";
                IsLoadingCaptcha = false;
            });
        }

        private void InitCommands()
        {
            SendReportCommand = new Command(() =>
            {
                _bwSenderReport.RunWorkerAsync();
            }, () => IsFullBlank && IsCorrectData());

            OpenImageFullScreenCommand = new Command<ImageSource>(image =>
            {
                _ownPage.Navigation.ShowPopup(new ImageFullScreenPopup(image, _ownPage));
            });

            PickPhotoCommand = new Command<string>(PickPhoto);

            TakePhotosCommand = new Command<string>(TakePhoto);

            DeletePhotoCommand = new Command<string>(x =>
            {
                int value = int.Parse(x);

                CompressedImagesPathsCollection[value] = null;
                ImagesPathsCollection[value] = null;

                OnPropertyChanged(nameof(HasPhoto1));
                OnPropertyChanged(nameof(HasPhoto2));
                OnPropertyChanged(nameof(HasPhoto3));
                OnPropertyChanged(nameof(IsVisibleTakeOrPickPhoto1));
                OnPropertyChanged(nameof(IsVisibleTakeOrPickPhoto2));
                OnPropertyChanged(nameof(IsVisibleTakeOrPickPhoto3));
            });

            this.PropertyChanged += (_, __) =>
            {
                SendReportCommand.ChangeCanExecute();
            };
        }

        public ObservableCollection<ImageSource> CompressedImagesPathsCollection { get; }
        public ObservableCollection<string> ImagesPathsCollection { get; }

        private bool _isLoadingCaptcha;
        public bool IsLoadingCaptcha
        {
            get => _isLoadingCaptcha;
            set
            {
                _isLoadingCaptcha = value;
                OnPropertyChanged(nameof(IsLoadingCaptcha));
            }
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

        public bool IsFullBlank => ValidateData();

        public Command OpenImageFullScreenCommand { get; private set; }
        public Command PickPhotoCommand { get; private set; }
        public Command TakePhotosCommand { get; private set; }
        public Command SendReportCommand { get; private set; }
        public Command<string> DeletePhotoCommand { get; set; }


        private string _countryCar;
        public string CountryCar
        {
            get => _countryCar;
            set
            {
                _countryCar = value.ToUpper();
                OnPropertyChanged(nameof(CountryFlag));
                OnPropertyChanged(nameof(CountryCar));
                OnPropertyChanged(nameof(IsFullBlank));
            }
        }

        private string _numberCar;
        public string NumberCar
        {
            get => _numberCar;
            set
            {
                _numberCar = value.ToUpper();
                OnPropertyChanged(nameof(NumberCar));
                OnPropertyChanged(nameof(IsFullBlank));
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(IsFullBlank));
            }
        }

        private string _regionCar;
        public string RegionCar
        {
            get => _regionCar;
            set
            {
                _regionCar = value.ToUpper();
                OnPropertyChanged(nameof(RegionCar));
                OnPropertyChanged(nameof(IsFullBlank));
            }
        }

        private string _captchaUrl;
        public string CaptchaUrl
        {
            get => _captchaUrl;
            set
            {
                _captchaUrl = value;
                OnPropertyChanged(nameof(CaptchaUrl));
            }
        }

        private string _enteredCaptcha;
        public string EnteredCaptcha
        {
            get => _enteredCaptcha;
            set
            {
                _enteredCaptcha = value;
                OnPropertyChanged(nameof(EnteredCaptcha));
            }
        }

        public bool HasPhoto1 => CompressedImagesPathsCollection[0] != null;

        public bool IsVisibleTakeOrPickPhoto1 => !HasPhoto1;

        public bool HasPhoto2 => CompressedImagesPathsCollection[1] != null;

        public bool IsVisibleTakeOrPickPhoto2 => !HasPhoto2;

        public bool HasPhoto3 => CompressedImagesPathsCollection[2] != null;

        public bool IsVisibleTakeOrPickPhoto3 => !HasPhoto3;

        public ImageSource CountryFlag 
        {
            get
            {
                return CountryCar switch
                {
                    "RUS" => (ImageSource)(new ImageSourceConverter().ConvertFromInvariantString("russia.png")),
                    "UA" => (ImageSource)(new ImageSourceConverter().ConvertFromInvariantString("ukraine.png")),
                    _ => (ImageSource)(new ImageSourceConverter().ConvertFromInvariantString("NonameFlag.png"))
                };
            }
        }

        private bool ValidateData()
        {
            return IsCorrectData() &&
                   CompressedImagesPathsCollection.Count >= 3 &&
                   string.IsNullOrWhiteSpace(Description) == false;
        }

        private void BwSenderReportOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _ownPage.DisplayToastAsync(new ToastOptions
            {
                BackgroundColor = Color.FromHex("#c661cf"),
                Duration = TimeSpan.FromSeconds(500),
                MessageOptions = new MessageOptions
                {
                    Message = (string)e.UserState,
                    Foreground = Color.White
                }
            });
        }

        private void BwSenderReportOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is HttpResponseMessage response)
            {
                if (response?.StatusCode != HttpStatusCode.OK)
                {
                    _ownPage.DisplayAlert("Ошибка", $"Произошла проблема при отправке!!!\n{response.Content.ReadAsStringAsync().Result}", "OK");
                    return;
                }

                _ownPage.DisplayToastAsync(new ToastOptions
                {
                    BackgroundColor = Color.FromHex("#c661cf"),
                    Duration = TimeSpan.FromSeconds(500),
                    MessageOptions = new MessageOptions
                    {
                        Message = "Жалоба отлично отправлена!",
                        Foreground = Color.White
                    }
                });
                _ownPage.SendBackButtonPressed();
            }

            if (e.Result is Exception ex)
            {
                _ownPage.DisplayAlert("Ошибка", $"Произошла проблема при отправке!!!\n{ex.Message}", "OK");
            }

            IsBusy = false;
        }

        private void BwSenderReportOnDoWork(object sender, DoWorkEventArgs e)
        {
            IsBusy = true;

            _bwSenderReport.ReportProgress(3, "Всё верно, отправляем жалобу!");
            _bwSenderReport.ReportProgress(4, "Загружаем фотографии на сервер!");

            var loadedFileUrls = new List<string>();

            for (int i = 0; i < _fileResults.Count; i++)
            {
                _bwSenderReport.ReportProgress(i+5, $"Отправляем {i+1} фотографию!");

                var url = FirebaseManager.UploadImage(_fileResults[i].OpenReadAsync().Result, App.CurrentUser).Result;

                loadedFileUrls.Add(url);
            }

            var car = new Car(NumberCar, RegionCar, CountryCar);
            var report = new Report(car, loadedFileUrls, DateTime.Now, Description, StatusReport.Processing);

            var response = (_user as User)?.SendReport(report, App.IpAddress).Result;

            e.Result = response;
        }

        private async void PickPhoto(string index)
        {
            try
            {
                var intIndex = int.Parse(index);

                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 1 фото",

                });
                _fileResults.Add(photo);

                var pathResizedPhoto = ImageResizer.ResizeImage(1024, 100, photo);

                CompressedImagesPathsCollection[intIndex] = ImageSource.FromFile(pathResizedPhoto);
                ImagesPathsCollection[intIndex] = pathResizedPhoto;
                
                OnPropertyChanged(nameof(HasPhoto1));
                OnPropertyChanged(nameof(IsVisibleTakeOrPickPhoto1));
                OnPropertyChanged(nameof(HasPhoto2));
                OnPropertyChanged(nameof(IsVisibleTakeOrPickPhoto2));
                OnPropertyChanged(nameof(HasPhoto3));
                OnPropertyChanged(nameof(IsVisibleTakeOrPickPhoto3));
                OnPropertyChanged(nameof(CompressedImagesPathsCollection));
                OnPropertyChanged(nameof(IsFullBlank));

                _ownPage.ForceLayout();
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message, "Error");
            }
        }

        private async void TakePhoto(string index)
        {
            try
            {
                int intIndex = int.Parse(index);

                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 1 фото",
                });
                _fileResults.Add(photo);
                
                var pathResizedPhoto = ImageResizer.ResizeImage(1024, 100, photo);

                CompressedImagesPathsCollection[intIndex] = ImageSource.FromFile(pathResizedPhoto);
                ImagesPathsCollection[intIndex] = pathResizedPhoto;

                OnPropertyChanged(nameof(HasPhoto1));
                OnPropertyChanged(nameof(IsVisibleTakeOrPickPhoto1));
                OnPropertyChanged(nameof(HasPhoto2));
                OnPropertyChanged(nameof(IsVisibleTakeOrPickPhoto2));
                OnPropertyChanged(nameof(HasPhoto3));
                OnPropertyChanged(nameof(IsVisibleTakeOrPickPhoto3));
                OnPropertyChanged(nameof(CompressedImagesPathsCollection));
                OnPropertyChanged(nameof(IsFullBlank));

                _ownPage.ForceLayout();
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message, "Error");
            }
        }

        private bool IsCorrectData()
        {
            bool result = false;

            switch (CountryCar?.ToUpper())
            {
                case "RUS":
                {
                    result = char.IsDigit(NumberCar[1]) && char.IsDigit(NumberCar[2]) &&
                             char.IsDigit(NumberCar[3]) && char.IsLetter(NumberCar[0]) &&
                             char.IsLetter(NumberCar[4]) && char.IsLetter(NumberCar[5]) &&
                             RegionCar.All(char.IsDigit);
                    break;
                }
                case "UA":
                {
                    result = true;
                    break;
                }
                default:
                    break;
            }

            return result;
        }
    }
}