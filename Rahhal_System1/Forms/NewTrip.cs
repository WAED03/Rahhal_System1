// مراجع مهمة للتعامل مع عناصر الفورم، قواعد البيانات، والكائنات المعرفة مسبقاً
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
using Rahhal_System1.DAL;     // الوصول لطبقة التعامل مع البيانات (Data Access Layer)
using Rahhal_System1.Models;  // الوصول لتعريف الكائنات (Models)

namespace Rahhal_System1.Forms
{
    public partial class NewTrip : Form
    {
        // متغير لتخزين رقم الرحلة عند التعديل، nullable لأن الحالة ممكن تكون إضافة جديدة (null)
        private int? editingTripID = null;

        // المنشئ الافتراضي لإنشاء نموذج جديد لإضافة رحلة
        public NewTrip()
        {
            InitializeComponent();  // تهيئة مكونات الواجهة
            FillTravelMethods();    // تعبئة قائمة وسائل السفر
            this.Load += NewTrip_Load;  // ربط حدث تحميل الفورم مع الدالة NewTrip_Load
        }

        // منشئ ثاني خاص بحالة التعديل حيث يتم تمرير معرف الرحلة لتحميل بياناتها
        public NewTrip(int tripID) : this()
        {
            editingTripID = tripID;  // حفظ معرف الرحلة التي سيتم تعديلها
        }

        // دالة تشتغل عند تحميل الفورم
        private void NewTrip_Load(object sender, EventArgs e)
        {
            FillTravelMethods();  // تعبئة ComboBox بوسائل السفر

            // إذا كان لدينا معرف رحلة (أي تعديل) نحمّل بيانات الرحلة
            if (editingTripID != null)
            {
                LoadTripData((int)editingTripID);
            }
        }

        // دالة تعبئة ComboBox بوسائل السفر المتاحة
        private void FillTravelMethods()
        {
            cbTravelMethod.Items.Clear();  // تنظيف المحتوى القديم
            cbTravelMethod.Items.AddRange(new string[]
            {
                "Airplane", "Ship", "Car", "Bus", "Boat", "Walking"  // إضافة الخيارات
            });
            cbTravelMethod.SelectedIndex = 0;  // اختيار أول عنصر تلقائياً
        }

        // دالة التحقق من صحة البيانات المدخلة في الحقول
        private bool ValidateFields()
        {
            bool isValid = true;
            errorProvider1.Clear();  // إزالة أي أخطاء سابقة

            // التحقق من تعبئة اسم الرحلة
            if (string.IsNullOrWhiteSpace(txtTripName.Text))
            {
                errorProvider1.SetError(txtTripName, "Please enter trip name");
                isValid = false;
            }

            // التحقق من اختيار وسيلة السفر
            if (cbTravelMethod.SelectedItem == null)
            {
                errorProvider1.SetError(cbTravelMethod, "Please select a travel method");
                isValid = false;
            }

            // التحقق من أن تاريخ الانتهاء بعد تاريخ البداية
            if (dtpEndDate.Value.Date < dtpStartDate.Value.Date)
            {
                errorProvider1.SetError(dtpEndDate, "End date must be after start date");
                isValid = false;
            }

            // التحقق من وجود ملاحظات
            if (string.IsNullOrWhiteSpace(txtNotes.Text))
            {
                errorProvider1.SetError(txtNotes, "Please enter notes");
                isValid = false;
            }

            return isValid;
        }

        // حدث الضغط على زر حفظ الرحلة
        private void btnSaveTrip_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;  // التحقق من صحة الحقول، إذا خطأ نوقف التنفيذ

            // إنشاء كائن رحلة جديد وتعبئته من بيانات الواجهة
            var trip = new Trip
            {
                TripName = txtTripName.Text.Trim(),
                StartDate = dtpStartDate.Value.Date,
                EndDate = dtpEndDate.Value.Date,
                TravelMethod = cbTravelMethod.SelectedItem.ToString(),
                Notes = txtNotes.Text.Trim(),
                UserID = ActivityLogger.CurrentUser.UserID  // ربط الرحلة بالمستخدم الحالي
            };

            bool success;

            // التحقق هل هو إضافة جديدة أو تعديل
            if (editingTripID == null)
            {
                success = TripDAL.AddTrip(trip);  // إضافة الرحلة الجديدة لقاعدة البيانات
            }
            else
            {
                trip.TripID = editingTripID.Value;  // تعيين معرف الرحلة للتعديل
                success = TripDAL.UpdateTrip(trip);  // تعديل بيانات الرحلة في قاعدة البيانات
            }

            // إذا تمت العملية بنجاح
            if (success)
            {
                string action = editingTripID == null ? "Add Trip" : "Update Trip";

                // تسجيل النشاط في سجل الأحداث (اللوج)
                using (var con = DbHelper.GetConnection())
                {
                    con.Open();
                    ActivityLogger.Log(con, action, $"Trip: {trip.TripName}");
                }

                // إظهار رسالة نجاح
                MessageBox.Show(editingTripID == null ? "✅ Trip saved successfully." : "✏️ Trip updated successfully.");

                this.DialogResult = DialogResult.OK;  // إشارة للنافذة الأصلية أن العملية تمت بنجاح
                this.Close();  // إغلاق نموذج الإضافة/التعديل
            }
            else
            {
                MessageBox.Show("❌ Operation failed.");  // رسالة خطأ في حالة فشل العملية
            }
        }

        // دالة لتحميل بيانات رحلة معينة في حالة التعديل
        private void LoadTripData(int tripID)
        {
            Trip trip = TripDAL.GetTripById(tripID);  // جلب بيانات الرحلة من قاعدة البيانات
            if (trip != null)
            {
                // تعبئة الحقول بالبيانات التي تم جلبها
                txtTripName.Text = trip.TripName;
                dtpStartDate.Value = trip.StartDate;
                dtpEndDate.Value = trip.EndDate;
                cbTravelMethod.SelectedItem = trip.TravelMethod;
                txtNotes.Text = trip.Notes;
            }
        }
    }
}
