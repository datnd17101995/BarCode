using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merit.BarCodeScanner.Models
{
    public class EmpWorking
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public string Day { get; set; }

        public DateTime? WorkTime { get; set; }
    }
}
