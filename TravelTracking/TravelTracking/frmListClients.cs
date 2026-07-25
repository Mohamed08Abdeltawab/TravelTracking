using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TravelAgencyBusiness;

namespace TravelTracking
{
    public partial class frmListClients : Form
    {
        private DataTable _dtAllClients;

        public frmListClients()
        {
            InitializeComponent();
        }

        private void _FillCountriesComboBox()
        {
            DataTable dtCountries = clsCountry.GetAllCountries();

            if (dtCountries != null && dtCountries.Rows.Count > 0)
            {
                // إضافة خيار "الكل" في بداية القائمة
                DataRow dr = dtCountries.NewRow();
                dr["Id"] = -1;
                dr["Name"] = "الكل";
                dtCountries.Rows.InsertAt(dr, 0);

                cbCountries.DisplayMember = "Name";
                cbCountries.ValueMember = "Name"; // نستخدم اسم الدولة للفلترة المباشرة
                cbCountries.DataSource = dtCountries;
                cbCountries.SelectedIndex = 0;
            }
        }

        private void _FillVisaTypesComboBox()
        {
            DataTable dtVisaTypes = clsVisaType.GetAllVisaTypes();

            if (dtVisaTypes != null && dtVisaTypes.Rows.Count > 0)
            {
                // إضافة خيار "الكل" في بداية القائمة
                DataRow dr = dtVisaTypes.NewRow();
                dr["Id"] = -1;
                dr["Name"] = "الكل";
                dtVisaTypes.Rows.InsertAt(dr, 0);

                cbVisaTypes.DisplayMember = "Name";
                cbVisaTypes.ValueMember = "Name"; // نستخدم اسم التأشيرة للفلترة المباشرة
                cbVisaTypes.DataSource = dtVisaTypes;
                cbVisaTypes.SelectedIndex = 0;
            }
        }

        private void _RefreshClientsList()
        {
            _dtAllClients = clsClient.GetAllClients();
            dgvUsers.DataSource = _dtAllClients;
            lblRecordsCount.Text = dgvUsers.Rows.Count.ToString();

            if (dgvUsers.Rows.Count > 0)
            {
                cbFilterBy.Enabled = true;

                // إعادة تسمية وتنسيق عناوين الأعمدة باللغة العربية
                dgvUsers.Columns["Id"].HeaderText = "معرف العميل";
                dgvUsers.Columns["Id"].Width = 100;

                dgvUsers.Columns["FullName"].HeaderText = "اسم العميل";
                dgvUsers.Columns["FullName"].Width = 180;

                dgvUsers.Columns["PassportNumber"].HeaderText = "رقم الجواز";
                dgvUsers.Columns["PassportNumber"].Width = 150;

                dgvUsers.Columns["CountryName"].HeaderText = "الدولة";
                dgvUsers.Columns["CountryName"].Width = 130;

                dgvUsers.Columns["Email"].HeaderText = "الإيميل";
                dgvUsers.Columns["Email"].Width = 180;

                dgvUsers.Columns["PhoneNumber"].HeaderText = "رقم الموبايل";
                dgvUsers.Columns["PhoneNumber"].Width = 180;

                dgvUsers.Columns["VisaTypeName"].HeaderText = "نوع التأشيرة";
                dgvUsers.Columns["VisaTypeName"].Width = 120;

                dgvUsers.Columns["CreatedAt"].HeaderText = "تاريخ الإنشاء";
                dgvUsers.Columns["CreatedAt"].Width = 190;

            }
            else
            {
                cbFilterBy.SelectedIndex = 0;
                cbFilterBy.Enabled = false;
            }
        }

        private void frmListClients_Load(object sender, EventArgs e)
        {
            _FillCountriesComboBox();
            _FillVisaTypesComboBox();
            _RefreshClientsList();

            cbFilterBy.SelectedIndex = 0;
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_dtAllClients != null)
                _dtAllClients.DefaultView.RowFilter = "";

            txtFilterValue.Text = "";

            /* 
               عناصر cbFilterBy المتاحة:
               0: لا شيء
               1: معرف العميل (Id)
               2: اسم العميل (FullName)
               3: رقم الموبيل (PhoneNumber)
               4: الإيميل (Email)
               5: الدولة (CountryName)
               6: نوع التأشيرة (VisaTypeName)
               7: الوقت/تاريخ الإنشاء (CreatedAt)
            */

            switch (cbFilterBy.SelectedIndex)
            {
                case 5: // الفلترة بالدولة
                    txtFilterValue.Visible = false;
                    cbVisaTypes.Visible = false;
                    cbCountries.Visible = true;
                    cbCountries.Location = new Point(315, 222); // ضبط الموقع بجانب الفلتر
                    cbCountries.SelectedIndex = 0;
                    break;

                case 6: // الفلترة بنوع التأشيرة
                    txtFilterValue.Visible = false;
                    cbCountries.Visible = false;
                    cbVisaTypes.Visible = true;
                    cbVisaTypes.Location = new Point(315, 222);
                    cbVisaTypes.SelectedIndex = 0;
                    break;

                default: // الفلترة بالنصوص أو "لا شيء"
                    cbCountries.Visible = false;
                    cbVisaTypes.Visible = false;
                    txtFilterValue.Visible = (cbFilterBy.SelectedIndex != 0);
                    txtFilterValue.Focus();
                    break;
            }

