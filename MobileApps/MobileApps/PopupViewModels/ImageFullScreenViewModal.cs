using System.IO;
using MobileApps.ViewModels;
using SkiaSharp;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace MobileApps.PopupViewModels
{
    public class ImageFullScreenViewModal : BaseViewModel
    {
        private ImageSource _source;
        private Page _ownPage;
        private Size _size;

        public Size SizePopup
        {
            get => _size;
            set => _size = value;
        }

        public ImageSource CurrentImage
        {
            get => _source;
            set
            {
                _source = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        public ImageFullScreenViewModal(ImageSource source, Page ownPage)
        {
            CurrentImage = source;
            _ownPage = ownPage;
            SKBitmap image = null;
            if (source is FileImageSource)
                image = SKBitmap.Decode((source as FileImageSource).File);
            else if (source is UriImageSource)
            {
                var httpClient = new System.Net.Http.HttpClient();
                var bytes = httpClient.GetByteArrayAsync((source as UriImageSource).Uri.ToString()).Result;
                var stream = new MemoryStream(bytes);
                image = SKBitmap.Decode(stream);
            }
            if (image is null)
                SizePopup = new Size(_ownPage.Width - 30, (_ownPage.Width - 30) * 0.75);
            else
            {
                double coaf = (double) image.Height / image.Width;
                SizePopup = new Size(_ownPage.Width - 30, (_ownPage.Width - 30) * coaf);
            }
        }
    }
}