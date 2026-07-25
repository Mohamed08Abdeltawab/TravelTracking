using System;
using System.Data;
using System.Data.SQLite;

namespace TravelAgencyDataAccess
{
    public static class clsCountryData
    {
        private static string connectionString = "Data Source=TravelClients.db;Version=3;";

        public static DataTable GetAllCountries()
        {
            DataTable dt = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "SELECT Id, Name FROM Countries ORDER BY Name ASC;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        dt = null;
                    }
                }
            }

            return dt;
        }

        public static bool GetCountryByID(int id, ref string countryName)
        {
            bool isFound = false;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "SELECT Name FROM Countries WHERE Id = @Id;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        connection.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                countryName = reader["Name"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }

        public static bool GetCountryByName(string countryName, ref int id)
        {
            bool isFound = false;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "SELECT Id FROM Countries WHERE Name = @Name;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", countryName);

                    try
                    {
                        connection.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                id = Convert.ToInt32(reader["Id"]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }
    }
}