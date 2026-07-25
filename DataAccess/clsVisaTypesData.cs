using System;
using System.Data;
using System.Data.SQLite;

namespace TravelAgencyDataAccess
{
    public static class clsVisaTypesData
    {
        // مسار قاعدة البيانات - يمكنك تعديله حسب موقع الملف لديك
        private static string connectionString = "Data Source=TravelClients.db;Version=3;";

        /// <summary>
        /// جلب جميع انواع التأشيرات لملء الـ ComboBox أو القوائم
        /// </summary>
        public static DataTable GetAllVisaTypes()
        {
            DataTable dt = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "SELECT Id, Name FROM VisaTypes ORDER BY Name ASC;";

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
                        // يمكنك تسجيل الأخطاء هنا عند الحاجة
                        dt = null;
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// البحث عن نوع تأشيرة باستخدام الـ ID
        /// </summary>
        public static bool GetVisaTypeByID(int id, ref string visaTypeName)
        {
            bool isFound = false;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "SELECT Name FROM VisaTypes WHERE Id = @Id;";

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
                                visaTypeName = reader["Name"].ToString();
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
        /// إضافة نوع تأشيرة جديد وإرجاع الـ ID الخاص به
        /// </summary>
        public static int AddNewVisaType(string name)
        {
            int newId = -1;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                // في SQLite بنستخدم last_insert_rowid() لجلب الـ ID الجديد
                string query = @"INSERT INTO VisaTypes (Name) VALUES (@Name);
                                SELECT last_insert_rowid();";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            newId = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        // في حالة وجود خطأ (مثل تكرار الاسم Unique Constraint)
                        newId = -1;
                    }
                }
            }

            return newId;
        }

        /// <summary>
        /// تعديل اسم نوع التأشيرة
        /// </summary>
        public static bool UpdateVisaType(int id, string name)
        {
            int rowsAffected = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "UPDATE VisaTypes SET Name = @Name WHERE Id = @Id;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);

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
        /// حذف نوع تأشيرة
        /// </summary>
        public static bool DeleteVisaType(int id)
        {
            int rowsAffected = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "DELETE FROM VisaTypes WHERE Id = @Id;";

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