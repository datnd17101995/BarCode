using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merit.BarCodeScanner.Models
{
    [Table("LocationShifts")]
    public class LocationShift
    {
        public int LocationId { get; set; }

        public int ShiftId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
