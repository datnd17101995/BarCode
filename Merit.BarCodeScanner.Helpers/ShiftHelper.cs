using Merit.BarCodeScanner.Data;
using Merit.BarCodeScanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merit.BarCodeScanner.Helpers
{
    public class ShiftHelper
    {
        //private barCodeDbContext dbContext;
        public ShiftHelper()
        {
            //dbContext = new barCodeDbContext();
        }

        public static LocationShiftProjection GetShift(barCodeDbContext dbContext, DeliveryBlock block)
        {
            List<LocationShift> locationShifts = dbContext.LocationShifts.Where(l => l.LocationId == 35).ToList();
            Func<TimeSpan, TimeSpan, double> GetMedianDiff = (dt1, dt2) =>
            {
                if (dt1 < dt2)
                {
                    var tmp = dt2;
                    dt2 = dt1;
                    dt1 = tmp;
                }
                return Math.Min(Math.Abs((dt1 - dt2).TotalMinutes), Math.Abs((dt1 - dt2).TotalMinutes - 24 * 60));
            };
            var locationShiftsStart = locationShifts.Select(x => new Tuple<double, LocationShift>(GetMedianDiff(x.Start.TimeOfDay, block.BlockStartTime.Value.TimeOfDay), x));
            var locationShiftsEnd = locationShifts.Select(x => new Tuple<double, LocationShift>(GetMedianDiff(x.End.TimeOfDay, block.BlockStartTime.Value.TimeOfDay), x));
            var shift = FindShiftForLocation(block.BlockStartTime.Value, locationShifts) ??
                        locationShiftsStart
                        .Union(locationShiftsEnd)
                        .OrderBy(x => x.Item1).Select(x => x.Item2).FirstOrDefault();
            var dateOfShift = block.BlockStartTime.Value;
            if (shift.IsPreviousDay(block.BlockStartTime.Value.TimeOfDay))
            {
                dateOfShift=dateOfShift.AddDays(-1);
            }
            return new LocationShiftProjection
            {
                LocationId = shift.LocationId,
                ShiftId = shift.ShiftId,
                Start = shift.Start,
                End = shift.End,
                SpansDays = shift.SpansDays,
                DateOfShift = dateOfShift
            };
        }

        public static LocationShift FindShiftForLocation(DateTime localCreateTime, List<LocationShift> locationShifts)
        {
            var result = locationShifts.FirstOrDefault(s =>
                    TimeSpan.Compare(s.Start.TimeOfDay, s.End.TimeOfDay) <= 0
                    ? TimeSpan.Compare(localCreateTime.TimeOfDay, s.Start.TimeOfDay) >= 0 && TimeSpan.Compare(localCreateTime.TimeOfDay, s.End.TimeOfDay) <= 0
                    : TimeSpan.Compare(localCreateTime.TimeOfDay, s.Start.TimeOfDay) >= 0 || TimeSpan.Compare(localCreateTime.TimeOfDay, s.End.TimeOfDay) <= 0);
            return result;
        }
    }
}
