using System;
using System.Data;
using System.Data.SQLite;

namespace TravelAgencyDataAccess
{
    public static class clsClientData
    {
        private static string connectionString = "Data Source=TravelClients.db;Version=3;";

        /// <summary>
        /// جلب جميع العملاء مع أسماء الدول وأنواع التأشيرات للعرض في الـ DataGrid
        /// </summary>
        public static DataTable GetAllClients()
        {
            DataTable dt = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        c.Id,
                        c.FullName,
                        c.PassportNumber,
                        co.Name AS CountryName,
                        c.CountryId,
                        c.Email,
                        c.Password,
                        c.PhoneNumber,
                        v.Name AS VisaTypeName,
                        c.VisaTypeId,
                        c.ImagePath,
                        c.CreatedAt,
                        c.UpdatedAt
                    FROM Clients c
                    LEFT JOIN Countries co ON c.CountryId = co.Id
                    LEFT JOIN VisaTypes v ON c.VisaTypeId = v.Id
                    ORDER BY c.Id DESC;";

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

        /// <summary>
        /// البحث عن عميل بواسطة الـ ID
        /// </summary>
        public static bool GetClientByID(
            int id,
            ref string fullName,
            ref string passportNumber,
            ref int countryId,
            ref string email,
            ref string password,
            ref string phoneNumber,
            ref int visaTypeId,
            ref string imagePath,
            ref DateTime createdAt,
            ref DateTime updatedAt)
        {
            bool isFound = false;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "SELECT * FROM Clients WHERE Id = @Id;";

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

                                fullName = Convert.ToString(reader["FullName"]);
                                passportNumber = Convert.ToString(reader["PassportNumber"]);
                                countryId = reader["CountryId"] != DBNull.Value ? Convert.ToInt32(reader["CountryId"]) : -1;
                                email = reader["Email"] != DBNull.Value ? Convert.ToString(reader["Email"]) : "";
                                password = reader["Password"] != DBNull.Value ? Convert.ToString(reader["Password"]) : "";
                                phoneNumber = reader["PhoneNumber"] != DBNull.Value ? Convert.ToString(reader["PhoneNumber"]) : "";
                                visaTypeId = reader["VisaTypeId"] != DBNull.Value ? Convert.ToInt32(reader["VisaTypeId"]) : -1;
                                imagePath = reader["ImagePath"] != DBNull.Value ? Convert.ToString(reader["ImagePath"]) : "";
                                createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                                updatedAt = Convert.ToDateTime(reader["UpdatedAt"]);
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

        /// <summary>
        /// إضافة عميل جديد وإرجاع الـ ID
        /// </summary>
        public static int AddNewClient(
            string fullName,
            string passportNumber,
            int countryId,
            string email,
            string password,
            string phoneNumber,
            int visaTypeId,
            string imagePath)
        {
            int newId = -1;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = @"
                    INSERT INTO Clients 
                    (FullName, PassportNumber, CountryId, Email, Password, PhoneNumber, VisaTypeId, ImagePath, CreatedAt, UpdatedAt)
                    VALUES 
                    (@FullName, @PassportNumber, @CountryId, @Email, @Password, @PhoneNumber, @VisaTypeId, @ImagePath, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
                    SELECT last_insert_rowid();";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", fullName);
                    command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                    command.Parameters.AddWithValue("@CountryId", countryId > 0 ? (object)countryId : DBNull.Value);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                    command.Parameters.AddWithValue("@Password", string.IsNullOrEmpty(password) ? (object)DBNull.Value : password);
                    command.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrEmpty(phoneNumber) ? (object)DBNull.Value : phoneNumber);
                    command.Parameters.AddWithValue("@VisaTypeId", visaTypeId > 0 ? (object)visaTypeId : DBNull.Value);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(imagePath) ? (object)DBNull.Value : imagePath);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            newId = Convert.ToInt32(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        newId = -1;
                    }
                }
            }

            return newId;
        }

        /// <summary>
        /// تعديل بيانات عميل قائم مع تحديث تاريخ الـ UpdatedAt
        /// </summary>
        public static bool UpdateClient(
            int id,
            string fullName,
            string passportNumber,
            int countryId,
            string email,
            string password,
            string phoneNumber,
            int visaTypeId,
            string imagePath)
        {
            int rowsAffected = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = @"
                    UPDATE Clients 
                    SET FullName = @FullName,
                        PassportNumber = @PassportNumber,
                        CountryId = @CountryId,
                        Email = @Email,
                        Password = @Password,
                        PhoneNumber = @PhoneNumber,
                        VisaTypeId = @VisaTypeId,
                        ImagePath = @ImagePath,
                        UpdatedAt = CURRENT_TIMESTAMP
                    WHERE Id = @Id;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@FullName", fullName);
                    command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                    command.Parameters.AddWithValue("@CountryId", countryId > 0 ? (object)countryId : DBNull.Value);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                    command.Parameters.AddWithValue("@Password", string.IsNullOrEmpty(password) ? (object)DBNull.Value : password);
                    command.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrEmpty(phoneNumber) ? (object)DBNull.Value : phoneNumber);
                    command.Parameters.AddWithValue("@VisaTypeId", visaTypeId > 0 ? (object)visaTypeId : DBNull.Value);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(imagePath) ? (object)DBNull.Value : imagePath);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }

        /// <summary>
        /// حذف عميل بواسطة الـ ID
        /// </summary>
        public static bool DeleteClient(int id)
        {
            int rowsAffected = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "DELETE FROM Clients WHERE Id = @Id;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }
    }
}