using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class clsDataAccessSettings
    {
        public static string ConnectionString = $@"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TravelTracking.db")};Version=3;";
    }
}
