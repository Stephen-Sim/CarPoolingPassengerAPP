using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPoolingPassengerAPP.ViewModels.Home
{
    [QueryProperty(nameof(StringRequest), nameof(StringRequest))]
    public class HomePageViewModel : BindableObject
    {
        private string stringRequest = string.Empty;
        public string StringRequest
        {
            get { return stringRequest; }
            set
            {
                stringRequest = Uri.UnescapeDataString(value ?? string.Empty);
                Request = JsonConvert.DeserializeObject<Request>(stringRequest);
            }
        }

        public Map Map { get; set; }
        public Request Request { get; set; }
        public string ButtonText { get; set; } = "Place your Request";
        public int NumberOfPass { get; set; } = 0;

        public HomePageViewModel()
        {
            Map = new Map() { IsShowingUser = true };
        }

        public ICommand PinStartLocationEditorClicked
        {
            get
            {
                return new Command(async () =>
                {
                    await Shell.Current.GoToAsync($"Home/{nameof(PinStartLocationPage)}");
                });
            }
        }
    }
}
