using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views.Auth;
using CarPoolingPassengerAPP.Views.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarPoolingPassengerAPP.ViewModels.Menu
{
    public class MenuPageViewModel : BindableObject
    {
        public ICommand ExitFrameClicked
        {
            get
            {
                return new Command(async () =>
                {
                    App.Current.Properties.Clear();
                    await App.Current.MainPage.DisplayAlert("Alert", "You have logout!", "Ok");
                    await Shell.Current.GoToAsync($"//LoginPage");
                });
            }
        }

        public ICommand EditImageButtonClick
        {
            get
            {
                return new Command(async () =>
                {
                    await Shell.Current.GoToAsync($"Menu/{nameof(EditProfilePage)}");
                });
            }
        }

        public ICommand AboutTheAppFrameClicked
        {
            get
            {
                return new Command(async () =>
                {
                    await Shell.Current.GoToAsync($"Menu/{nameof(AboutTheAppPage)}");
                });
            }
        }

        public ICommand ChangePasswordFrameClicked
        {
            get
            {
                return new Command(async () =>
                {
                    await Shell.Current.GoToAsync($"Menu/{nameof(ChangePasswordPage)}");
                });
            }
        }

        public AuthService authService { get; set; }

        public MenuPageViewModel()
        {
            authService = new AuthService();
            this.GetUserDetial();
        }

        private User user;

        public User User
        {
            get { return user; }
            set { user = value; OnPropertyChanged(); }
        }

        private ImageSource profileImage = null;

        public ImageSource ProfileImage
        {
            get { return profileImage; }
            set { profileImage = value; OnPropertyChanged(); }
        }

        public async void GetUserDetial()
        {
            var token = Application.Current.Properties["token"] as string;
            var res = await authService.GetUserByToken(token);

            if (res != null)
                User =  res;

            if (User.ProfileImage != null)
            {
                Stream stream = new MemoryStream(User.ProfileImage);
                ProfileImage = ImageSource.FromStream(() => stream);
            }
        }
    }
}
