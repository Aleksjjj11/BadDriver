using System;
using System.Collections.ObjectModel;
using System.IO;
using MobileApps.Interfaces;
using SkiaSharp;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class DetailReportInfoViewModel : BaseViewModel
    {
        private readonly IReport _report;

        public IReport Report => _report;
        public ObservableCollection<ImageSource> ImageSources => Report.ImagesSources;

        public DetailReportInfoViewModel(IReport report)
        {
            _report = report;
            //_imageSources = new ObservableCollection<UriImageSource>();
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

        public ImageSource CountryFlag
        {
            get
            {
                return Report.BadCar.Country switch
                {
                    "RUS" => (ImageSource)(new ImageSourceConverter().ConvertFromInvariantString("russia.png")),
                    "UA" => (ImageSource)(new ImageSourceConverter().ConvertFromInvariantString("ukraine.png")),
                    _ => (ImageSource)(new ImageSourceConverter().ConvertFromInvariantString("NonameFlag.png"))
                };
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
    }
}