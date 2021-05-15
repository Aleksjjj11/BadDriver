﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
//using Android.Graphics;
//using Android.Widget;
using MobileApps.Interfaces;
using MobileApps.Models;
using MobileApps.Popups;
using SkiaSharp;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
//using Application = Android.App.Application;
using Path = System.IO.Path;

namespace MobileApps.ViewModels
{
    public class NewReportViewModel : BaseViewModel
    {
        private readonly Page _ownPage;
        private  IUser _user => App.CurrentUser;
        private readonly BackgroundWorker _bwSenderReport;
        
        public NewReportViewModel(Page page)
        {
            _ownPage = page;
            CompressedImagesPathsCollection = new ObservableCollection<ImageSource>();
            ImagesPathsCollection = new ObservableCollection<string>();
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
            };
        }

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

        public bool IsFullBlank => ValidateData();

        private bool ValidateData()
        {
            bool result = true;
            //Проверка верно введённых данных
            result = IsCorrectData() && CompressedImagesPathsCollection.Count == 3;
            return result;
        }

        private void BwSenderReportOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Toast.MakeText(Application.Context, (string)e.UserState, ToastLength.Long)?.Show();
        }

        private void BwSenderReportOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
            //Toast.MakeText(Application.Context, "Жалоба отправлена!", ToastLength.Long);
            _ownPage.SendBackButtonPressed();
        }

        private void BwSenderReportOnDoWork(object sender, DoWorkEventArgs e)
        {
            IsBusy = true;
            //Toast.MakeText(Application.Context, "Всё верно, отправляем жалобу!", ToastLength.Long);
            _bwSenderReport.ReportProgress(3, "Всё верно, отправляем жалобу!");
            //Дальше будет отправка запроса на сервер
            (_user as User)?.SendReport(new Report(new Car(NumberCar, RegionCar, CountryCar), ImagesPathsCollection, DateTime.Now, Description, StatusReport.Processing));
        }
        
        public Command OpenImageFullScreenCommand => new Command<ImageSource>(image =>
        {
            _ownPage.Navigation.ShowPopup(new ImageFullScreenPopup(image, _ownPage));
        });

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
            OnPropertyChanged(nameof(IsFullBlank));
            _ownPage.ForceLayout();
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

                string pathResizedPhoto = ImageResizer.ResizeImage(1024, 100, photo);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto));
                // ImagesPathsCollection.Add(photo.FullPath);
                ImagesPathsCollection.Add(pathResizedPhoto);

                var photo2 = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 2 фото",
                });

                string pathResizedPhoto2 = ImageResizer.ResizeImage(1024, 100, photo2);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto2));
                // ImagesPathsCollection.Add(photo2.FullPath);
                ImagesPathsCollection.Add(pathResizedPhoto2);

                var photo3 = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите 3 фото",
                });

                string pathResizedPhoto3 = ImageResizer.ResizeImage(1024, 100, photo3);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto3));
                // ImagesPathsCollection.Add(photo3.FullPath);
                ImagesPathsCollection.Add(pathResizedPhoto3);
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
            OnPropertyChanged(nameof(IsFullBlank));
            _ownPage.ForceLayout();
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

                string pathResizedPhoto = ImageResizer.ResizeImage(1024, 100, photo);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto));
                // ImagesPathsCollection.Add(photo.FullPath);
                ImagesPathsCollection.Add(pathResizedPhoto);

                var photo2 = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 2 фото",
                });

                string pathResizedPhoto2 = ImageResizer.ResizeImage(1024, 100, photo2);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto2));
                // ImagesPathsCollection.Add(photo2.FullPath);
                ImagesPathsCollection.Add(pathResizedPhoto2);

                var photo3 = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Сделайте 3 фото",
                });

                string pathResizedPhoto3 = ImageResizer.ResizeImage(1024, 100, photo3);
                CompressedImagesPathsCollection.Add(ImageSource.FromFile(pathResizedPhoto3));
                // ImagesPathsCollection.Add(photo3.FullPath);
                ImagesPathsCollection.Add(pathResizedPhoto3);

            }
            catch (Exception ex)
            {
                await _ownPage.DisplayAlert("Ошибка", ex.Message, "Ok");
            }
        }

        public ICommand SendReportCommand => new Command(() =>
        {
            _bwSenderReport.RunWorkerAsync();
        });

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
    }
}