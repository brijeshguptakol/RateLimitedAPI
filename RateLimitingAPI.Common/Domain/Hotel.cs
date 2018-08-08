using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimitingAPI.Common
{
    public class Hotel
    {
        public int HotelId { get; set; }
        public string City { get; set; }
        public string Room { get; set; }
        public decimal Price { get; set; }
    }
}
