using CarPoolingPassengerAPP.Models;
using CarPoolingPassengerAPP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPoolingPassengerAPP.ViewModels.Request.Chat
{
    [QueryProperty(nameof(RequestId), nameof(RequestId))]
    public class ChatDriverPageViewModel : BindableObject
    {
        private string requestId;
        public string RequestId
        {
            get { return requestId.ToString(); }
            set
            {
                requestId = Uri.UnescapeDataString(value ?? string.Empty);
                this.LoadData();
            }
        }

        private RequestRequest request;
        public RequestRequest Request
        {
            get { return request; }
            set { request = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        public ICommand SendMessageCommand
        {
            get
            {
                return new Command(async () => {
                    if (!string.IsNullOrEmpty(Message))
                    {
                        await chatService.SendMessage(int.Parse(requestId), Message);
                        var tempList = Chats;
                        tempList.Add(new ChatMessage { Message = message, IsYourMessage = true, IsNotYourMessage = false });
                        Chats = new ObservableCollection<ChatMessage>(tempList);
                        Message = string.Empty;
                    }
                });
            }
        }

        public ChatDriverPageViewModel()
        {
            requestService = new RequestService();
            chatService = new ChatService();
            Chats = new ObservableCollection<ChatMessage>();
        }

        public ChatService chatService { get; set; }
        public RequestService requestService { get; set; }

        private ObservableCollection<ChatMessage> chat;
        public ObservableCollection<ChatMessage> Chats
        {
            get { return chat; }
            set { chat = value; OnPropertyChanged(); }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(); }
        }

        private async void LoadData()
        {
            Request = await requestService.GetRequest(int.Parse(this.requestId));
            this.Title = $"{Request.RequestNumber} - {Request.Status} (RM {Request.Charges?.ToString("0.00")})";

            try
            {
                var res = await chatService.GetChatsByRequestId(this.requestId);
                if (res != null)
                {
                    Chats = res;
                }

                await chatService.Connect(int.Parse(this.requestId));
                chatService.ReceiveMessageFromDriver((requestId, message) =>
                {
                    if (requestId == int.Parse(this.requestId))
                    {
                        var tempList = Chats;
                        tempList.Add(new ChatMessage { Message = message, IsYourMessage = false, IsNotYourMessage = true });
                        Chats = new ObservableCollection<ChatMessage>(tempList);
                    }
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
