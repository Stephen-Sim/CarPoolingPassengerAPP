using CarPoolingPassengerAPP.Views.Auth;
using CarPoolingPassengerAPP.Views.Home;
using CarPoolingPassengerAPP.Views.Menu;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarPoolingPassengerAPP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Auth
            Routing.RegisterRoute($"Auth/{nameof(RegisterPage)}", typeof(RegisterPage));

            // Home
            Routing.RegisterRoute($"Home/{nameof(PinStartLocationPage)}", typeof(PinStartLocationPage));
            Routing.RegisterRoute($"Home/{nameof(PinEndLocationPage)}", typeof(PinEndLocationPage));
            Routing.RegisterRoute($"Home/{nameof(ConfirmRequestPage)}", typeof(ConfirmRequestPage));
            
            // Menu
            Routing.RegisterRoute($"Menu/{nameof(EditProfilePage)}", typeof(EditProfilePage));
            Routing.RegisterRoute($"Menu/{nameof(ChangePasswordPage)}", typeof(ChangePasswordPage));
            Routing.RegisterRoute($"Menu/{nameof(AboutTheAppPage)}", typeof(AboutTheAppPage));
        }
    }
}