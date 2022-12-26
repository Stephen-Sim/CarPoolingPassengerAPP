using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPoolingPassengerAPP.ViewModels.Request
{
    [QueryProperty(nameof(RequestId), nameof(RequestId))]
    public class CompletedTripPageViewModel : BindableObject
    {
        public CompletedTripPageViewModel()
        {
            Map = new Map();
            requestService = new RequestService();
        }
        public RequestService requestService { get; set; }

        private string requestId;
        public string RequestId
        {
            get { return requestId.ToString(); }
            set
            {
                requestId = Uri.UnescapeDataString(value ?? string.Empty);
                Map = new Map();
                this.LoadData();
            }
        }

        private RequestRequest request;

        public RequestRequest Request
        {
            get { return request; }
            set { request = value; }
        }

        private async void LoadData()
        {
            var map = Map;
            Request = await requestService.GetRequest(int.Parse(this.requestId));
            this.Title = $"{Request.RequestNumber} - {Request.Status} (RM {Request.Charges?.ToString("0.00")})";

            var StartPin = new Pin
            {
                Label = "My Start Location",
                Address = "",
                Type = PinType.Place,
                Position = new Position((double)Request.FromLatitude, (double)Request.FromLongitude)
            };

            var EndPin = new Pin
            {
                Label = "My Destination Location",
                Address = "",
                Type = PinType.Place,
                Position = new Position((double)Request.ToLatitude, (double)Request.ToLongitude)
            };

            map.Pins.Add(StartPin);
            map.Pins.Add(EndPin);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(FindMidPoint((double)Request.FromLatitude, (double)Request.FromLongitude, (double)Request.ToLatitude, (double)Request.ToLongitude), Distance.FromMiles(5)));

            Map = map;

            var completeInfo = await requestService.GetCompleteRequestInfo(int.Parse(this.requestId));

            if (completeInfo != null)
            {
                DriverName = completeInfo.DriverName;
                RatingValue = completeInfo.Rating == null ? 0 : (int) completeInfo.Rating;
                TempRatingValue = RatingValue;
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

        private Map map;
        public Map Map
        {
            get { return map; }
            set { map = value; OnPropertyChanged(); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        private string driverName;

        public string DriverName
        {
            get { return driverName; }
            set { driverName = value; OnPropertyChanged(); }
        }

        public int TempRatingValue { get; set; } = 0;

        private int ratingValue = 0;

        public int RatingValue
        {
            get { return ratingValue; }
            set { ratingValue = value; OnPropertyChanged(); }
        }

        public ICommand RatingBarTapped
        {
            get
            {
                return new Command(async () =>
                {
                    bool isRated = await Application.Current.MainPage.DisplayAlert("Alert", $"Are you sure to rate the experience as {RatingValue} star?", "Yes", "No");

                    if (isRated)
                    {
                        var res = await requestService.RateTrip(this.requestId, RatingValue);
                        TempRatingValue = RatingValue;

                        if (res)
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert", "You have succesfully rated the driver!!", "Ok");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert", "Operation Failed, Please Try Again!!", "Ok");
                        }
                    }
                    else
                    {
                        RatingValue = TempRatingValue;
                    }
                });
            }
        }
        public ICommand PlaceTheRequestButtonClicked
        {
            get
            {
                return new Command(async () =>
                {
                    var content = JsonConvert.SerializeObject(this.Request);
                    await Shell.Current.GoToAsync($"Home/{nameof(ConfirmRequestPage)}?StringRequest={content}");
                });
            }
        }
    }
}
