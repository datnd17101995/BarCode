using Merit.BarCodeScanner.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merit.BarCodeScanner.Services
{
    public class ShiftServices
    {
        //string connectString = ConfigurationManager.ConnectionStrings["MeritConnectionString"].ToString();
        string connectString = "";
        public ShiftServices()
        {
            ConnectionStringSettings mySetting = ConfigurationManager.ConnectionStrings["MeritConnectionString"];
            if (mySetting == null || string.IsNullOrEmpty(mySetting.ConnectionString))
                throw new Exception("Fatal error: missing connecting string in web.config file");
            connectString = mySetting.ConnectionString;
        }
        public List<LocationShift> GetLocationShift(string locationCode)
        {
            List<LocationShift> LocationsShift = new List<LocationShift>();
            using (SqlConnection con = new SqlConnection(connectString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("select l.Id as LocationId,ls.Id as ShiftId from dbo.LocationShift ls join dbo.Location l on ls.LocationId = l.Id where l.LocationCode = '"+ locationCode + "' ", con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        foreach (DataRow obj in dataTable.Rows)
                        {
                            LocationsShift.Add(new LocationShift
                            {
                                LocationId = int.Parse(obj["LocationId"].ToString()),
                                ShiftId = int.Parse(obj["ShiftId"].ToString())
                            });
                        }
                        return LocationsShift;
                    }
                }
            }

        }
    }
}
