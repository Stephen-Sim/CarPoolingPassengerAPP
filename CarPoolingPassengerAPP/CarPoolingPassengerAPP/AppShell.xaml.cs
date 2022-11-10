using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views.Auth;
using CarPoolingPassengerAPP.Views.Home;
using CarPoolingPassengerAPP.Views.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // Menu
            Routing.RegisterRoute($"Menu/{nameof(EditProfilePage)}", typeof(EditProfilePage));
            Routing.RegisterRoute($"Menu/{nameof(ChangePasswordPage)}", typeof(ChangePasswordPage));
            Routing.RegisterRoute($"Menu/{nameof(AboutTheAppPage)}", typeof(AboutTheAppPage));
        }
    }
}