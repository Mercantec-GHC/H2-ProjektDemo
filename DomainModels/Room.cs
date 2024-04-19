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
        public string RoomSize { get; set; }
        public string RoomType { get; set; }
        public bool IsAvailable { get; set; }
    }
}
