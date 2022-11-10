using System;
using System.Collections.Generic;
using System.Text;

namespace CarPoolingPassengerAPP.Models
{
    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsDriver { get; set; } = false;
    }
}
