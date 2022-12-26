using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarPoolingPassengerAPP.ViewModels.Menu
{
    public class EditProfilePageViewModel : BindableObject
    {
        public AuthService authService { get; set; }
        public EditProfilePageViewModel()
        {
            authService = new AuthService();
            user = new User();
            this.GetUserDetial();
        }

        private bool [] gender = new bool[] { false, false };

        public bool [] Genders
        {
            get { return gender; }
            set { gender = value; OnPropertyChanged(); }
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
                User = res;

            if (User.ProfileImage != null)
            {
                Stream stream = new MemoryStream(User.ProfileImage);
                ProfileImage = ImageSource.FromStream(() => stream);
            }

            if (User.Gender != null)
            {
                if (User.Gender == "Male")
                {
                    Genders = new bool[] { true, false };
                }
                else
                {
                    Genders = new bool[] { false, true };
                }
            }
        }

        public ICommand CancelButtonClicked
        {
            get
            {
                return new Command(async () => await Shell.Current.Navigation.PopAsync());
            }
        }

        public ICommand ProfilePictureImageButtonClicked
        {
            get
            {
                return new Command(async () =>
                {
                    var res = await App.Current.MainPage.DisplayActionSheet("Options", "Cancel", null, "From File", "Camera", "Remove Profile");

                    if (res == "From File" || res == "Camera")
                    {
                        var image = res == "From File" ? await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                        {
                            Title = "Please pick a photo"
                        }) :
                        await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                        {
                            Title = "Please pick a photo"
                        });

                        if (image != null)
                        {
                            var stream = await image.OpenReadAsync();
                            ProfileImage = ImageSource.FromStream(() => stream);
                            user.ProfileImage = File.ReadAllBytes(image.FullPath);
                        }
                    }
                    else if (res == "Remove Profile")
                    {
                        user.ProfileImage = null;
                        ProfileImage = null;
                    }

                });
            }
        }

        public ICommand EditProfileButtonClicked
        {
            get
            {
                return new Command(async () => {

                    if (string.IsNullOrEmpty(User.FirstName) || string.IsNullOrEmpty(user.LastName) || string.IsNullOrEmpty(User.PhoneNo) || (Genders[0] == false && Genders[1] == false))
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "All the fields are required!!", "Ok");
                        return;
                    }

                    if (User.PhoneNo.Length < 10)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Invalid Phone Number Length", "Ok");
                        return;
                    }

                    User.Username = $"{User.FirstName.Trim()} {User.LastName.Trim()}";
                    User.Gender = Genders[0] == true ? "Male" : "Female";

                    var token = Application.Current.Properties["token"] as string;
                    var res = await authService.EditProfile(token, User);

                    if (res)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "User Profile is Edited!!", "Ok");
                        await Shell.Current.GoToAsync("///MenuPage");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Failed to Edit!!", "Ok");
                    }
                });
            }
        }
    }
}
