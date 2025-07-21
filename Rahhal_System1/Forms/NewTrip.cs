// المراجع المطلوبة للتعامل مع عناصر الفورم، الاتصال بقاعدة البيانات، والكائنات المخصصة
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Rahhal_System1.DAL;

namespace Rahhal_System1.Forms
{
    public partial class NewTrip : Form
    {
        // متغير لتخزين رقم الرحلة إذا كنا في وضع تعديل
        private int? editingTripID = null;

        // منشئ النموذج عند فتح الفورم لإضافة رحلة جديدة
        public NewTrip()
        {
            InitializeComponent(); // تهيئة عناصر الواجهة
            FillTravelMethods();   // تعبئة وسيلة السفر داخل ComboBox
        }

        // منشئ ثاني عند فتح النموذج للتعديل على رحلة موجودة
        public NewTrip(int tripID) : this()
        {
            editingTripID = tripID; // نحدد رقم الرحلة المراد تعديلها
        }

        // عند تحميل النموذج
        private void NewTrip_Load(object sender, EventArgs e)
        {
            FillTravelMethods(); // نعيد تعبئة ComboBox

            // إذا كنا في وضع تعديل (أي لدينا TripID)
            if (editingTripID != null)
            {
                LoadTripData((int)editingTripID); // تحميل بيانات الرحلة للتعديل
            }
        }

        // دالة لتعبئة ComboBox بوسائل السفر المتاحة
        private void FillTravelMethods()
        {
            cbTravelMethod.Items.Clear(); // نفرغ العناصر القديمة
            cbTravelMethod.Items.AddRange(new string[]
            {
                "Airplane", "Ship", "Car", "Bus", "Boat", "Walking"
            });
            cbTravelMethod.SelectedIndex = 0; // نختار أول عنصر كخيار افتراضي
        }

        // دالة للتحقق من صحة البيانات المدخلة من قبل المستخدم
        private bool ValidateFields()
        {
            bool isValid = true;
            errorProvider1.Clear(); // نزيل أي أخطاء سابقة

            // تحقق من اسم الرحلة
            if (string.IsNullOrWhiteSpace(txtTripName.Text))
            {
                errorProvider1.SetError(txtTripName, "Please enter trip name");
                isValid = false;
            }

            // تحقق من اختيار وسيلة السفر
            if (cbTravelMethod.SelectedItem == null)
            {
                errorProvider1.SetError(cbTravelMethod, "Please select a travel method");
                isValid = false;
            }

            // التحقق أن تاريخ الانتهاء بعد تاريخ البداية
            if (dtpEndDate.Value.Date < dtpStartDate.Value.Date)
            {
                errorProvider1.SetError(dtpEndDate, "End date must be after start date");
                isValid = false;
            }

            // تحقق من وجود ملاحظات
            if (string.IsNullOrWhiteSpace(txtNotes.Text))
            {
                errorProvider1.SetError(txtNotes, "Please enter notes");
                isValid = false;
            }

            return isValid;
        }

        // حدث الضغط على زر "حفظ الرحلة"
        private void btnSaveTrip_Click(object sender, EventArgs e)
        {
            // إذا كانت البيانات غير صحيحة، نوقف العملية
            if (!ValidateFields()) return;

            // استخراج البيانات من الحقول
            string tripName = txtTripName.Text.Trim();
            DateTime startDate = dtpStartDate.Value.Date;
            DateTime endDate = dtpEndDate.Value.Date;
            string travelMethod = cbTravelMethod.SelectedItem.ToString();
            string notes = txtNotes.Text.Trim();

            // فتح الاتصال بقاعدة البيانات
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();

                SqlCommand cmd;

                // إذا كنا نضيف رحلة جديدة
                if (editingTripID == null)
                {
                    cmd = new SqlCommand(
                        @"INSERT INTO Trip (UserID, TripName, StartDate, EndDate, TravelMethod, Notes)
                          VALUES (@UserID, @TripName, @StartDate, @EndDate, @TravelMethod, @Notes)", con);
                }
                else
                {
                    // إذا كنا نعدل على رحلة موجودة
                    cmd = new SqlCommand(
                        @"UPDATE Trip 
                          SET TripName = @TripName, StartDate = @StartDate, EndDate = @EndDate,
                              TravelMethod = @TravelMethod, Notes = @Notes, UpdatedAt = GETDATE()
                          WHERE TripID = @TripID", con);

                    cmd.Parameters.AddWithValue("@TripID", editingTripID); // نضيف رقم الرحلة للتعديل
                }

                // تمرير القيم كـ Parameters لحماية ضد SQL Injection
                cmd.Parameters.AddWithValue("@UserID", ActivityLogger.CurrentUser.UserID);
                cmd.Parameters.AddWithValue("@TripName", tripName);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                cmd.Parameters.AddWithValue("@TravelMethod", travelMethod);
                cmd.Parameters.AddWithValue("@Notes", notes);

                cmd.ExecuteNonQuery(); // تنفيذ الأمر (إضافة أو تعديل)

                // تسجيل النشاط في سجل الأحداث
                string action = (editingTripID == null) ? "Add Trip" : "Update Trip";
                ActivityLogger.Log(con, action, $"Trip: {tripName}");

                // رسالة نجاح
                MessageBox.Show(editingTripID == null ? "✅ Trip saved successfully." : "✏️ Trip updated successfully.");
                this.DialogResult = DialogResult.OK; // نرجع OK كإشارة للواجهة الأصلية
                this.Close(); // إغلاق النموذج
            }
        }

        // دالة لتحميل بيانات رحلة معينة للتعديل
        private void LoadTripData(int tripID)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    @"SELECT TripName, StartDate, EndDate, TravelMethod, Notes 
                      FROM Trip WHERE TripID = @TripID", con);
                cmd.Parameters.AddWithValue("@TripID", tripID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // تعبئة الحقول من البيانات
                        txtTripName.Text = reader["TripName"].ToString();
                        dtpStartDate.Value = Convert.ToDateTime(reader["StartDate"]);
                        dtpEndDate.Value = Convert.ToDateTime(reader["EndDate"]);
                        cbTravelMethod.SelectedItem = reader["TravelMethod"].ToString();
                        txtNotes.Text = reader["Notes"].ToString();
                    }
                }
            }
        }
    }
}