            lblRecordsCount.Text = dgvUsers.Rows.Count.ToString();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string filterColumn = "";

            switch (cbFilterBy.SelectedIndex)
            {
                case 1:
                    filterColumn = "Id";
                    break;
                case 2:
                    filterColumn = "FullName";
                    break;
                case 3:
                    filterColumn = "PhoneNumber";
                    break;
                case 4:
                    filterColumn = "Email";
                    break;
                default:
                    filterColumn = "None";
                    break;
            }

            if (txtFilterValue.Text.Trim() == "" || filterColumn == "None")
            {
                _dtAllClients.DefaultView.RowFilter = "";
                lblRecordsCount.Text = dgvUsers.Rows.Count.ToString();
                return;
            }

            if (filterColumn == "Id")
            {
                // التعامل مع الأرقام بشكل مباشر
                _dtAllClients.DefaultView.RowFilter = string.Format("[{0}] = {1}", filterColumn, txtFilterValue.Text.Trim());
            }
            else
            {
                // التعامل مع النصوص باستخدام LIKE
                _dtAllClients.DefaultView.RowFilter = string.Format("[{0}] LIKE '%{1}%'", filterColumn, txtFilterValue.Text.Trim().Replace("'", "''"));
            }

            lblRecordsCount.Text = dgvUsers.Rows.Count.ToString();
        }

        private void cbCountries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCountries.SelectedValue == null || _dtAllClients == null) return;

            string filterValue = cbCountries.SelectedValue.ToString();

            if (filterValue == "الكل" || cbCountries.SelectedIndex == 0)
            {
                _dtAllClients.DefaultView.RowFilter = "";
            }
            else
            {
                _dtAllClients.DefaultView.RowFilter = string.Format("[CountryName] = '{0}'", filterValue.Replace("'", "''"));
            }

            lblRecordsCount.Text = dgvUsers.Rows.Count.ToString();
        }

        private void cbVisaTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVisaTypes.SelectedValue == null || _dtAllClients == null) return;

            string filterValue = cbVisaTypes.SelectedValue.ToString();

            if (filterValue == "الكل" || cbVisaTypes.SelectedIndex == 0)
            {
                _dtAllClients.DefaultView.RowFilter = "";
            }
            else
            {
                _dtAllClients.DefaultView.RowFilter = string.Format("[VisaTypeName] = '{0}'", filterValue.Replace("'", "''"));
            }

            lblRecordsCount.Text = dgvUsers.Rows.Count.ToString();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.SelectedIndex == 1 || cbFilterBy.SelectedIndex == 3)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        // ================= الإجراءات والفتح والتعديل والحذف =================

        private void btnAddClient_Click(object sender, EventArgs e)
        {
            try
            {
                frmAddUpdateClient frm = new frmAddUpdateClient();
                frm.ShowDialog();
            }
            catch { }
            
            _RefreshClientsList();
        }

        private void AddNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddClient_Click(sender, e);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;

            int clientID = Convert.ToInt32(dgvUsers.CurrentRow.Cells["Id"].Value);
            try
            {
                frmAddUpdateClient frm = new frmAddUpdateClient(clientID);
                frm.ShowDialog();
            }
            catch { }
            _RefreshClientsList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;

            int clientID = Convert.ToInt32(dgvUsers.CurrentRow.Cells["Id"].Value);
            string clientName = dgvUsers.CurrentRow.Cells["FullName"].Value.ToString();

            if (MessageBox.Show($"هل أنت أختيارياً تأكد من حذف العميل [{clientName}]؟", "تأكيد الحذف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // جلب بيانات العميل لمسح صورته من الفولدر إن وجدت قبل مسح السجل
                clsClient client = clsClient.Find(clientID);

                if (client != null && !string.IsNullOrEmpty(client.ImagePath) && File.Exists(client.ImagePath))
                {
                    try
                    {
                        File.Delete(client.ImagePath);
                    }
                    catch (Exception)
                    {
                        // التغاضي عن أخطاء القفل أثناء الحذف
                    }
                }

                if (clsClient.DeleteClient(clientID))
                {
                    MessageBox.Show("تم حذف العميل بنجاح.", "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshClientsList();
                }
                else
                {
                    MessageBox.Show("حدث خطأ: لم يتم حذف العميل.", "فشل الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // التأكد إن الضغط كان على صف حقيقي مش على الهيدر
            if (dgvUsers.CurrentRow == null || dgvUsers.CurrentRow.Index < 0) return;

            int clientID = Convert.ToInt32(dgvUsers.CurrentRow.Cells["Id"].Value);

            try
            {
                frmAddUpdateClient frm = new frmAddUpdateClient(clientID);
                frm.ShowDialog();
            }
            catch { }

            _RefreshClientsList();
        }
    }
}