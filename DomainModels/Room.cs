using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels
{
    public class Room : Common
    {
        public string RoomNumber { get; set; }
        public int RoomTypeId { get; set; }
        public List<string> pictureURLs { get; set; }
    }

    public class RoomDTO : Common
    {
        public string RoomNumber { get; set; }
        public string RoomName { get; set; }
        public float PricePerDay { get; set; }
        public int NumberOfBeds { get; set; }
        public List<string> pictureURLs { get; set; }
        public List<string> RoomPictures { get; set; }

    }
}
