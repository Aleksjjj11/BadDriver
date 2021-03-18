﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using MobileApps.Interfaces;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MobileApps.Models
{
    public class Report : IReport
    {
        public DateTime DateReported { get; }
        public ICar BadCar { get; set; }
        public ObservableCollection<ImageSource> ImagesSources { get; }
        public ObservableCollection<string> ImagesPaths { get; }
        public StatusReport Status { get; set; }
        private Stream _imagePreview;

        public string TextStatus
        {
            get
            {
                return Status switch
                {
                    StatusReport.Processing => "В обработке",
                    StatusReport.Accepted => "Принято",
                    StatusReport.Declined => "Отклонено",
                    _ => "Неизвестный статус"
                };
        }
        }

        public Color ColorStatus => Status switch
        {
            StatusReport.Processing => Color.DeepSkyBlue,
            StatusReport.Accepted => Color.Green,
            StatusReport.Declined => Color.DarkRed
        };
        public string Description { get; set; }

        public ImageSource ImagePreview => _imagePreview is null ? null : ImageSource.FromStream(() => new StreamReader(_imagePreview).BaseStream);

        public Report(ICar car, ObservableCollection<string> imagesPaths, DateTime date, string description, StatusReport status)
        {
            DateReported = date;
            BadCar = car;
            ImagesPaths = imagesPaths;
            Status = status;
            Description = description;
            ImagesSources = new ObservableCollection<ImageSource>();
            foreach (var imagesPath in ImagesPaths)
            {
                /*ImagesSources.Add(ImageSource.FromStream(() => 
                    ResizeImage(1024, 100, new Uri($"http://188.225.83.42:7000{imagesPath}"))));*/
                ImagesSources.Add(ImageSource.FromUri(new Uri($"http://188.225.83.42:7000{imagesPath}")));
            }

            _imagePreview = ResizeImage(256, 100, new Uri($"http://188.225.83.42:7000{this.ImagesPaths[0]}"));
            Log.Warning("f", "a");
        }
        
        private Stream ResizeImage(int size, int quality, Uri imageUri)
        {
            try
            {
                using (var input = new WebClient().OpenRead(imageUri))
                {
                    using (var original = SKBitmap.Decode(input))
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
                                var data = image.Encode();
                                var result = data.AsStream();
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning("MyError", ex.Message);
                return null;
            }
        }
    }
}