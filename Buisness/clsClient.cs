using System;
using System.Data;
using TravelAgencyDataAccess;

namespace TravelAgencyBusiness
{
    public class clsClient
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public int CountryID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public int VisaTypeID { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public clsCountry CountryInfo { get; set; }
        public clsVisaType VisaTypeInfo { get; set; }

        public clsClient()
        {
            this.ID = -1;
            this.FullName = "";
            this.PassportNumber = "";
            this.CountryID = -1;
            this.Email = "";
            this.Password = "";
            this.PhoneNumber = "";
            this.Address = "";
            this.Notes = "";
            this.VisaTypeID = -1;
            this.ImagePath = "";
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;

            Mode = enMode.AddNew;
        }

        private clsClient(
            int id,
            string fullName,
            string passportNumber,
            int countryId,
            string email,
            string password,
            string phoneNumber,
            string address,
            string notes,
            int visaTypeId,
            string imagePath,
            DateTime createdAt,
            DateTime updatedAt)
        {
            this.ID = id;
            this.FullName = fullName;
            this.PassportNumber = passportNumber;
            this.CountryID = countryId;
            this.Email = email;
            this.Password = password;
            this.PhoneNumber = phoneNumber;
            this.Address = address;
            this.Notes = notes;
            this.VisaTypeID = visaTypeId;
            this.ImagePath = imagePath;
            this.CreatedAt = createdAt;
            this.UpdatedAt = updatedAt;

            this.CountryInfo = clsCountry.Find(countryId);
            this.VisaTypeInfo = clsVisaType.Find(visaTypeId);

            Mode = enMode.Update;
        }

        private bool _AddNewClient()
        {
            this.ID = clsClientData.AddNewClient(
                this.FullName,
                this.PassportNumber,
                this.CountryID,
                this.Email,
                this.Password,
                this.PhoneNumber,
                this.Address,
                this.Notes,
                this.VisaTypeID,
                this.ImagePath
            );

            return (this.ID != -1);
        }

        private bool _UpdateClient()
        {
            return clsClientData.UpdateClient(
                this.ID,
                this.FullName,
                this.PassportNumber,
                this.CountryID,
                this.Email,
                this.Password,
                this.PhoneNumber,
                this.Address,
                this.Notes,
                this.VisaTypeID,
                this.ImagePath
            );
        }

        public static clsClient Find(int id)
        {
            string fullName = "", passportNumber = "", email = "", password = "", phoneNumber = "", address = "", notes = "", imagePath = "";
            int countryId = -1, visaTypeId = -1;
            DateTime createdAt = DateTime.Now, updatedAt = DateTime.Now;

            if (clsClientData.GetClientByID(
                id,
                ref fullName,
                ref passportNumber,
                ref countryId,
                ref email,
                ref password,
                ref phoneNumber,
                ref address,
                ref notes,
                ref visaTypeId,
                ref imagePath,
                ref createdAt,
                ref updatedAt))
            {
                return new clsClient(
                    id,
                    fullName,
                    passportNumber,
                    countryId,
                    email,
                    password,
                    phoneNumber,
                    address,
                    notes,
                    visaTypeId,
                    imagePath,
                    createdAt,
                    updatedAt);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetAllClients()
        {
            return clsClientData.GetAllClients();
        }

        public static bool DeleteClient(int id)
        {
            return clsClientData.DeleteClient(id);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewClient())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateClient();
            }

            return false;
        }
    }
}