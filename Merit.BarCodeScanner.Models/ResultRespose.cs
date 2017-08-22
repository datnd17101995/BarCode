using System;
namespace Merit.BarCodeScanner.Models
{
    public class ResultRespose
    {
        public bool Status { get; set; }
        public string Message { get; set; }

        public DateTime? StartDate { get; set; }
    }
}
