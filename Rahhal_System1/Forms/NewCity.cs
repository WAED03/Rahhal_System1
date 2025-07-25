using Rahhal_System1.DAL;
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace Rahhal_System1.Forms
{
    public partial class NewCity : Form
    {
        // معرف الزيارة في حالة التعديل، إذا كان null فهذا يعني إضافة جديدة
        private int? editingVisitID = null;

        // المُنشئ الافتراضي يقوم باستدعاء المُنشئ الأساسي مع قيمة null (لإضافة جديدة)
        public NewCity() : this(null)
        {
        }

        // المُنشئ الأساسي يستقبل معرف الزيارة في حالة التعديل
        public NewCity(int? visitID)
        {
            InitializeComponent();
            editingVisitID = visitID;

            // ضبط قيم النطاق للتقييم من 1 إلى 5 والقيمة الافتراضية 3
            nudRating.Minimum = 1;
            nudRating.Maximum = 5;
            nudRating.Value = 3;

            // ربط حدث تحميل الفورم بالدالة NewCity_Load1
            this.Load += NewCity_Load1;

            // ربط حدث تغيير اختيار الدولة بالدالة cbCountries_SelectedIndexChanged
            this.cbCountries.SelectedIndexChanged += cbCountries_SelectedIndexChanged;
        }

        // دالة للتحقق من صحة تعبئة الحقول في الفورم
        private bool ValidateFields()
        {
            bool isValid = true;
            errorProvider1.Clear(); // مسح الأخطاء السابقة

            // التحقق من اختيار الرحلة
            if (cbTrips.SelectedItem == null)
            {
                errorProvider1.SetError(cbTrips, "Please select a trip");
                isValid = false;
            }

            // التحقق من اختيار الدولة
            if (cbCountries.SelectedItem == null)
            {
                errorProvider1.SetError(cbCountries, "Please select a country");
                isValid = false;
            }

            // التحقق من اختيار المدينة
            if (cbCities.SelectedItem == null)
            {
                errorProvider1.SetError(cbCities, "Please select a city");
                isValid = false;
            }

            // التحقق من إدخال ملاحظات
            if (string.IsNullOrWhiteSpace(txtNotes.Text))
            {
                errorProvider1.SetError(txtNotes, "Please enter notes");
                isValid = false;
            }

            // التأكد من أن التقييم ضمن النطاق المسموح به
            if (nudRating.Value < 1 || nudRating.Value > 5)
            {
                errorProvider1.SetError(nudRating, "Please select a rating between 1 and 5");
                isValid = false;
            }

            return isValid;
        }

        // حدث الضغط على زر الحفظ لإضافة أو تعديل زيارة المدينة
        private void btnSaveCity_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return; // إذا لم تتحقق الشروط لا تستمر

            // جمع البيانات من الحقول
            int tripId = (int)cbTrips.SelectedValue;
            int cityId = (int)cbCities.SelectedValue;
            DateTime visitDate = dtpVisitDate.Value.Date;
            string rating = nudRating.Value.ToString();
            string notes = txtNotes.Text.Trim();

            try
            {
                bool success;

                if (editingVisitID == null)
                {
                    // إذا إضافة جديدة، إنشاء كائن جديد للزيارة وتمريره إلى DAL
                    CityVisit newVisit = new CityVisit
                    {
                        TripID = tripId,
                        CityID = cityId,
                        VisitDate = visitDate,
                        Rating = rating,
                        Notes = notes
                    };

                    success = CityVisitDAL.AddVisit(newVisit);
                }
                else
                {
                    // إذا تعديل، إنشاء كائن مع المعرف وتمريره لتحديث البيانات
                    CityVisit updatedVisit = new CityVisit
                    {
                        VisitID = editingVisitID.Value,
                        TripID = tripId,
                        CityID = cityId,
                        VisitDate = visitDate,
                        Rating = rating,
                        Notes = notes
                    };

                    success = CityVisitDAL.UpdateVisit(updatedVisit);
                }

                if (success)
                {
                    // تسجيل النشاط (إضافة أو تعديل) في السجل
                    string action = editingVisitID == null ? "Add CityVisit" : "Edit CityVisit";
                    using (SqlConnection con = DbHelper.GetConnection())
                    {
                        con.Open();
                        ActivityLogger.Log(con, action, $"CityID={cityId}, Rating={rating}, Date={visitDate:yyyy-MM-dd}");
                    }

                    // إظهار رسالة نجاح وإغلاق الفورم مع تعيين نتيجة DialogResult
                    MessageBox.Show(editingVisitID == null ? "✅ Visit saved successfully." : "✏️ Visit updated successfully.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // رسالة فشل العملية
                    MessageBox.Show("❌ Operation failed.");
                }
            }
            catch (Exception ex)
            {
                // رسالة الخطأ في حالة الاستثناء
                MessageBox.Show("❌ Error: " + ex.Message);
            }
        }

        // حدث تحميل الفورم، يتم تحميل البيانات الضرورية للقوائم المنسدلة
        private void NewCity_Load1(object sender, EventArgs e)
        {
            // تحميل الرحلات الخاصة بالمستخدم الحالي عبر DAL
            cbTrips.DataSource = TripDAL.GetTripsByUser(ActivityLogger.CurrentUser.UserID);
            cbTrips.DisplayMember = "TripName";  // ما يعرض في القائمة
            cbTrips.ValueMember = "TripID";      // القيمة الفعلية المرتبطة بكل عنصر

            // تحميل قائمة الدول
            cbCountries.DataSource = CountryDAL.GetAllCountries();
            cbCountries.DisplayMember = "CountryName";
            cbCountries.ValueMember = "CountryID";

            // تحميل المدن للدولة الأولى المختارة
            if (cbCountries.Items.Count > 0 && cbCountries.SelectedValue != null)
            {
                int firstCountryId = Convert.ToInt32(cbCountries.SelectedValue);
                cbCities.DataSource = CityDAL.GetCitiesByCountryTable(firstCountryId);
                cbCities.DisplayMember = "CityName";
                cbCities.ValueMember = "CityID";
            }

            // تعيين قيم النطاق الافتراضية للتقييم (يمكن حذفها إذا سبق وتم تعيينها في المُنشئ)
            nudRating.Minimum = 1;
            nudRating.Maximum = 5;
            nudRating.Value = 3;

            // إذا كانت عملية تعديل، تحميل بيانات الزيارة الموجودة مسبقًا
            if (editingVisitID != null)
            {
                LoadVisitData((int)editingVisitID);
            }
        }

        // حدث تغيير اختيار الدولة، يتم تحديث المدن بناءً على الدولة المحددة
        private void cbCountries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCountries.SelectedValue != null && int.TryParse(cbCountries.SelectedValue.ToString(), out int countryId))
            {
                cbCities.DataSource = CityDAL.GetCitiesByCountryTable(countryId);
                cbCities.DisplayMember = "CityName";
                cbCities.ValueMember = "CityID";
            }
        }

        // تحميل بيانات زيارة موجودة للتعديل بناءً على معرف الزيارة
        private void LoadVisitData(int visitID)
        {
            // استدعاء دالة من DAL لجلب تفاصيل الزيارة
            CityVisit visit = CityVisitDAL.GetVisitById(visitID);
            if (visit != null)
            {
                cbTrips.SelectedValue = visit.TripID;

                int cityId = visit.CityID;
                // الحصول على معرف الدولة من خلال معرف المدينة عبر DAL
                int countryId = CityDAL.GetCountryIdByCityId(cityId);

                cbCountries.SelectedValue = countryId;

                // تحميل المدن الخاصة بتلك الدولة
                cbCities.DataSource = CityDAL.GetCitiesByCountryTable(countryId);
                cbCities.DisplayMember = "CityName";
                cbCities.ValueMember = "CityID";
                cbCities.SelectedValue = cityId;

                dtpVisitDate.Value = visit.VisitDate;
                nudRating.Value = Convert.ToInt32(visit.Rating);
                txtNotes.Text = visit.Notes;
            }
        }

    }
}
