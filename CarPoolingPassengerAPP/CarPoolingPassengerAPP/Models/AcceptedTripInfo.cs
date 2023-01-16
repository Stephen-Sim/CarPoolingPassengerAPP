using System;
using System.Collections.Generic;
using System.Text;

namespace CarPoolingPassengerAPP.Models
{
    public class AcceptedTripInfo
    {
        public string DriverName { get; set; }
        public byte[] DriverImage { get; set; } = null;
        public string CarName { get; set; }
        public string CarPlatNo { get; set; }
        public string CarColor { get; set; }
        public string Rating { get; set; } = string.Empty;
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
