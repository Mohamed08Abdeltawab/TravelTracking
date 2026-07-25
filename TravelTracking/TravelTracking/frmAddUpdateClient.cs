using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravelAgencyBusiness;
using TravelTracking.Global_Classes;
using TravelTracking.Properties;

namespace TravelTracking
{
    public partial class frmAddUpdateClient : Form
    {
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode;

        private int _ClientID = -1;
        private clsClient _Client;

        public frmAddUpdateClient(int ClientID)
        {
            InitializeComponent();
            _ClientID = ClientID;
            _Mode = enMode.Update;
        }

        public frmAddUpdateClient()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        private void _FillCountriesComboBox()
        {
            DataTable dtCountries = clsCountry.GetAllCountries();

            if (dtCountries != null && dtCountries.Rows.Count > 0)
            {
                cbCountries.DisplayMember = "Name";
                cbCountries.ValueMember = "Id";
                cbCountries.DataSource = dtCountries;
                cbCountries.SelectedIndex = -1; // علشان يفضل فاضي لحد ما المستخدم يختار
            }
        }

        private void _FillVisaTypesComboBox()
        {
            DataTable dtVisaTypes = clsVisaType.GetAllVisaTypes();

            if (dtVisaTypes != null && dtVisaTypes.Rows.Count > 0)
            {
                cbVisaTypes.DisplayMember = "Name";
                cbVisaTypes.ValueMember = "Id";
                cbVisaTypes.DataSource = dtVisaTypes;
                cbVisaTypes.SelectedIndex = -1;
            }
        }

        private void _ResetDefaultValues()
        {
            // تعبئة القوائم المنسدلة أولاً
            _FillCountriesComboBox();
            _FillVisaTypesComboBox();

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "إضافة عميل جديد";
                _Client = new clsClient();
                lblCreatedAt.Text = DateTime.Now.ToString("dd/MM/yyyy");
                lblUpdatedAt.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            else
            {
                lblTitle.Text = "تعديل بيانات عميل";
            }

            lblClientID.Text = "???";
            txtFullName.Text = "";
            txtPassportNumber.Text = "";
            txtEmail.Text = "";
            txtPassword.Text = "";
            txtPhoneNumber.Text = "";
            txtAddress.Text = "";
            txtNotes.Text = "";
            cbVisaTypes.SelectedIndex = cbVisaTypes.Items.Count > 2 ? 2 : -1;
            pbClientImage.Image = Resources.users_512;
            pbClientImage.ImageLocation = null;
            llRemoveImage.Visible = false;
            btnSave.Enabled = true;
        }

