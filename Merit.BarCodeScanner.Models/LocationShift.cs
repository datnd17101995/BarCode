using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merit.BarCodeScanner.Models
{
    [Table("V_LocationShifts")]
    public class LocationShift
    {
        [Key]
        [Column(Order = 1)]
        public int LocationId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ShiftId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
