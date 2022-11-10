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

        public string Username { get; set; } = null;
        public string Password { get; set; } = null;
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
