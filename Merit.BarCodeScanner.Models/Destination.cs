using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merit.BarCodeScanner.Models
{
    public class Destination
    {
        public string DestinationId { get; set; }

        public Guid BlockId { get; set; }

        public DateTime? WorkTime { get; set; }
    }
}
