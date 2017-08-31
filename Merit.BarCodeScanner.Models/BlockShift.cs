using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merit.BarCodeScanner.Models
{
    public class BlockShift
    {
        public int Id { get; set; }

        public Guid BlockId { get; set; }

        public int ShiftId { get; set; }
    }
}
