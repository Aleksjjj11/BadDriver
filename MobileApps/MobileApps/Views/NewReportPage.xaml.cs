using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileApps.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApps.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewReportPage : ContentPage
    {
        public NewReportPage()
        {
            BindingContext = new NewReportViewModel(this);
            InitializeComponent();
        }
    }
}