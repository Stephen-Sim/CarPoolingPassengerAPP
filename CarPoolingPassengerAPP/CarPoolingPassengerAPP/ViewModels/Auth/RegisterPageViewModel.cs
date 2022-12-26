using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views;
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
    public class RegisterPageViewModel : BindableObject
    {
        public AuthService authService { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand RegisterButtonClicked
        {
            get
            {
                return new Command(async () =>
                {
                    if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(PhoneNo) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "All the fields are required!!", "Ok");
                        return;
                    }

                    if (!Password.Equals(ConfirmPassword))
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Confirm Password is not matched!", "Ok");
                        return;
                    }

                    if (PhoneNo.Length < 10)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Invalid Phone Number Length", "Ok");
                        return;
                    }

                    if (Password.Length < 6)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Password length must be greater than or equal to 6", "Ok");
                        return;
                    }

                    var request = new RegisterRequest
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        PhoneNo = PhoneNo,
                        Password = Password
                    };

                    var res = await authService.Register(request);

                    if (res)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "You are successfully registered!!", "Ok");
                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Register Failed!!", "Ok");
                    }
                });
            }
        }

        public RegisterPageViewModel()
        {
            authService = new AuthService();
        }
    }
}
