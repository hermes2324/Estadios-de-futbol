
using Estadios_de_futbol.Views;

namespace Estadios_de_futbol
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Base_de_datos.Secretos.SyncfusionLicense);
            InitializeComponent();
            MainPage = new NavigationPage(new Views.MainTabbedPage())
            {
                BarBackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#121212"),
                BarTextColor = Microsoft.Maui.Graphics.Color.FromArgb("#F5C842")
            };
        }

       
    }
}