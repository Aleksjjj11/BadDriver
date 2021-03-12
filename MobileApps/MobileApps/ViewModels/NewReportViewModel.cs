using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Android.Graphics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class NewReportViewModel : BaseViewModel
    {
        private readonly Page _ownPage;
        public ObservableCollection<ImageSource> ImagesPathsCollection { get; }

        public NewReportViewModel(Page page)
        {
            _ownPage = page;
            ImagesPathsCollection = new ObservableCollection<ImageSource>();
        }

        private string _countryCar;

        public string CountryCar
        {
            get => _countryCar;
            set
            {
                _countryCar = value.ToUpper();
                OnPropertyChanged(nameof(CountryFlag));
                OnPropertyChanged(nameof(CountryCar));
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
            }
        }

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

        public ICommand PickPhotoCommand => new Command(() =>
        {
            PickPhoto();
            OnPropertyChanged(nameof(ImagesPathsCollection));
        });

        private async void PickPhoto()
        {
            try
            {
                ImagesPathsCollection.Clear();
                
                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 1 фото",

                });
                var stream = await photo.OpenReadAsync();
                ImagesPathsCollection.Add(ImageSource.FromStream(() => stream));

                var photo2 = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 2 фото",
                });
                var stream2 = await photo2.OpenReadAsync();
                ImagesPathsCollection.Add(ImageSource.FromStream(() => stream2));

                var photo3 = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 3 фото",
                });
                var stream3 = await photo3.OpenReadAsync();
                ImagesPathsCollection.Add(ImageSource.FromStream(() => stream3));
            }
            catch (Exception ex)
            {
                await _ownPage.DisplayAlert("Ошибка", ex.Message, "Ok");
            }
        }

        public ICommand TakePhotosCommand => new Command(() =>
        {
            TakePhoto();
            OnPropertyChanged(nameof(ImagesPathsCollection));
        });

        private async void TakePhoto()
        {
            try
            {
                ImagesPathsCollection.Clear();
                
                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 1 фото",
                });
                //var stream = await photo.OpenReadAsync();
                ImagesPathsCollection.Add(ImageSource.FromFile(photo.FullPath));

                var photo2 = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 2 фото",
                });
                //var stream2 = await photo2.OpenReadAsync();
                ImagesPathsCollection.Add(ImageSource.FromFile(photo2.FullPath));

                var photo3 = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 3 фото",
                });
                //var stream3 = await photo3.OpenReadAsync();
                ImagesPathsCollection.Add(ImageSource.FromFile(photo3.FullPath));
            }
            catch (Exception ex)
            {
                await _ownPage.DisplayAlert("Ошибка", ex.Message, "Ok");
            }
        }

        public ICommand SendReportCommand => new Command(SendReportToServer);

        private async void SendReportToServer()
        {
            //Проверка верно введённых данных
            if (IsCorrectData() == false)
            {
                await _ownPage.DisplayAlert("Неверный формат", "Вы ввели номер машины неверного формата", "Ok");
                return;
            }
            //Проверка выбранных картинок
            if (ImagesPathsCollection.Count != 3)
            {
                await _ownPage.DisplayAlert("Нехватка данных", "Не были выбрана или сделаны фотографии нарушения.", "Исправить");
                return;
            }

            //Дальше будет отправка запроса на сервер

        }

        private bool IsCorrectData()
        {
            bool result = false;
            switch (CountryCar)
            {
                case "RUS":
                {
                    result = char.IsDigit(NumberCar[1]) && char.IsDigit(NumberCar[2]) &&
                             char.IsDigit(NumberCar[3]) && char.IsLetter(NumberCar[0]) &&
                             char.IsLetter(NumberCar[4]) && char.IsLetter(NumberCar[5]) &&
                             int.TryParse(RegionCar, out _);
                    break;
                }
                case "UA":
                {
                    break;
                }
                default:
                    break;
            }
            return result;
        }
    }
}