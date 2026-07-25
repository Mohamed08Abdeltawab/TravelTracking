using System;
using System.Data;
using TravelAgencyDataAccess;

namespace TravelAgencyBusiness
{
    public class clsVisaType
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public string Name { get; set; }

        // 1. Default Constructor (للـ Add New)
        public clsVisaType()
        {
            this.ID = -1;
            this.Name = "";
            Mode = enMode.AddNew;
        }

        // 2. Private Parameterized Constructor (للـ Find / Read)
        private clsVisaType(int id, string name)
        {
            this.ID = id;
            this.Name = name;
            Mode = enMode.Update;
        }

        // 3. Private Methods للتعامل مع الـ DAL
        private bool _AddNewVisaType()
        {
            this.ID = clsVisaTypesData.AddNewVisaType(this.Name);
            return (this.ID != -1);
        }

        private bool _UpdateVisaType()
        {
            return clsVisaTypesData.UpdateVisaType(this.ID, this.Name);
        }

        // 4. Static Methods لاسترجاع البيانات
        public static clsVisaType Find(int id)
        {
            string name = "";

            if (clsVisaTypesData.GetVisaTypeByID(id, ref name))
            {
                return new clsVisaType(id, name);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetAllVisaTypes()
        {
            return clsVisaTypesData.GetAllVisaTypes();
        }

        public static bool DeleteVisaType(int id)
        {
            return clsVisaTypesData.DeleteVisaType(id);
        }

        // 5. Save Method (تحدد تلقائياً هل العملية إضافة أم تعديل)
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewVisaType())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateVisaType();
            }

            return false;
        }
    }
}