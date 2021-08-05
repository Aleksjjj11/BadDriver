using System.IO;
using MobileApps.ViewModels;
using SkiaSharp;
using Xamarin.Forms;

namespace MobileApps.PopupViewModels
{
    public class ImageFullScreenViewModal : BaseViewModel
    {
        private Page _ownPage;

        public ImageFullScreenViewModal(ImageSource source, Page ownPage)
        {
            CurrentImage = source;
            _ownPage = ownPage;

            SKBitmap image = null;

            switch (source)
            {
                case FileImageSource imageSource:
                {
                    image = SKBitmap.Decode(imageSource.File);
                    break;
                }
                case UriImageSource uriImageSource:
                {
                    var httpClient = new System.Net.Http.HttpClient();
                    var bytes = httpClient.GetByteArrayAsync(uriImageSource.Uri.ToString()).Result;
                    var stream = new MemoryStream(bytes);

                    image = SKBitmap.Decode(stream);
                    break;
                }
            }

            var widthPopup = _ownPage.Width - 30;

            if (image is null)
            {
                SizePopup = new Size(widthPopup, widthPopup * 0.75);
            }
            else
            {
                double ratio = (double)image.Height / image.Width;

                SizePopup = new Size(widthPopup, widthPopup * ratio);
            }
        }

        private Size _sizePopup;
        public Size SizePopup
        {
            get => _sizePopup;
            set
            {
                _sizePopup = value;
                OnPropertyChanged(nameof(SizePopup));
            }
        }

        private ImageSource _currentImage;
        public ImageSource CurrentImage
        {
            get => _currentImage;
            set
            {
                _currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }
    }
}