        private void _LoadData()
        {
            _Client = clsClient.Find(_ClientID);

            if (_Client == null)
            {
                MessageBox.Show("لا يوجد عميل يحمل الرقم التعريفى = " + _ClientID, "العميل غير موجود", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            lblClientID.Text = _Client.ID.ToString();
            txtFullName.Text = _Client.FullName;
            txtPassportNumber.Text = _Client.PassportNumber;
            txtEmail.Text = _Client.Email;
            txtPassword.Text = _Client.Password;
            txtPhoneNumber.Text = _Client.PhoneNumber;
            txtAddress.Text = _Client.Address;
            txtNotes.Text = _Client.Notes;

            if (_Client.CountryID != -1)
                cbCountries.SelectedValue = _Client.CountryID;

            if (_Client.VisaTypeID != -1)
                cbVisaTypes.SelectedValue = _Client.VisaTypeID;

            lblCreatedAt.Text = _Client.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            lblUpdatedAt.Text = _Client.UpdatedAt.ToString("dd/MM/yyyy HH:mm");

            if (!string.IsNullOrEmpty(_Client.ImagePath) && File.Exists(_Client.ImagePath))
            {
                pbClientImage.ImageLocation = _Client.ImagePath;
                llRemoveImage.Visible = true;
            }
            else
            {
                pbClientImage.Image = Resources.users_512;
                llRemoveImage.Visible = false;
            }
        }

        private void frmAddUpdateClient_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (_Mode == enMode.Update)
                _LoadData();
        }

        // ================= Validations =================

        private void txtFullName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFullName, "اسم العميل لا يمكن أن يكون فارغاً");
            }
            else
            {
                errorProvider1.SetError(txtFullName, null);
            }
        }

        private void txtPassportNumber_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassportNumber.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtPassportNumber, "رقم الجواز لا يمكن أن يكون فارغاً");
            }
            else
            {
                errorProvider1.SetError(txtPassportNumber, null);
            }
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text.Trim()) && !txtEmail.Text.Contains("@"))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, "صيغة البريد الإلكتروني غير صحيحة");
            }
            else
            {
                errorProvider1.SetError(txtEmail, null);
            }
        }

        // ================= Save & Operations =================

        private bool _HandlePersonImage()
        {
            // التحقق مما إذا كانت الصورة قد تغيرت فعلاً
            if (_Client.ImagePath != pbClientImage.ImageLocation)
            {
                // 1. حذف الصورة القديمة من الفولدر إن وجدت
                if (!string.IsNullOrEmpty(_Client.ImagePath))
                {
                    try
                    {
                        if (File.Exists(_Client.ImagePath))
                        {
                            File.Delete(_Client.ImagePath);
                        }
                    }
                    catch (IOException)
                    {
                        // تعذر الحذف بسبب قفل الملف أو تصاريح
                    }
                }

                // 2. إذا تم اختيار صورة جديدة
                if (!string.IsNullOrEmpty(pbClientImage.ImageLocation))
                {
                    string SourceImageFile = pbClientImage.ImageLocation;

                    if (clsUtil.CopyImageToProjectImagesFolder(ref SourceImageFile))
                    {
                        pbClientImage.ImageLocation = SourceImageFile;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Error Copying Image File", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else
                {
                    // إذا قام المستخدم بتفرغ الصورة (مسح الصورة)
                    _Client.ImagePath = "";
                }
            }

            return true;
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("بعض الحقول غير صالحة! ضع الماوس فوق الأيقونة الحمراء لرؤية الخطأ",
                    "خطأ في التحقق", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. أولاً: معالجة الصورة (نسخها للمشروع وتحديث pbClientImage.ImageLocation بالمسار الجديد)
            if (!_HandlePersonImage())
                return;

            // 2. ثانياً: تعبئة بقية البيانات في الكائن
            _Client.FullName = txtFullName.Text.Trim();
            _Client.PassportNumber = txtPassportNumber.Text.Trim();
            _Client.Email = txtEmail.Text.Trim();
            _Client.Password = txtPassword.Text.Trim();
            _Client.PhoneNumber = txtPhoneNumber.Text.Trim();
            _Client.Address = txtAddress.Text.Trim();
            _Client.Notes = txtNotes.Text.Trim();

            _Client.CountryID = cbCountries.SelectedValue != null ? Convert.ToInt32(cbCountries.SelectedValue) : -1;
            _Client.VisaTypeID = cbVisaTypes.SelectedValue != null ? Convert.ToInt32(cbVisaTypes.SelectedValue) : -1;

            // 3. ثالثاً: إسناد مسار الصورة النهائي (بعد ما اتنسخت وحصل لها Update)
            _Client.ImagePath = pbClientImage.ImageLocation ?? "";

            // 4. رابعاً: الحفظ في قاعدة البيانات
            if (_Client.Save())
            {
                lblClientID.Text = _Client.ID.ToString();
                _Mode = enMode.Update;
                lblTitle.Text = "تعديل بيانات عميل";
                lblUpdatedAt.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                MessageBox.Show("تم حفظ البيانات بنجاح.", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("حدث خطأ: لم يتم حفظ البيانات بنجاح.", "خطأ في الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void llSetImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog1.FileName;
                pbClientImage.ImageLocation = selectedFilePath;
                llRemoveImage.Visible = true;
            }
        }

        private void llRemoveImage_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pbClientImage.ImageLocation = null;
            pbClientImage.Image = Resources.users_512;
            llRemoveImage.Visible = false;
        }

        private void txtFullName_Validating_1(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFullName, "اسم العميل مطلوب ولا يمكن أن يكون فارغاً");
            }
            else
            {
                errorProvider1.SetError(txtFullName, null);
            }
        }

        private void txtPassportNumber_Validating_1(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassportNumber.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtPassportNumber, "رقم الجواز مطلوب ولا يمكن أن يكون فارغاً");
            }
            else
            {
                errorProvider1.SetError(txtPassportNumber, null);
            }
        }

        private void txtPhoneNumber_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtPhoneNumber, "رقم الهاتف مطلوب");
            }
            else
            {
                errorProvider1.SetError(txtPhoneNumber, null);
            }
        }

        private void cbCountries_Validating(object sender, CancelEventArgs e)
        {
            if (cbCountries.SelectedIndex == -1 || cbCountries.SelectedValue == null)
            {
                e.Cancel = true;
                errorProvider1.SetError(cbCountries, "برجاء اختيار الدولة من القائمة");
            }
            else
            {
                errorProvider1.SetError(cbCountries, null);
            }
        }

        private void cbVisaTypes_Validating(object sender, CancelEventArgs e)
        {
            if (cbVisaTypes.SelectedIndex == -1 || cbVisaTypes.SelectedValue == null)
            {
                e.Cancel = true;
                errorProvider1.SetError(cbVisaTypes, "برجاء اختيار نوع التأشيرة من القائمة");
            }
            else
            {
                errorProvider1.SetError(cbVisaTypes, null);
            }
        }

        private void txtPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtCountryFilter_TextChanged(object sender, EventArgs e)
        {
            string filterText = txtCountryFilter.Text.Trim();

            DataTable dt = cbCountries.DataSource as DataTable;

            if (dt == null)
                return;

            DataView dv = dt.DefaultView;

            if (string.IsNullOrWhiteSpace(filterText))
            {
                dv.RowFilter = "";
                cbCountries.SelectedIndex = -1;
                cbCountries.DroppedDown = false;
                return;
            }

            // حفظ الفلتر الحالي
            string oldFilter = dv.RowFilter;

            // تجربة الفلتر الجديد
            dv.RowFilter = $"Name LIKE '{filterText.Replace("'", "''")}%'";

            if (dv.Count > 0)
            {
                cbCountries.DroppedDown = true;
            }
            else
            {
                // الرجوع للفلتر السابق
                dv.RowFilter = oldFilter;

                // حذف آخر حرف كتبه المستخدم
                txtCountryFilter.Text = filterText.Substring(0, filterText.Length - 1);
                txtCountryFilter.SelectionStart = txtCountryFilter.Text.Length;
            }
        }

        private void txtCountryFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            string nextText = txtCountryFilter.Text + e.KeyChar;

            if (cbCountries.DataSource is DataTable dt)
            {
                DataView dv = new DataView(dt);

                dv.RowFilter = $"Name LIKE '{nextText.Replace("'", "''")}%'";

                if (dv.Count == 0)
                {
                    e.Handled = true; // يمنع إدخال الحرف
                }
            }
        }
    }
}