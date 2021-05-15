using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileApps.PopupViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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