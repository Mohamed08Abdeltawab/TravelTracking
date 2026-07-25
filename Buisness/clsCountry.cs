using System;
using System.Data;
using TravelAgencyDataAccess;

namespace TravelAgencyBusiness
{
    public class clsCountry
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public clsCountry()
        {
            this.ID = -1;
            this.Name = "";
        }

        private clsCountry(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

        public static clsCountry Find(int id)
        {
            string name = "";

            if (clsCountryData.GetCountryByID(id, ref name))
            {
                return new clsCountry(id, name);
            }
            else
            {
                return null;
            }
        }

        public static clsCountry Find(string name)
        {
            int id = -1;

            if (clsCountryData.GetCountryByName(name, ref id))
            {
                return new clsCountry(id, name);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetAllCountries()
        {
            return clsCountryData.GetAllCountries();
        }
    }
}