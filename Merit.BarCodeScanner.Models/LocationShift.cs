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

        public bool SpansDays { get; set; }

        public DateTime GetShiftStartDateTime(DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, date.Day, Start.Hour, Start.Minute, Start.Second);
            return start;
        }

        public DateTime GetShiftEndDateTime(DateTime date)
        {
            var finish = new DateTime(date.Year, date.Month, date.Day, End.Hour, End.Minute, End.Second);
            if (SpansDays || Start > End)
            {
                finish = finish.AddDays(1);
            }

            return finish;
        }

        /// <summary>
        /// Given current shift info returns true if this is a previous day shift relative to timeOfDay provided.
        /// </summary>
        /// <param name="timeOfDay"></param>
        /// <returns></returns>
        public bool IsPreviousDay(TimeSpan timeOfDay)
        {
            return SpansDays && Start.TimeOfDay > timeOfDay;
        }

        public DateTime DateOfShift(LocationShift shift, DateTime date)
        {
            if (shift.IsPreviousDay(date.TimeOfDay))
            {
                // this Work Order was created during previous day's shift
                date = date.AddDays(-1);
            }
            return date;
        }
    }
}
