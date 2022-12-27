using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views;
using CarPoolingPassengerAPP.Views.Auth;
using CarPoolingPassengerAPP.Views.Home;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarPoolingPassengerAPP
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            App.Current.MainPage = new AppShell();

            authService = new AuthService();
            _ = this.ValidateAuthentication();
        }

        public AuthService authService { get; set; }

        public async Task ValidateAuthentication()
        {
            var connect = await authService.ConnectToServer();
            if (!connect)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Server Down", "Ok");
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }

            if (Application.Current.Properties.ContainsKey("token"))
            {
                var token = Application.Current.Properties["token"] as string;
                var res = await authService.RefreshToken(token);

                if (res)
                {
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {
                    await Shell.Current.GoToAsync($"//LoginPage");
                    await App.Current.MainPage.DisplayAlert("Alert", "Session has Expired!!", "Ok");
                }
            }
            else
            {
                await Shell.Current.GoToAsync($"//LoginPage");
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
