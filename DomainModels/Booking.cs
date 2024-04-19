using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels
{
    public class Booking : Common
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
    }
}
