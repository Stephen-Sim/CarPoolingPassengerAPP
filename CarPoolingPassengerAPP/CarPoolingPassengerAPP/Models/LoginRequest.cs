using System;
using System.Collections.Generic;
using System.Text;

namespace CarPoolingPassengerAPP.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsDriver { get; set; } = false;
    }
}
