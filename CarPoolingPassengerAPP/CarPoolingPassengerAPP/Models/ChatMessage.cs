using System;
using System.Collections.Generic;
using System.Text;

namespace CarPoolingPassengerAPP.Models
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public bool? IsYourMessage { get; set; }
        public bool? IsNotYourMessage { get; set; }
    }
}
