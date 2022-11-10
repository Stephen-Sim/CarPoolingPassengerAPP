using System;
using System.Collections.Generic;
using System.Text;

namespace CarPoolingPassengerAPP.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public byte[] ProfileImage { get; set; } = null;
    }
}
