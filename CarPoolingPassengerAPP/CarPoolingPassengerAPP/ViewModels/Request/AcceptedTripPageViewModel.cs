using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views.Request.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPoolingPassengerAPP.ViewModels.Request
{
    [QueryProperty(nameof(RequestId), nameof(RequestId))]
    public class AcceptedTripPageViewModel : BindableObject
    {
        public AcceptedTripPageViewModel()
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

        private AcceptedTripInfo info;

        public AcceptedTripInfo Info
        {
            get { return info; }
            set { info = value; OnPropertyChanged(); }
        }

        private RequestRequest request;

        public RequestRequest Request
        {
            get { return request; }
            set { request = value; }
        }

        private ImageSource driverProfileImage = null;

        public ImageSource DriverProfileImage
        {
            get { return driverProfileImage; }
            set { driverProfileImage = value; OnPropertyChanged(); }
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

            var tripInfo = await requestService.GetAcceptedTripInfo(this.requestId);

            if (tripInfo != null)
            {
                Info = tripInfo;

                if (Info.DriverImage != null)
                {
                    Stream stream = new MemoryStream(Info.DriverImage);
                    DriverProfileImage = ImageSource.FromStream(() => stream);
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

        public ICommand ChatDriverFrameClicked
        {
            get
            {
                return new Command(async () =>
                {
                    await Shell.Current.GoToAsync($"Chat/{nameof(ChatDriverPage)}?RequestId={request.Id}");
                });
            }
        }
    }
}
