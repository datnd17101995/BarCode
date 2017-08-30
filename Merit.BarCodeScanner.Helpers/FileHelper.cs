using System;
using System.Globalization;
using System.IO;
using Merit.BarCodeScanner.Logging;
using Merit.BarCodeScanner.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Merit.BarCodeScanner.Helpers
{
    public class FileHelper
    {
        #region 

        #endregion

        #region Read csvLine

        /// <summary>
        /// Convert data form CSV to list Object
        /// </summary>
        /// <param name="csvLine"></param>
        /// <returns></returns>
        public static CsvValues FromCsv(string csvLine, int row)
        {
            CsvValues dailyValues = null;

            string[] values = csvLine.Split(','); // delimiter CSV

            dailyValues = new CsvValues
            {
                Id = Guid.NewGuid(),
                Line = row + 1
            };

            try
            {
                if (!string.IsNullOrEmpty(csvLine) && !csvLine.Contains("#")) { }
                {
                    if (string.IsNullOrEmpty(csvLine))
                    {
                        dailyValues.RowType = Contains.ItemStatus.BLANK.ToString();
                        return dailyValues;
                    }
                    if (values.Count() > 0 && values.Count() != 4 && !csvLine.Contains("#"))
                    {
                        dailyValues.RowType = Contains.ItemStatus.FOMATERROR.ToString();
                        return dailyValues;
                    }

                    var date = values[0].TrimStart('"');
                    var time = values[1];
                    var code = values[3].TrimEnd('"');
                    var prefix = values[2];

                    dailyValues.RowType = Contains.RowType.UNDEFINED.ToString();

                    if (string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(time))
                    {
                        dailyValues.RowType = Contains.RowType.EMPLOYEE.ToString();
                        dailyValues.IsBarCode = true;
                    }

                    if (!string.IsNullOrEmpty(code) && IsEmployeeBarCode(code))
                    {
                        dailyValues.RowType = Contains.RowType.EMPLOYEE.ToString();
                        dailyValues.IsBarCode = true;
                    }

                    if (!string.IsNullOrEmpty(code) && IsDestinationBarCode(code))
                    {
                        dailyValues.RowType = Contains.RowType.DESTINATION.ToString();
                        dailyValues.IsBarCode = true;
                    }

                    if (!string.IsNullOrEmpty(code) && IsPalletBarCode(code))
                    {
                        dailyValues.RowType = Contains.RowType.PALLET.ToString();
                        dailyValues.IsBarCode = true;
                    }

                    dailyValues.Id = Guid.NewGuid();
                    dailyValues.Date = date;
                    dailyValues.Time = time;
                    dailyValues.BarCode = code;
                    dailyValues.Prefix = prefix;
                    dailyValues.Commas = values.Count();
                }
            }
            catch (Exception ex)
            {
                if (values.Count() > 0 && values.Count() != 4 && !csvLine.Contains("#"))
                {
                    dailyValues.RowType = Contains.ItemStatus.FOMATERROR.ToString();
                } 
                else if (csvLine.Contains("#"))
                {
                    dailyValues.RowType = Contains.RowType.ENDBLOCK.ToString();
                }
                else
                {
                    dailyValues.RowType = Contains.RowType.EXCEPTION.ToString();
                }
            }
            return dailyValues;
        }


        public static CsvValues FromCsv(string csvLine)
        {
            CsvValues dailyValues = null;

            string[] values = csvLine.Split(','); // delimiter CSV

            dailyValues = new CsvValues
            {
                Id = Guid.NewGuid()
            };

            try
            {
                if (!string.IsNullOrEmpty(csvLine) && !csvLine.Contains("#")) { }
                {
                    var date = values[0].TrimStart('"');
                    var time = values[1];
                    var code = values[3].TrimEnd('"');
                    var prefix = values[2];

                    dailyValues.RowType = Contains.RowType.UNDEFINED.ToString();

                    if (string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(time))
                    {
                        dailyValues.RowType = Contains.RowType.EMPLOYEE.ToString();
                        dailyValues.IsBarCode = true;
                    }

                    if (!string.IsNullOrEmpty(code) && IsEmployeeBarCode(code))
                    {
                        dailyValues.RowType = Contains.RowType.EMPLOYEE.ToString();
                        dailyValues.IsBarCode = true;
                    }

                    if (!string.IsNullOrEmpty(code) && IsDestinationBarCode(code))
                    {
                        dailyValues.RowType = Contains.RowType.DESTINATION.ToString();
                        dailyValues.IsBarCode = true;
                    }

                    if (!string.IsNullOrEmpty(code) && IsPalletBarCode(code))
                    {
                        dailyValues.RowType = Contains.RowType.PALLET.ToString();
                        dailyValues.IsBarCode = true;
                    }

                    dailyValues.Id = Guid.NewGuid();
                    dailyValues.Date = date;
                    dailyValues.Time = time;
                    dailyValues.BarCode = code;
                    dailyValues.Prefix = prefix;
                    dailyValues.Commas = values.Count();
                }
            }
            catch (Exception ex)
            {
                if (csvLine.Contains("#"))
                {
                    dailyValues.RowType = Contains.RowType.ENDBLOCK.ToString();
                }
                else
                {
                    dailyValues.RowType = Contains.RowType.EXCEPTION.ToString();
                }

                if (string.IsNullOrEmpty(csvLine))
                {
                    dailyValues.RowType = Contains.ItemStatus.BLANK.ToString();
                }
            }
            return dailyValues;
        }
        #endregion

        #region Validate file name and file exsit
        public static Contains.FileStatus CheckFile(ImportRequest request)
        {

            if (string.IsNullOrEmpty(request.PathFolder) || string.IsNullOrEmpty(request.FileName))
            {
                return Contains.FileStatus.NOTEXSIT;
            }

            if (!File.Exists(request.PathFile)) return Contains.FileStatus.NOTEXSIT;

            var ext = Path.GetExtension(request.PathFile);

            return !".csv".Equals(ext) ? Contains.FileStatus.NOTEXTENSION : Contains.FileStatus.EXTENSION;
        }

        #endregion

        #region Copy File and rename

        public static Contains.FileStatus CopyFile(ImportRequest request)
        {
            var pathFile = request.PathFile;
            //var version = 1;
            if (string.IsNullOrEmpty(request.PathFolder) || string.IsNullOrEmpty(request.FileName) || !File.Exists(pathFile))
            {
                return Contains.FileStatus.NOTEXSIT;
            }

            try
            {
                var date = DateTime.Now;
                if (request.FileNameNew.ToString() != "")
                {
                    string newFile = string.Format("{0}\\{1}_{2:yyyyMMdd_HHmmss}.csv", request.FileNameNew, request.FileName.Replace(".csv", ""), date);
                    File.Copy(pathFile, newFile);
                }
                else
                {
                    string newFile = string.Format("{0}\\{1}_{2:yyyyMMdd_HHmmss}.csv", request.PathFile, request.FileName.Replace(".csv", ""), date);
                    File.Copy(pathFile, newFile);
                }
                
                File.Delete(pathFile);
                return Contains.FileStatus.SUCCESSFULL;
            }
            catch (Exception)
            {
                return Contains.FileStatus.UNSUCCESSFULL;
            }
        }

        #endregion

        private static ILogger _logService = new FileLogManager(typeof(FileHelper));

        public static DateTime? CvStringToDate(string date, string barCode, int rowNum)
        {
            try
            {
                DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
                DateTimeFormatInfo ukDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                string result = Convert.ToDateTime(date, usDtfi).ToString(ukDtfi.LongTimePattern);

                var dt = DateTime.Parse(result, new CultureInfo("en-US", true));
                return dt;
            }
            catch (Exception)
            {
                _logService.LogError("Cannot convert to datetime: " + date + " Line " + rowNum);
                return null;
            }
        }

        public static DateTime? CvStringToDate(string date)
        {
            try
            {
                DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
                DateTimeFormatInfo ukDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                string result = Convert.ToDateTime(date, usDtfi).ToString(ukDtfi.FullDateTimePattern);

                var dt = DateTime.Parse(result, new CultureInfo("en-US", true));
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Check Bar Code

        /// <summary>
        /// Lenght of Employee bar code is 5
        /// </summary>
        private static int EMPLOYEE_BAR_CODE = 5;

        /// <summary>
        /// Lenght of pallet bar code is 10
        /// </summary>
        private static int PALLET_BAR_CODE = 10;

        /// <summary>
        /// Lenght of destiantion bar code is 8
        /// </summary>
        private static int DESTINATION_BAR_CODE = 8;


        private static bool IsEmployeeBarCode(string code)
        {
            return !string.IsNullOrEmpty(code) && code.Length == EMPLOYEE_BAR_CODE && (code.StartsWith("5") || code.StartsWith("7") || code.StartsWith("6"));
        }

        private static bool IsPalletBarCode(string code)
        {
            return !string.IsNullOrEmpty(code) && code.Length == PALLET_BAR_CODE;
        }

        private static bool IsDestinationBarCode(string code)
        {
            return !string.IsNullOrEmpty(code) && code.Length == DESTINATION_BAR_CODE;
        }

        #endregion

    }
}
