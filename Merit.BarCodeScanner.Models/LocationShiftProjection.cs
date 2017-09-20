using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merit.BarCodeScanner.Models
{
    public class LocationShiftProjection
    {
        public int LocationId { get; set; }

        public int ShiftId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool SpansDays { get; set; }

        public DateTime? DateOfShift { get; set; }
    }
}
