using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merit.BarCodeScanner.Models
{
    public class Block
    {
        [Key]
        public Guid BlockId { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
