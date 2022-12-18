using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Views.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPoolingPassengerAPP.ViewModels.Home
{
    [QueryProperty(nameof(StartLocation), nameof(StartLocation))]
    public class PinEndLocationPageViewModel : BindableObject
    {
        private readonly Geocoder geocoder = new Geocoder();

        public Map Map { get; set; }

        public Position Position { get; set; } = new Position();

        public Pin StartPin { get; set; }

        private string startLocation;
        public string StartLocation
        {
            get { return startLocation; }
            set 
            {
                startLocation = Uri.UnescapeDataString(value ?? string.Empty);
                Request = JsonConvert.DeserializeObject<Request>(startLocation);

                StartPin = new Pin
                {
                    Label = "Start Location",
                    Address = "",
                    Type = PinType.Place,
                    Position = new Position((double)Request.FromLatitude, (double)Request.FromLongitude)
                };

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position((double)Request.FromLatitude, (double)Request.FromLongitude), Distance.FromMiles(1)));
                Map.Pins.Add(StartPin);
            }
        }

        private Request request;

        public Request Request
        {
            get { return request; }
            set { request = value; OnPropertyChanged(); }
        }

        public PinEndLocationPageViewModel()
        {
            Map = new Map() { IsShowingUser = true };
            Map.PropertyChanged += Map_PropertyChangedAsync;
        }

        private async void Map_PropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {
            var mapSender = (Map)sender;

            if (mapSender.VisibleRegion != null)
            {
                Position = new Position(mapSender.VisibleRegion.Center.Latitude, mapSender.VisibleRegion.Center.Longitude);
            }
        }

        public ICommand PinLocationButtonClicked
        {
            get
            {
                return new Command(async () =>
                {
                    var distance = Xamarin.Essentials.Location.CalculateDistance(this.Position.Latitude, this.Position.Longitude, (double)this.Request.FromLatitude, (double)this.Request.FromLongitude, Xamarin.Essentials.DistanceUnits.Kilometers);

                    if (distance > 30)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "The distance between two location have exceeded 30km.", "Select Again");
                        return;
                    }

                    var address = (await geocoder.GetAddressesForPositionAsync(this.Position)).FirstOrDefault()?.ToString();
                    bool result = await Application.Current.MainPage.DisplayAlert("Confirm the Location", $"{(address.Length > 100 ? address.Substring(0, 97) + "..." : address)}", "YES", "NO");


                    if (result)
                    {
                        var Request = new Request
                        {
                            FromLatitude = (decimal)this.Request.FromLatitude,
                            FromLongitude = (decimal)this.Request.FromLongitude,
                            FromAddress = this.Request.FromAddress,
                            ToLatitude = (decimal)this.Position.Latitude,
                            ToLongitude = (decimal)this.Position.Longitude,
                            ToAddress = address
                        };

                        var content = JsonConvert.SerializeObject(Request);

                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}?StringRequest={content}");
                    }
                });
            }
        }

        public string AddressInserted { get; set; } = "";

        public ICommand SearchButtonClicked
        {
            get
            {
                return new Command(async () =>
                {
                    var location = (await Xamarin.Essentials.Geocoding.GetLocationsAsync($"{AddressInserted}")).FirstOrDefault();

                    if (location != null)
                    {
                        Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMiles(1)));
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", $"Location Not Found!!", "Ok");
                    }
                });
            }
        }
    }
}
