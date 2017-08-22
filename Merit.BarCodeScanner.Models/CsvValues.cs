using System;

namespace Merit.BarCodeScanner.Models
{
    public class CsvValues
    {
        public Guid Id { get; set; }

        public int Line { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string BarCode { get; set; }

        public string Prefix { get; set; }

        public int Commas { get; set; }

        public int RowNumber { get; set; }

        public string RowType { get; set; }

        public bool IsBarCode { get; set; }

        public string DateTime
        {
            get { return string.Format("{0} {1}", Date, Time); }
        }
    }
}
