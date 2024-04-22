using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels
{
    public class RoomType : Common
    {
        public string RoomName { get; set; }
        public float PricePerDay { get; set;}
        public int NumberOfBeds { get; set;}
    }
}
