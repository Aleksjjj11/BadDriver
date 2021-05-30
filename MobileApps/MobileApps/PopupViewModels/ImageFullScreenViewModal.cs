using System.IO;
using MobileApps.ViewModels;
using SkiaSharp;
using Xamarin.Forms;

namespace MobileApps.PopupViewModels
{
    public class ImageFullScreenViewModal : BaseViewModel
    {
        private Page _ownPage;

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

        public ImageFullScreenViewModal(ImageSource source, Page ownPage)
        {
            CurrentImage = source;
            _ownPage = ownPage;

            SKBitmap image = null;

            if (source is FileImageSource imageSource)
            {
                image = SKBitmap.Decode(imageSource.File);
            }
            else if (source is UriImageSource uriImageSource)
            {
                var httpClient = new System.Net.Http.HttpClient();
                var bytes = httpClient.GetByteArrayAsync(uriImageSource.Uri.ToString()).Result;
                var stream = new MemoryStream(bytes);

                image = SKBitmap.Decode(stream);
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
    }
}