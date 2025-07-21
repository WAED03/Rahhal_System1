// استيراد المكاتب الضرورية
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rahhal_System1.DAL;      // للوصول إلى كلاس DbHelper
using System.Data.SqlClient;    // التعامل مع SQL Server
using Rahhal_System1.Forms;     // للوصول إلى الفورم NewTrip
using Rahhal_System1.Models;    // النماذج (Models)
using Rahhal_System1.Data;      // بيانات عامة مثل GlobalData

namespace Rahhal_System1.UC
{
    public partial class TripsUC : UserControl
    {
        // المُنشئ
        public TripsUC()
        {
            InitializeComponent();
            LoadTrips(); // تحميل بيانات الرحلات عند تحميل الواجهة
        }

        // دالة لتحميل الرحلات وعرضها في DataGridView
        private void LoadTrips()
        {
            // تحديث قائمة الرحلات من قاعدة البيانات (حسب المستخدم الحالي)
            GlobalData.RefreshTrips(ActivityLogger.CurrentUser.UserID);
            var trips = GlobalData.TripsList;

            // ربط قائمة الرحلات بجدول DataGridView مع تحديد الحقول المعروضة
            dgTrips.DataSource = trips.Select(t => new
            {
                t.TripID,
                t.TripName,
                t.StartDate,
                t.EndDate,
                t.TravelMethod,
                t.Notes
            }).ToList();

            // تغيير عناوين الأعمدة لتكون مفهومة أكثر
            if (dgTrips.Columns.Contains("TripName"))
                dgTrips.Columns["TripName"].HeaderText = "Trip Name";
            if (dgTrips.Columns.Contains("StartDate"))
                dgTrips.Columns["StartDate"].HeaderText = "Start Date";
            if (dgTrips.Columns.Contains("EndDate"))
                dgTrips.Columns["EndDate"].HeaderText = "End Date";
            if (dgTrips.Columns.Contains("TravelMethod"))
                dgTrips.Columns["TravelMethod"].HeaderText = "Travel Method";
            if (dgTrips.Columns.Contains("Notes"))
                dgTrips.Columns["Notes"].HeaderText = "Notes";

            // إضافة أعمدة أزرار "تعديل" و"حذف" إذا لم تكن موجودة مسبقًا
            if (dgTrips.Columns["Edit"] == null && dgTrips.Columns["Delete"] == null)
            {
                dgTrips.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Edit",
                    HeaderText = "",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true
                });

                dgTrips.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Delete",
                    HeaderText = "",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true
                });

                // تخصيص شكل الزرين (لون وخط)
                dgTrips.CellPainting += (s, e) =>
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex == dgTrips.Columns["Edit"].Index)
                    {
                        e.PaintBackground(e.CellBounds, true);
                        using (Brush b = new SolidBrush(Color.DodgerBlue))
                            e.Graphics.FillRectangle(b, e.CellBounds);

                        TextRenderer.DrawText(e.Graphics, "Edit", e.CellStyle.Font, e.CellBounds, Color.White,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        e.Handled = true;
                    }
                    else if (e.RowIndex >= 0 && e.ColumnIndex == dgTrips.Columns["Delete"].Index)
                    {
                        e.PaintBackground(e.CellBounds, true);
                        using (Brush b = new SolidBrush(Color.IndianRed))
                            e.Graphics.FillRectangle(b, e.CellBounds);

                        TextRenderer.DrawText(e.Graphics, "Delete", e.CellStyle.Font, e.CellBounds, Color.White,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        e.Handled = true;
                    }
                };
            }
        }

        // عند الضغط على زر داخل الـ DataGridView
        private void dgTrips_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // تجاهل الضغط على رأس الجدول

            // استخراج ID واسم الرحلة من السطر المحدد
            int tripID = Convert.ToInt32(dgTrips.Rows[e.RowIndex].Cells["TripID"].Value);
            string tripName = dgTrips.Rows[e.RowIndex].Cells["TripName"].Value.ToString();

            // إذا تم الضغط على زر "حذف"
            if (dgTrips.Columns[e.ColumnIndex].Name == "Delete")
            {
                var confirm = MessageBox.Show($"Are you sure you want to delete '{tripName}'?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    SoftDeleteTrip(tripID, tripName); // تنفيذ الحذف الناعم
                    LoadTrips(); // إعادة تحميل القائمة بعد الحذف
                }
            }

            // إذا تم الضغط على زر "تعديل"
            else if (dgTrips.Columns[e.ColumnIndex].Name == "Edit")
            {
                NewTrip editForm = new NewTrip(tripID); // فتح الفورم مع تمرير رقم الرحلة للتعديل
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadTrips(); // تحديث الجدول بعد التعديل
                }
            }
        }

        // دالة لتنفيذ الحذف الناعم للرحلة وجميع البيانات المرتبطة بها
        private void SoftDeleteTrip(int tripID, string tripName)
        {
            using (var con = DbHelper.GetConnection())
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction(); // فتح معاملة لضمان التكامل

                try
                {
                    // 1. حذف الرحلة نفسها (soft delete)
                    SqlCommand cmdTrip = new SqlCommand("UPDATE Trip SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE TripID = @TripID", con, transaction);
                    cmdTrip.Parameters.AddWithValue("@TripID", tripID);
                    cmdTrip.ExecuteNonQuery();

                    // 2. حذف جميع الزيارات المرتبطة بالرحلة (soft delete)
                    SqlCommand cmdVisits = new SqlCommand("UPDATE CityVisit SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE TripID = @TripID", con, transaction);
                    cmdVisits.Parameters.AddWithValue("@TripID", tripID);
                    cmdVisits.ExecuteNonQuery();

                    // 3. حذف جميع العبارات المرتبطة بالزيارات (soft delete)
                    SqlCommand cmdPhrases = new SqlCommand(@"
                        UPDATE Phrase SET IsDeleted = 1, UpdatedAt = GETDATE()
                        WHERE VisitID IN (SELECT VisitID FROM CityVisit WHERE TripID = @TripID)", con, transaction);
                    cmdPhrases.Parameters.AddWithValue("@TripID", tripID);
                    cmdPhrases.ExecuteNonQuery();

                    // 4. تسجيل العملية في سجل النشاطات
                    ActivityLogger.Log(con, transaction, "SoftDelete Trip", $"Soft-deleted trip '{tripName}' (ID = {tripID})");

                    transaction.Commit(); // تأكيد المعاملة
                    GlobalData.RefreshTrips(ActivityLogger.CurrentUser.UserID); // تحديث البيانات العامة
                    MessageBox.Show("🗑️ Trip deleted successfully (soft delete).");
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // في حالة الخطأ يتم التراجع عن كل العمليات
                    MessageBox.Show("❌ Error during delete: " + ex.Message);
                }
            }
        }

        // عند الضغط على زر "إضافة رحلة جديدة"
        private void btnAddTrip_Click(object sender, EventArgs e)
        {
            NewTrip newTripForm = new NewTrip(); // فتح فورم إضافة رحلة

            if (newTripForm.ShowDialog() == DialogResult.OK)
            {
                LoadTrips(); // إعادة تحميل الرحلات بعد الإضافة
            }
        }
    }
}
