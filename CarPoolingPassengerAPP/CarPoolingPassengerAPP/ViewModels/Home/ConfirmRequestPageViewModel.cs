using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPoolingPassengerAPP.ViewModels.Home
{
    [QueryProperty(nameof(StringRequest), nameof(StringRequest))]
    public class ConfirmRequestPageViewModel : BindableObject
    {
        private List<string> selectAddons = new List<string> { "white", "white", "white" };

        public List<string> SelectAddons
        {
            get { return selectAddons; }
            set { selectAddons = value; OnPropertyChanged(); }
        }

        private string addonsText = string.Empty;

        public string AddonsText
        {
            get { return addonsText; }
            set { addonsText = value; OnPropertyChanged(); }
        }


        private RequestRequest request;
        public RequestRequest Request
        {
            get { return request; }
            set { request = value; }
        }

        private string stringRequest = string.Empty;
        public string StringRequest
        {
            get { return stringRequest; }
            set
            {
                stringRequest = Uri.UnescapeDataString(value ?? string.Empty);
                Request = JsonConvert.DeserializeObject<RequestRequest>(stringRequest);

                var StartPin = new Pin
                {
                    Label = "Start Location",
                    Address = "",
                    Type = PinType.Place,
                    Position = new Position((double)Request.FromLatitude, (double)Request.FromLongitude)
                };

                var EndPin = new Pin
                {
                    Label = "Destination Location",
                    Address = "",
                    Type = PinType.Place,
                    Position = new Position((double)Request.ToLatitude, (double)Request.ToLongitude)
                };

                Request.Charges = Request.Charges * Request.NumberOfPassengers;

                AddonsText = $"Tatol Charges RM {Request.Charges?.ToString("0.00")} ({Request.NumberOfPassengers} 🤵), Select Add ons: ";

                Map.Pins.Add(StartPin);
                Map.Pins.Add(EndPin);
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(FindMidPoint((double)Request.FromLatitude, (double)Request.FromLongitude, (double)Request.ToLatitude, (double)Request.ToLongitude), Distance.FromMiles(5)));
            }
        }

        private Position FindMidPoint(double lat1, double lon1, double lat2, double lon2)
        {

            double dLon = (Math.PI / 180) * (lon2 - lon1);

            //convert to radians
            lat1 = (Math.PI / 180) * lat1;
            lat2 = (Math.PI / 180) * lat2;
            lon1 = (Math.PI / 180) * lon1;

            double Bx = Math.Cos(lat2) * Math.Cos(dLon);
            double By = Math.Cos(lat2) * Math.Sin(dLon);
            double lat3 = Math.Atan2(Math.Sin(lat1) + Math.Sin(lat2), Math.Sqrt((Math.Cos(lat1) + Bx) * (Math.Cos(lat1) + Bx) + By * By));
            double lon3 = lon1 + Math.Atan2(By, Math.Cos(lat1) + Bx);

            return new Position((180 / Math.PI) * lat3, (180 / Math.PI) * lon3);
        }

        public Map Map { get; set; }

        public ConfirmRequestPageViewModel()
        {
            Map = new Map();
            requestService = new RequestService();
        }

        public ICommand CancelButtonClicked
        {
            get
            {
                return new Command(async () => {
                    await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
                });
            }
        }

        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public TimeSpan SelectedTime { get; set; }

        public RequestService requestService { get; set; }

        public ICommand ConfirmButtonClicked
        {
            get
            {
                return new Command(async () => {

                    if (SelectedDate <= DateTime.Today)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "You can't Select Passed Date or Today", "Ok");
                        return;
                    }

                    Request.Date = SelectedDate;
                    Request.Time = SelectedTime;

                    var res = await requestService.CreateRequest(this.Request);

                    if (res)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "The Request is Successfully Created!!", "Ok");
                        await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Operation Failed!!", "Ok");
                    }
                });
            }
        }

        public ICommand AddonsFrameClicked
        {
            get
            {
                return new Command(async (x) =>
                {
                    var selectedAddonsIndex = int.Parse(x.ToString());

                    var temp = SelectAddons;

                    if (temp[selectedAddonsIndex] == "#f5c542")
                    {
                        temp[selectedAddonsIndex] = "white";
                        SelectAddons = temp;

                        Request.Charges -= (selectedAddonsIndex + 1) * 2;
                        AddonsText = $"Tatol Charges RM {Request.Charges?.ToString("0.00")} ({Request.NumberOfPassengers} 🤵), Select Add ons: ";
                    }
                    else
                    {
                        temp = new List<string> { "white", "white", "white" };
                        temp[selectedAddonsIndex] = "#f5c542";
                        Request.Charges -= (SelectAddons.FindIndex(y => y == "#f5c542") + 1) * 2;
                        SelectAddons = temp;

                        Request.Charges += (selectedAddonsIndex + 1) * 2;
                        AddonsText = $"Tatol Charges RM {Request.Charges?.ToString("0.00")} ({Request.NumberOfPassengers} 🤵), Select Add ons: ";
                    }
                });
            }
        }
    }
}
