using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Android.Graphics;
using MobileApps.Interfaces;
using MobileApps.Models;
using SkiaSharp;
using Xamarin.Essentials;
using Xamarin.Forms;
using Path = System.IO.Path;

namespace MobileApps.ViewModels
{
    public class NewReportViewModel : BaseViewModel
    {
        private readonly Page _ownPage;
        private  IUser _user => App.CurrentUser;
        public ObservableCollection<ImageSource> CompressedImagesPathsCollection { get; }
        public ObservableCollection<string> ImagesPathsCollection { get; }

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
        
        public NewReportViewModel(Page page)
        {
            _ownPage = page;
            CompressedImagesPathsCollection = new ObservableCollection<ImageSource>();
            ImagesPathsCollection = new ObservableCollection<string>();
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

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
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
            OnPropertyChanged(nameof(CompressedImagesPathsCollection));
        });

        private async void PickPhoto()
        {
            try
            {
                string folder = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);

                CompressedImagesPathsCollection.Clear();
                
                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 1 фото",

                });

                string pathResizedPhoto = ResizeImage(1024, 100, photo);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto));
                ImagesPathsCollection.Add(photo.FullPath);

                var photo2 = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 2 фото",
                });

                string pathResizedPhoto2 = ResizeImage(1024, 100, photo2);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto2));
                ImagesPathsCollection.Add(photo2.FullPath);

                var photo3 = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 3 фото",
                });

                string pathResizedPhoto3 = ResizeImage(1024, 100, photo3);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto3));
                ImagesPathsCollection.Add(photo3.FullPath);
            }
            catch (Exception ex)
            {
                await _ownPage.DisplayAlert("Ошибка", ex.Message, "Ok");
            }
        }

        public ICommand TakePhotosCommand => new Command(() =>
        {
            TakePhoto();
            OnPropertyChanged(nameof(CompressedImagesPathsCollection));
        });

        private async void TakePhoto()
        {
            try
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                
                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 1 фото",
                });

                string pathResizedPhoto = ResizeImage(1024, 100, photo);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto));
                ImagesPathsCollection.Add(photo.FullPath);

                var photo2 = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 2 фото",
                });

                string pathResizedPhoto2 = ResizeImage(1024, 100, photo2);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto2));
                ImagesPathsCollection.Add(photo2.FullPath);

                var photo3 = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 3 фото",
                });

                string pathResizedPhoto3 = ResizeImage(1024, 100, photo3);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto3));
                ImagesPathsCollection.Add(photo3.FullPath);

            }
            catch (Exception ex)
            {
                await _ownPage.DisplayAlert("Ошибка", ex.Message, "Ok");
            }
        }

        public ICommand SendReportCommand => new Command(SendReportToServer);

        private async void SendReportToServer()
        {
            IsBusy = true;
            //Проверка верно введённых данных
            if (IsCorrectData() == false)
            {
                await _ownPage.DisplayAlert("Неверный формат", "Вы ввели номер машины неверного формата", "Ok");
                return;
            }
            //Проверка выбранных картинок
            if (CompressedImagesPathsCollection.Count != 3)
            {
                await _ownPage.DisplayAlert("Нехватка данных", "Не были выбрана или сделаны фотографии нарушения.", "Исправить");
                return;
            }

            //Дальше будет отправка запроса на сервер
            (_user as User)?.SendReport(new Report(new Car(NumberCar, RegionCar, CountryCar), ImagesPathsCollection, DateTime.Now, Description, StatusReport.Processing));
            IsBusy = false;
            _ownPage.SendBackButtonPressed();
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

        private string ResizeImage(int size, int quality, FileResult streamImage)
        {
            using (var input = streamImage.OpenReadAsync().Result)
            {
                using (var inputStream = new SKManagedStream(input))
                {
                    using (var original = SKBitmap.Decode(inputStream))
                    {
                        int width, height;
                        if (original.Width > original.Height)
                        {
                            width = size;
                            height = original.Height * size / original.Width;
                        }
                        else
                        {
                            width = original.Width * size / original.Height;
                            height = size;
                        }

                        using (var resized = original.Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3))
                        {
                            if (resized == null) throw new Exception("Error resized.");

                            using (var image = SKImage.FromBitmap(resized))
                            {
                                string outputPath = streamImage.FullPath.Remove(streamImage.FullPath.Length - streamImage.FileName.Length, streamImage.FileName.Length);
                                using (var output =
                                File.OpenWrite($"{outputPath}/{DateTime.Now:O}_BadDrive.jpg"))
                                {
                                    image.Encode(SKEncodedImageFormat.Jpeg, quality).SaveTo(output);
                                    return output.Name;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}