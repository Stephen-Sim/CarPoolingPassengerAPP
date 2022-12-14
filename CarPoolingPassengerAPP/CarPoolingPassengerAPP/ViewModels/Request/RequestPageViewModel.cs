using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using CarPoolingPassengerAPP.Views.Menu;
using CarPoolingPassengerAPP.Views.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarPoolingPassengerAPP.ViewModels.Request
{
    public class RequestPageViewModel : BindableObject
    {
        private List<RequestRequest> requests;

        public List<RequestRequest> Requests
        {
            get { return requests; }
            set { requests = value; OnPropertyChanged(); }
        }

        public RequestService requestService { get; set; }

        public RequestPageViewModel()
        {
            Requests = new List<RequestRequest>();
            requestService = new RequestService();
            this.LoadData();
        }

        private async void LoadData()
        {
            var res = await requestService.GetRequests();
            if (res != null)
            {
                Requests = res;
            }
        }

        private bool isRefreshing = false;

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { isRefreshing = value; OnPropertyChanged(); }
        }


        public ICommand ListViewRereshed
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    this.LoadData();
                    IsRefreshing = false;
                });
            }
        }

        public Command ListViewItemTapped
        {
            get
            {
                return new Command(async (sender) => {
                    var request = (RequestRequest)sender;

                    if (request.Status == "Searching")
                    {
                        await Shell.Current.GoToAsync($"Request/{nameof(SearchingTripPage)}?RequestId={request.Id}");
                    }
                    else if (request.Status == "Accepted")
                    {
                        await Shell.Current.GoToAsync($"Request/{nameof(AcceptedTripPage)}?RequestId={request.Id}");
                    }
                    else if (request.Status == "Canceled")
                    {
                        await Shell.Current.GoToAsync($"Request/{nameof(CanceledTripPage)}?RequestId={request.Id}");
                    }
                    else if (request.Status == "Completed")
                    {
                        await Shell.Current.GoToAsync($"Request/{nameof(CompletedTripPage)}?RequestId={request.Id}");
                    }
                });
            }
        }
    }
}
