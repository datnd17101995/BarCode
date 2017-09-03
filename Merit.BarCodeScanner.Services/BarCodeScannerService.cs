using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using Merit.BarCodeScanner.Data;
using Merit.BarCodeScanner.Helpers;
using Merit.BarCodeScanner.Logging;
using Merit.BarCodeScanner.Models;

namespace Merit.BarCodeScanner.Services
{
    public class BarCodeScannerService : IBarCodeScannerService
    {
        private ILogger _logService = new FileLogManager(typeof(BarCodeScannerService));
        private readonly ShiftServices _shiftServices = new ShiftServices();

        public ResultRespose Import(ImportRequest request, AppSettingsSection _appSettings)
        {
            List<CsvValues> dailyScanValues = new List<CsvValues>();
            List<EmpWorking> empWorkings = new List<EmpWorking>();
            List<DeliveryBlock> deliveryBlocks = new List<DeliveryBlock>();
            List<Block> blocks = new List<Block>();
            List<PalletDetail> palletDetails = new List<PalletDetail>();
            List<Destination> destinations = new List<Destination>();
            EmpWorking empl = null;
            //Block block = null;
            PalletDetail pallet = null;
            //DeliveryBlock deliveryBlock = null;
            Destination destination = null;
            int row = 1;
            var endBlock = 0;
            int totalLog = 0;
            var startScaner = DateTime.Now;

            var redAllLine = File.ReadAllLines(request.PathFile).ToList();
            dailyScanValues = redAllLine.Skip(0)
                              .Select(line => FileHelper.FromCsv(line, redAllLine.FindIndex(x => x.Equals(line))))
                              .ToList();
            _logService.LogInfo("Start: " + startScaner.ToString());
            CsvValues lineBlank = FileHelper.FromCsv("");
            //insert 10 line blank.end file
            for (int i = 0; i < 10; i++)
            {
                dailyScanValues.Add(lineBlank);
            }
            foreach (var item in dailyScanValues.ToList())
            {
                var index = dailyScanValues.FindIndex(a => a.Id == item.Id);
                var prevIndex = index - 1;
                var nextIndex = index + 1;

                #region check item error
                //Special character row
                if (item.RowType.Equals(Contains.RowType.ENDBLOCK.ToString()))
                {
                    try
                    {
                        var nextItem = dailyScanValues[nextIndex];

                        if (nextItem.RowType.Equals(Contains.RowType.ENDBLOCK.ToString()) || nextItem.RowType.Equals(Contains.RowType.EXCEPTION.ToString()))
                        {
                            endBlock++;

                            if (endBlock > 1)
                            {

                                _logService.LogError("Special character row " + nextItem.BarCode + ", Line: " + row);
                                totalLog++;
                                return new ResultRespose
                                {
                                    Status = false,
                                    Message = "Special character row " + nextItem.BarCode + ", Line: " + row,
                                    StartDate = startScaner
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.LogError(ex.ToString());
                    }
                }
                //over scan if 10 black row consecutive
                if (item.RowType.Equals(Contains.RowType.BLANK.ToString()))
                {
                    var nextItem = dailyScanValues[nextIndex];
                    if (nextItem.RowType.Equals(Contains.RowType.BLANK.ToString()))
                    {
                        try
                        {
                            var nextRow = row + 1;
                            var blankRow = 0;
                            blankRow++;
                            for (int i = 1; i <= 8; i++)
                            {
                                if (dailyScanValues[nextRow].RowType.Equals(Contains.RowType.BLANK.ToString()))
                                {
                                    blankRow++;
                                    nextRow++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (blankRow >= 9)
                            {
                                break;
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
                //Lacking date info
                if (item.IsBarCode && string.IsNullOrEmpty(item.Date))
                {
                    _logService.LogError("Lacking date Line: " + row);
                    totalLog++;
                    return new ResultRespose
                    {
                        Status = false,
                        Message = "Lacking date Line: " + row,
                        StartDate = startScaner
                    };
                }

                //Lacking time info
                if (item.IsBarCode && string.IsNullOrEmpty(item.Time))
                {
                    _logService.LogError("Lacking time Line: " + row);
                    totalLog++;
                    return new ResultRespose
                    {
                        Status = false,
                        Message = "Lacking time Line: " + row,
                        StartDate = startScaner
                    };
                }

                //Lacking prefix info
                if (item.IsBarCode && string.IsNullOrEmpty(item.Prefix))
                {
                    _logService.LogError("Lacking perfix Line: " + row);
                    totalLog++;
                    return new ResultRespose
                    {
                        Status = false,
                        Message = "Lacking perfix Line: " + row,
                        StartDate = startScaner
                    };
                }
                //Redundant code info
                if (item.RowType.Equals(Contains.RowType.FOMATERROR.ToString()))
                {
                    _logService.LogError("Raw data file has wrong content format Line: " + row);
                    totalLog++;
                    return new ResultRespose
                    {
                        Status = false,
                        Message = "Raw data file has wrong content format Line: " + row,
                        StartDate = startScaner
                    };
                }

                //Datetime Format is not correct
                if (item.IsBarCode
                    && !string.IsNullOrEmpty(item.Date)
                    && !string.IsNullOrEmpty(item.Time)
                     && FileHelper.CvStringToDate(item.DateTime) == null)
                {
                    _logService.LogError("Datetime Format is not correct " + item.BarCode + ", Line: " + row);
                    totalLog++;
                    return new ResultRespose
                    {
                        Status = false,
                        Message = "Datetime Format is not correct " + item.BarCode + ", Line: " + row,
                        StartDate = startScaner
                    };
                }

                //Lacking Barcode
                if (item.IsBarCode
                    && string.IsNullOrEmpty(item.BarCode))
                {
                    _logService.LogError("Lacking Barcode " + item.BarCode + ", Line: " + row);
                    totalLog++;
                    return new ResultRespose
                    {
                        Status = false,
                        Message = "Lacking Barcode " + item.BarCode + ", Line: " + row,
                        StartDate = startScaner
                    };
                }

                //Must have Emplyee ID at the first row of each scanner dataset
                if (row == 1 && item.RowType.Equals(Contains.RowType.PALLET.ToString()))
                {
                    _logService.LogError("Lacking Employee barcode " + item.BarCode + ", Line: " + row);
                    totalLog++;
                    return new ResultRespose
                    {
                        Status = false,
                        Message = "Lacking Employee barcode " + item.BarCode + ", Line: " + row,
                        StartDate = startScaner
                    };
                }
                #endregion

                #region show log wanning file
                //check destination firt file
                if (empWorkings.Count() == 0)
                {
                    if (item.RowType.Equals(Contains.RowType.DESTINATION.ToString()))
                    {
                        dailyScanValues.Remove(item);
                        _logService.LogWarn("Redundant Destination barcode " + item.BarCode + ", Line: " + row);
                        totalLog++;
                    }
                }
                if (item.IsBarCode && row > 0 &&
                    item.RowType.Equals(Contains.RowType.DESTINATION.ToString()))
                {

                    try
                    {
                        var nextItem = dailyScanValues[nextIndex];
                        var prevItem = dailyScanValues[prevIndex];
                        //Destination barcodes are displayed at the beginning of Scanner dataset                    
                        if (prevItem.RowType.Equals(Contains.RowType.ENDBLOCK.ToString()))
                        {
                            dailyScanValues.Remove(item);
                            _logService.LogWarn("Redundant Destination barcode " + item.BarCode + ", Line: " + row);
                            totalLog++;
                        }
                        else if (nextItem.RowType.Equals(Contains.RowType.DESTINATION.ToString())
                                || prevItem.RowType.Equals(Contains.RowType.EMPLOYEE.ToString()))
                        {
                            dailyScanValues.Remove(item);
                            _logService.LogWarn("Redundant Destination barcode " + item.BarCode + ", Line: " + row);
                            totalLog++;
                        }
                        else
                        {
                            destination = new Destination
                            {
                                BlockId = palletDetails.Last().BlockId,
                                DestinationId = item.BarCode,
                                WorkTime = FileHelper.CvStringToDate(item.DateTime)
                            };

                            destinations.Add(destination);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logService.LogError(ex.ToString());
                    }
                }

                //Format is not correct
                if (item.RowType.Equals(Contains.RowType.UNDEFINED.ToString()))
                {
                    dailyScanValues.Remove(item);
                    _logService.LogWarn("EEID format is not correct " + item.BarCode + ", Line: " + row);
                    totalLog++;
                }

                //Multi Employee barcodes are displayed continuously in multi rows
                if (item.IsBarCode
                    && !string.IsNullOrEmpty(item.BarCode)
                    && item.RowType.Equals(Contains.RowType.EMPLOYEE.ToString()))
                {
                    //Lack Pallet barcode in working block (Pallet barcode-> Employee -> Destination)
                    try
                    {
                        var nextItem = dailyScanValues[nextIndex];
                        var prevItem = dailyScanValues[prevIndex];

                        if (prevItem.RowType.Equals(Contains.RowType.PALLET.ToString())
                            && nextItem.RowType.Equals(Contains.RowType.DESTINATION.ToString()))
                        {
                            dailyScanValues.Remove(item);
                            _logService.LogWarn("Redundant Employee barcode " + item.BarCode + ", Line: " + row);
                            totalLog++;
                        }
                    }
                    catch (Exception) { }

                    try
                    {
                        var nextItem = dailyScanValues[nextIndex];

                        //Employee barcode is displayed at the end of Scanner dataset
                        if (nextItem.RowType.Equals(Contains.RowType.ENDBLOCK.ToString()))
                        {
                            dailyScanValues.Remove(item);
                            _logService.LogWarn("Redundant Employee barcode " + item.BarCode + ", Line: " + row);
                            totalLog++;
                        }
                    }
                    catch (Exception ex) { _logService.LogError(ex.ToString()); }

                    if (empWorkings.Any())
                    {
                        if (dailyScanValues[prevIndex].IsBarCode
                            && dailyScanValues[prevIndex].RowType.Equals(Contains.RowType.EMPLOYEE.ToString()))
                        {
                            var prevItem = dailyScanValues[prevIndex];
                            empWorkings.Remove(empWorkings.Last());
                            int prevRow = prevItem.Line;
                            dailyScanValues.Remove(prevItem);
                            _logService.LogWarn("Multi Employee Barcode " + prevItem.BarCode + ", Line: " + prevRow);
                            totalLog++;
                        }
                    }

                    empl = new EmpWorking
                    {
                        Id = Guid.NewGuid(),
                        Day = item.Date,
                        EmployeeId = item.BarCode,
                        WorkTime = FileHelper.CvStringToDate(item.DateTime)
                    };
                    empWorkings.Add(empl);
                }

                //Lack Employee barcode after 1 Destination barcode (have Pallet barcode after Destination barcode)
                if (item.IsBarCode && row > 0
                    && !string.IsNullOrEmpty(item.BarCode)
                    && item.RowType.Equals(Contains.RowType.PALLET.ToString()))
                {
                    index = dailyScanValues.FindIndex(a => a.Id == item.Id) - 1;
                    if (prevIndex < 0)
                    {
                        _logService.LogError("Lacking Employee barcode " + item.BarCode + ", Line: " + row);
                        return new ResultRespose
                        {
                            Status = false,
                            Message = "Lacking Employee barcode " + item.BarCode + ", Line: " + row,
                            StartDate = startScaner
                        };
                    }
                    else
                    {
                        try
                        {
                            var prevItem = dailyScanValues[index];
                            var nextItem = dailyScanValues[nextIndex];
                            if (prevItem.RowType.Equals(Contains.RowType.FOMATERROR.ToString()))
                            {
                                _logService.LogError("Lacking Employee barcode " + item.BarCode + ", Line: " + row);
                                return new ResultRespose
                                {
                                    Status = false,
                                    Message = "Lacking Employee barcode " + item.BarCode + ", Line: " + row,
                                    StartDate = startScaner
                                };
                            }
                            if (prevItem.RowType.Equals(Contains.RowType.DESTINATION.ToString()))
                            {
                                var lastEmp = empWorkings.Last();

                                empl = new EmpWorking
                                {
                                    Id = Guid.NewGuid(),
                                    Day = lastEmp.Day,
                                    EmployeeId = lastEmp.EmployeeId,
                                    WorkTime = FileHelper.CvStringToDate(item.DateTime).Value.AddSeconds(-1)
                                };
                                empWorkings.Add(empl);
                                _logService.LogWarn("Lack Employee barcode before Pallet " + item.BarCode + ", Line: " + row);
                                totalLog++;
                            }
                            //Lacking Employee barcode after Pallet
                            if (nextItem.RowType.Equals(Contains.RowType.EMPLOYEE.ToString()))
                            {
                                _logService.LogWarn("Lacking Destination barcode after Pallet  " + item.BarCode + ", Line: " + row);
                            }
                            //Remove duplicate pallet
                            if (palletDetails.Any(p => p.PalletId == item.BarCode))
                            {
                                var listDuplicate =
                                    palletDetails.FirstOrDefault(x => x.PalletId == item.BarCode);

                                if (listDuplicate != null)
                                {
                                    dailyScanValues.Remove(item);
                                    _logService.LogWarn("Pallet Barcode is duplicated " + item.BarCode + ", Line: " + row);
                                    totalLog++;
                                }
                            }
                            else
                            {
                                //Create new Pallet
                                pallet = new PalletDetail
                                {
                                    BlockId = empWorkings.Last().Id,
                                    Day = empWorkings.Last().Day,
                                    EmployeeId = empWorkings.Last().EmployeeId,
                                    PalletId = item.BarCode,
                                    PalletScanTime = FileHelper.CvStringToDate(item.DateTime)
                                };
                                palletDetails.Add(pallet);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logService.LogError(ex.ToString());
                        }

                    }

                }
                row++;
                #endregion
            }
            //Insert to DB
            var countDelviveBlock = 0;
            try
            {
                _logService.LogInfo("start Insert");
                var locationCode = "GFSPLT";
                
                //List<LocationShift> locationShifts = _shiftServices.GetLocationShift(locationCode);
                using (var dbContext = new barCodeDbContext())
                {
                    _logService.LogInfo("Insert");
                    dbContext.Configuration.AutoDetectChangesEnabled = false;
                    dbContext.Configuration.ValidateOnSaveEnabled = false;
                    List<LocationShift> locationShifts = dbContext.LocationShifts.Where(l=>l.LocationId == 35).ToList();
                    List<Block> lstBlocks = new List<Block>();
                    var oldBlock = new DeliveryBlock();
                    DeliveryBlock barCodeBlock = null;
                    EmpWorking ep = null;
                    Destination des = null;
                    Block blo = null;
                    try
                    {
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
                        foreach (var pl in palletDetails)
                        {
                            barCodeBlock = new DeliveryBlock();
                            ep = empWorkings.Where(e => e.Id == pl.BlockId).FirstOrDefault();
                            if (pl.PalletId == null || (ep == null))
                            {
                                continue;
                            }
                            if (ep != null)
                            {
                                barCodeBlock.EmployeeId = ep.EmployeeId;
                                barCodeBlock.BlockStartTime = FileHelper.CvStringToDate(ep.WorkTime.ToString());
                            }
                            barCodeBlock.PalletId = pl.PalletId.ToString();
                            barCodeBlock.BlockId = pl.BlockId;
                            des = destinations.Where(d => d.BlockId == pl.BlockId).LastOrDefault();
                            if (des != null)
                            {
                                barCodeBlock.BlockEndTime = FileHelper.CvStringToDate(des.WorkTime.ToString() == null ? new DateTime().ToString() : des.WorkTime.ToString());
                                barCodeBlock.DestinationId = des.DestinationId;
                            }
                            oldBlock = dbContext.DeliveryBlocks.Where(b => b.PalletId == barCodeBlock.PalletId && b.BlockStartTime == barCodeBlock.BlockStartTime
                                                                           && (b.BlockEndTime.HasValue ? b.BlockEndTime == barCodeBlock.BlockEndTime : true)).FirstOrDefault();

                            if (oldBlock != null)
                            {
                                _logService.LogInfo(oldBlock.ToString());
                                continue;
                            }
                            blo = lstBlocks.Where(b => b.BlockId == pl.BlockId).FirstOrDefault();
                            if (blo == null)
                            {
                                Block bl = new Block();
                                bl.BlockId = pl.BlockId;
                                bl.CreatedDate = DateTime.Now;
                                var locationShiftsStart = locationShifts.Select(x => new Tuple<double, LocationShift>(GetMedianDiff(x.Start.TimeOfDay, barCodeBlock.BlockStartTime.Value.TimeOfDay), x));
                                var locationShiftsEnd = locationShifts.Select(x => new Tuple<double, LocationShift>(GetMedianDiff(x.End.TimeOfDay, barCodeBlock.BlockStartTime.Value.TimeOfDay), x));
                                var shift = locationShiftsStart.Union(locationShiftsEnd).OrderBy(x => x.Item1).Select(x => x.Item2).FirstOrDefault();
                                if (shift != null)
                                {
                                    BlockShift blockShift = new BlockShift()
                                    {
                                        BlockId = bl.BlockId,
                                        ShiftId = shift.ShiftId
                                    };
                                    dbContext.BlockShifts.Add(blockShift);
                                }
                                lstBlocks.Add(bl);
                            }
                            dbContext.DeliveryBlocks.Add(barCodeBlock);
                            dbContext.PalletDetails.Add(pl);
                        }
                        foreach (var item in lstBlocks)
                        {
                            dbContext.Blocks.Add(item);
                        }
                        dbContext.SaveChanges();
                        countDelviveBlock = lstBlocks.Count();
                    }
                    catch (Exception ex)
                    {
                        _logService.LogInfo(ex.ToString());
                        return new ResultRespose
                        {
                            Status = false,
                            Message = "Can Not Insert To Database",
                            StartDate = startScaner
                        };

                    }

                }
            }
            catch (Exception e)
            {
                _logService.LogError(e.ToString());
            }
            //log success
            var endScaner = DateTime.Now;
            _logService.LogInfo("Status Batch Successful");
            _logService.LogInfo("Total Block Insert:" + countDelviveBlock.ToString());
            _logService.LogInfo("End: " + endScaner.ToString());
            _logService.LogInfo("##############################################");
            return new ResultRespose
            {
                Status = true,
                StartDate = startScaner
            };
        }
    }
}
