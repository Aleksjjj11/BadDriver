using MobileApps.PopupViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace MobileApps.Popups
{
    public partial class ImageFullScreenPopup : Popup
    {
        public ImageFullScreenPopup(ImageSource source, Page page)
        {
            BindingContext = new ImageFullScreenViewModal(source, page);
            InitializeComponent();
        }
    }
}