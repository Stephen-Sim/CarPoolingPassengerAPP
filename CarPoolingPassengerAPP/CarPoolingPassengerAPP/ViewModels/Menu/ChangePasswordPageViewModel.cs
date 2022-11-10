using CarPoolingPassengerAPP.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarPoolingPassengerAPP.ViewModels.Menu
{
    public class ChangePasswordPageViewModel : BindableObject
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public AuthService authService { get; set; }
        public ChangePasswordPageViewModel()
        {
            authService = new AuthService();
        }

        public ICommand ConfirmChangeButtonClicked
        {
            get
            {
                return new Command(async () =>
                {
                    if (string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "All the fields are required!!", "Ok");
                        return;
                    }

                    if (NewPassword != ConfirmPassword)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Confirm Password is not matched!!", "Ok");
                        return;
                    }

                    var token = Application.Current.Properties["token"] as string;
                    var res = await authService.ChangePassword(token, OldPassword, NewPassword);

                    if (res)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Password is successfully changed!!", "Ok");
                        await Shell.Current.Navigation.PopAsync();
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Old Password is wrong!!", "Ok");
                    }
                });
            }
        }

        public ICommand CancelButtonClicked
        {
            get
            {
                return new Command(async () =>
                {
                    await Shell.Current.Navigation.PopAsync();
                });
            }
        }
    }
}
