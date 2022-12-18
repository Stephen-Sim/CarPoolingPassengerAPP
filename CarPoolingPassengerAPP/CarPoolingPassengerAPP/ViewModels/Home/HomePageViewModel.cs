using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
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
                if (value == null)
                {
                    Map = new Map() { IsShowingUser = true };
                    stringRequest = string.Empty;
                    ButtonTextColor = "Gray"; 
                    ButtonText = "Place your Request";
                    ToButtonText = "To: Destination Location";
                    FromButtonText = "From: Current Location";
                    NumberOfPassengerEntry.Text = string.Empty;
                    return;
                }

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

                Map.Pins.Add(StartPin);
                Map.Pins.Add(EndPin);
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(FindMidPoint((double)Request.FromLatitude, (double)Request.FromLongitude, (double)Request.ToLatitude, (double)Request.ToLongitude), Distance.FromMiles(5)));

                ButtonTextColor = "Black";

                FromButtonText = Request.FromAddress.Length > 33 ? Request.FromAddress.Substring(0, 28) + "...." : Request.FromAddress;
                ToButtonText = Request.ToAddress.Length > 33 ? Request.ToAddress.Substring(0, 28) + "...." : Request.ToAddress;

                var distance = (decimal) Xamarin.Essentials.Location.CalculateDistance((double)Request.FromLatitude, (double)Request.FromLongitude, (double)this.Request.ToLatitude, (double)this.Request.ToLongitude, Xamarin.Essentials.DistanceUnits.Kilometers);
                ButtonText = $"Enter the Numbers of Passenger (Total : {distance.ToString("0.00")} KM)";

                if (!string.IsNullOrEmpty(NumberOfPassengerEntry.Text))
                {
                    var num = int.Parse(NumberOfPassengerEntry.Text);

                    if (num >= 5)
                    {
                        ButtonText = $"Passengers Could not more than 4";
                    }
                    else if (num <= 0)
                    {
                        ButtonText = $"Invalid Value";
                    }
                    else
                    {
                        Request.NumberOfPassengers = num;
                        Request.Charges = (decimal)Xamarin.Essentials.Location.CalculateDistance((double)Request.FromLatitude, (double)Request.FromLongitude, (double)this.Request.ToLatitude, (double)this.Request.ToLongitude, Xamarin.Essentials.DistanceUnits.Kilometers);
                        ButtonText = $"RM {(Request.Charges * num)?.ToString("0.00")} - Place Your Request";
                    }
                }

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

        public Entry NumberOfPassengerEntry { get; set; }

        private Map map;

        public Map Map
        {
            get { return map; }
            set { map = value; OnPropertyChanged(); }
        }
        public RequestRequest Request { get; set; }

        private string buttonText = "Place your Request";

        public string ButtonText
        {
            get { return buttonText; }
            set { buttonText = value; OnPropertyChanged(); }
        }

        private string buttonTextColor = "Gray";
        public string ButtonTextColor
        {
            get { return buttonTextColor; }
            set { buttonTextColor = value; OnPropertyChanged(); }
        }

        private string fromButtonText = "From: Current Location";

        public string FromButtonText
        {
            get { return fromButtonText; }
            set { fromButtonText = value; OnPropertyChanged(); }
        }

        private string toButtonText = "To: Destination Location";

        public string ToButtonText
        {
            get { return toButtonText; }
            set { toButtonText = value; OnPropertyChanged(); }
        }

        public HomePageViewModel()
        {
            Map = new Map() { IsShowingUser = true };
            NumberOfPassengerEntry = new Entry { Placeholder = "Numbers of Passenger", Keyboard = Keyboard.Numeric };
            NumberOfPassengerEntry.TextChanged += NumberOfPassengerEntry_TextChanged;
        }

        private void NumberOfPassengerEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Map.Pins.Count == 0)
            {
                ButtonText = $"Place your Request";
                return;
            }

            if (string.IsNullOrEmpty(NumberOfPassengerEntry.Text))
            {
                ButtonText = $"Enter the Numbers of Passenger";
                return;
            }

            var num = int.Parse(NumberOfPassengerEntry.Text);

            if (num >= 5)
            {
                ButtonText = $"Passengers Could not more than 4";
            }
            else if (num <= 0)
            {
                ButtonText = $"Invalid Value";
            }
            else
            {
                Request.NumberOfPassengers = num;
                Request.Charges = (decimal)Xamarin.Essentials.Location.CalculateDistance((double)Request.FromLatitude, (double)Request.FromLongitude, (double)this.Request.ToLatitude, (double)this.Request.ToLongitude, Xamarin.Essentials.DistanceUnits.Kilometers);
                ButtonText = $"RM {(Request.Charges * num)?.ToString("0.00")} - Place Your Request";
            }

        }

        public ICommand PinLacationStackLayoutTapped
        {
            get
            {
                return new Command(async () =>
                {
                    await Shell.Current.GoToAsync($"Home/{nameof(PinStartLocationPage)}");
                });
            }
        }

        public ICommand PlaceRequestButtonClicked
        {
            get
            {
                return new Command(async () =>
                {
                    if (string.IsNullOrEmpty(stringRequest))
                    {
                        await Shell.Current.GoToAsync($"Home/{nameof(PinStartLocationPage)}");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(NumberOfPassengerEntry.Text))
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert", "Please Enter the Numbers of Passenger.", "Ok");
                            return;
                        }

                        var num = int.Parse(NumberOfPassengerEntry.Text);
                        
                        if (num >= 5)
                        {
                            ButtonText = $"Passengers Could not more than 4";
                            return;
                        }
                        else if (num <= 0)
                        {
                            ButtonText = $"Invalid Value";
                            return;
                        }

                        var content = JsonConvert.SerializeObject(this.Request);

                        await Shell.Current.GoToAsync($"Home/{nameof(ConfirmRequestPage)}?StringRequest={content}");
                    }
                });
            }
        }
    }
}
