using System;
using System.ComponentModel.DataAnnotations;

namespace Merit.BarCodeScanner.Models
{
    public class DeliveryBlock
    {

        [Key]
        public int Id { get; set; }

        public Guid BlockId { get; set; }

        public string DestinationId { get; set; }

        public string EmployeeId { get; set; }

        public string PalletId { get; set; }

        public DateTime? BlockStartTime { get; set; }

        public DateTime? BlockEndTime { get; set; }
    }
}
