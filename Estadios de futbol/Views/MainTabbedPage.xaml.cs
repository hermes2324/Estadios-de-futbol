// SIN ningún using de AndroidSpecific aquí
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace Estadios_de_futbol.Views;

public partial class MainTabbedPage : Microsoft.Maui.Controls.TabbedPage
{
    public MainTabbedPage()
    {
        InitializeComponent();

        On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
            .SetIsSwipePagingEnabled(false);
    }
}