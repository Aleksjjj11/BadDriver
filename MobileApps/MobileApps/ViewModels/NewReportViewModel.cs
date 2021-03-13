using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Android.Graphics;
using SkiaSharp;
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
                string pathResizedPhoto = ResizeImage(1024, 100, photo);
                ImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto));

                var photo2 = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 2 фото",
                });
                string pathResizedPhoto2 = ResizeImage(1024, 100, photo2);
                ImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto2));

                var photo3 = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 3 фото",
                });
                string pathResizedPhoto3 = ResizeImage(1024, 100, photo3);
                ImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto3));
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
                string pathResizedPhoto = ResizeImage(1024, 100, photo);
                ImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto));

                var photo2 = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 2 фото",
                });
                string pathResizedPhoto2 = ResizeImage(1024, 100, photo2);
                ImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto2));

                var photo3 = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 3 фото",
                });
                string pathResizedPhoto3 = ResizeImage(1024, 100, photo3);
                ImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto3));
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