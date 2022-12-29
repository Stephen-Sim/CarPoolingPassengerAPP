using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views.Auth;
using CarPoolingPassengerAPP.Views.Home;
using CarPoolingPassengerAPP.Views.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarPoolingPassengerAPP.ViewModels.Auth
{
    public class LoginPageViewModel : BindableObject
    {
        public AuthService authService { get; set; }

        private string username = string.Empty;

        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged(); }
        }

        private string password = string.Empty;

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }
        public ICommand SignInButtonClick
        {
            get
            {
                return new Command(async () =>
                {
                    if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "All the fields are required!!", "Ok");
                        return;
                    }

                    LoginRequest loginRequest = new LoginRequest
                    {
                        Username = Username,
                        Password = Password
                    };

                    var res = await authService.Login(loginRequest);

                    if (res)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "Successfully Login!!", "Ok");
                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                        Username = string.Empty;
                        Password = string.Empty;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "Fail to Login!!", "Ok");
                    }
                });
            }
        }

        public ICommand NewUserLabelClick
        {
            get
            {
                return new Command(async () =>
                {
                    await Shell.Current.GoToAsync($"Auth/{nameof(RegisterPage)}");
                });
            }
        }

        public LoginPageViewModel()
        {
            authService = new AuthService();
        }
    }
}
