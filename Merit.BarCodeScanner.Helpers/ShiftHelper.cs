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

        public static LocationShift GetShift(barCodeDbContext dbContext,DeliveryBlock block)
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
            var shift = locationShiftsStart
                        .Union(locationShiftsEnd)
                        .OrderBy(x => x.Item1).Select(x => x.Item2).FirstOrDefault();
            return shift;
        }
    }
}
