using Rahhal_System1.DAL;
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
        // متغير لتحديد إذا كانت العملية "إضافة" أو "تعديل"
        private int? editingVisitID = null; // null تعني إضافة، وإذا تحتوي رقم فهي تعديل لزيارة موجودة

        // مُنشئ يتم استدعاؤه عند إنشاء النموذج في حالة "إضافة"
        public NewCity() : this(null)
        {
        }

        // مُنشئ يتم استدعاؤه في حالة "تعديل"، حيث يتم تمرير visitID
        public NewCity(int? visitID)
        {
            InitializeComponent();
            editingVisitID = visitID;

            // تحديد نطاق التقييم من 1 إلى 5
            nudRating.Minimum = 1;
            nudRating.Maximum = 5;
            nudRating.Value = 3;

            // عند تحميل النموذج يتم تنفيذ الحدث التالي
            this.Load += NewCity_Load1;
            // عند تغيير الدولة يتم إعادة تحميل المدن التابعة لها
            this.cbCountries.SelectedIndexChanged += cbCountries_SelectedIndexChanged;
        }

        // التحقق من أن جميع الحقول تم تعبئتها بشكل صحيح
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

            // التحقق من كتابة ملاحظات
            if (string.IsNullOrWhiteSpace(txtNotes.Text))
            {
                errorProvider1.SetError(txtNotes, "Please enter notes");
                isValid = false;
            }

            // التأكد من أن التقييم يقع بين 1 و 5
            if (nudRating.Value < 1 || nudRating.Value > 5)
            {
                errorProvider1.SetError(nudRating, "Please select a rating between 1 and 5");
                isValid = false;
            }

            return isValid;
        }

        // عند الضغط على زر "حفظ المدينة"
        private void btnSaveCity_Click(object sender, EventArgs e)
        {
            // التحقق من صحة الحقول
            if (!ValidateFields()) return;

            // قراءة البيانات من الواجهة
            int tripId = (int)cbTrips.SelectedValue;
            int cityId = (int)cbCities.SelectedValue;
            DateTime visitDate = dtpVisitDate.Value.Date;
            string rating = nudRating.Value.ToString();
            string notes = txtNotes.Text.Trim();

            // الاتصال بقاعدة البيانات
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();

                try
                {
                    SqlCommand cmd;

                    if (editingVisitID == null)
                    {
                        // تنفيذ أمر الإدخال (إضافة)
                        cmd = new SqlCommand(
                            @"INSERT INTO CityVisit (TripID, CityID, VisitDate, Rating, Notes)
                              VALUES (@TripID, @CityID, @VisitDate, @Rating, @Notes)", con);
                    }
                    else
                    {
                        // تنفيذ أمر التحديث (تعديل)
                        cmd = new SqlCommand(
                            @"UPDATE CityVisit 
                              SET TripID = @TripID, CityID = @CityID, VisitDate = @VisitDate, Rating = @Rating, Notes = @Notes, UpdatedAt = GETDATE()
                              WHERE VisitID = @VisitID", con);
                        cmd.Parameters.AddWithValue("@VisitID", editingVisitID.Value);
                    }

                    // تمرير القيم للمعاملات
                    cmd.Parameters.AddWithValue("@TripID", tripId);
                    cmd.Parameters.AddWithValue("@CityID", cityId);
                    cmd.Parameters.AddWithValue("@VisitDate", visitDate);
                    cmd.Parameters.AddWithValue("@Rating", rating);
                    cmd.Parameters.AddWithValue("@Notes", notes);

                    // تنفيذ الأمر
                    cmd.ExecuteNonQuery();

                    // تسجيل النشاط
                    string action = editingVisitID == null ? "Add CityVisit" : "Edit CityVisit";
                    ActivityLogger.Log(con, action, $"CityID={cityId}, Rating={rating}, Date={visitDate:yyyy-MM-dd}");

                    // إظهار رسالة نجاح
                    MessageBox.Show(editingVisitID == null ? "✅ Visit saved successfully." : "✏️ Visit updated successfully.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Error: " + ex.Message);
                }
            }
        }

        // عند تحميل النموذج
        private void NewCity_Load1(object sender, EventArgs e)
        {
            // تحميل الرحلات والدول
            cbTrips.DataSource = GetTrips();
            cbTrips.DisplayMember = "TripName";
            cbTrips.ValueMember = "TripID";

            cbCountries.DataSource = GetCountries();
            cbCountries.DisplayMember = "CountryName";
            cbCountries.ValueMember = "CountryID";

            // تحميل المدن للدولة المحددة
            if (cbCountries.Items.Count > 0 && cbCountries.SelectedValue != null)
            {
                int firstCountryId = Convert.ToInt32(cbCountries.SelectedValue);
                cbCities.DataSource = GetCitiesByCountry(firstCountryId);
                cbCities.DisplayMember = "CityName";
                cbCities.ValueMember = "CityID";
            }

            // إعداد تقييم افتراضي
            nudRating.Minimum = 1;
            nudRating.Maximum = 5;
            nudRating.Value = 3;

            // إذا كانت العملية تعديل، يتم تحميل البيانات القديمة
            if (editingVisitID != null)
            {
                LoadVisitData((int)editingVisitID);
            }
        }

        // عند تغيير الدولة يتم تحميل المدن الخاصة بها
        private void cbCountries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCountries.SelectedValue != null && int.TryParse(cbCountries.SelectedValue.ToString(), out int countryId))
            {
                cbCities.DataSource = GetCitiesByCountry(countryId);
                cbCities.DisplayMember = "CityName";
                cbCities.ValueMember = "CityID";
            }
        }

        // تحميل بيانات الزيارة عند تعديلها
        private void LoadVisitData(int visitID)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    @"SELECT TripID, CityID, VisitDate, Rating, Notes 
                      FROM CityVisit 
                      WHERE VisitID = @VisitID", con);
                cmd.Parameters.AddWithValue("@VisitID", visitID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cbTrips.SelectedValue = reader["TripID"];
                        int cityId = Convert.ToInt32(reader["CityID"]);
                        int countryId = GetCountryIdByCityId(cityId);

                        cbCountries.SelectedValue = countryId;

                        cbCities.DataSource = GetCitiesByCountry(countryId);
                        cbCities.DisplayMember = "CityName";
                        cbCities.ValueMember = "CityID";
                        cbCities.SelectedValue = cityId;

                        dtpVisitDate.Value = Convert.ToDateTime(reader["VisitDate"]);
                        nudRating.Value = Convert.ToInt32(reader["Rating"].ToString());
                        txtNotes.Text = reader["Notes"].ToString();
                    }
                }
            }
        }

        // الحصول على معرف الدولة بناءً على معرف المدينة
        private int GetCountryIdByCityId(int cityId)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT CountryID FROM City WHERE CityID = @CityID", con);
                cmd.Parameters.AddWithValue("@CityID", cityId);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        // جلب قائمة الدول
        private DataTable GetCountries()
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT CountryID, CountryName FROM Country WHERE IsDeleted = 0", con))
                {
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        // جلب الرحلات الخاصة بالمستخدم الحالي
        private DataTable GetTrips()
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT TripID, TripName FROM Trip WHERE IsDeleted = 0 AND UserID = @UserID", con))
                {
                    cmd.Parameters.AddWithValue("@UserID", ActivityLogger.CurrentUser.UserID);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        // جلب جميع المدن
        private DataTable GetCities()
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT CityID, CityName FROM City WHERE IsDeleted = 0", con))
                {
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        // جلب المدن التابعة لدولة معينة
        private DataTable GetCitiesByCountry(int countryId)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT CityID, CityName FROM City WHERE IsDeleted = 0 AND CountryID = @CountryID", con))
                {
                    cmd.Parameters.AddWithValue("@CountryID", countryId);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }
    }
}
