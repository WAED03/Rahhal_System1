using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rahhal_System1.DAL;
using System.Data.SqlClient;
using Rahhal_System1.Forms;
using Rahhal_System1.Data;
using Rahhal_System1.Models;

namespace Rahhal_System1.UC
{
    public partial class CitiesUC : UserControl
    {
        public CitiesUC()
        {
            InitializeComponent();
            LoadCities(); // تحميل قائمة المدن عند تحميل الواجهة
        }

        // دالة لتحميل زيارات المدن من قاعدة البيانات
        private void LoadCities()
        {
            // الحصول على رقم المستخدم الحالي
            int userId = ActivityLogger.CurrentUser.UserID;

            // تحديث قائمة الرحلات من قاعدة البيانات
            GlobalData.RefreshTrips(userId);

            // إعادة تعيين قائمة زيارات المدن
            GlobalData.CityVisitsList = new List<CityVisit>();

            // تحميل كل زيارات المدن من كل رحلة
            foreach (var trip in GlobalData.TripsList)
            {
                GlobalData.CityVisitsList.AddRange(CityVisitDAL.GetVisitsByTrip(trip.TripID));
            }

            // تجهيز البيانات لعرضها في DataGridView
            var cityData = GlobalData.CityVisitsList
                .Where(v => !v.IsDeleted) // استثناء المحذوفة
                .Select(v => new
                {
                    v.VisitID,
                    v.City.CityID,
                    CityName = v.City.CityName,
                    CountryName = v.City.Country.CountryName,
                    v.Rating,
                    v.VisitDate,
                    v.Notes
                })
                .ToList();

            dgCity.DataSource = cityData; // عرض البيانات في الجدول

            // تخصيص أسماء أعمدة الجدول
            if (dgCity.Columns.Count > 0)
            {
                dgCity.Columns["VisitID"].HeaderText = "Visit ID";
                dgCity.Columns["CityID"].HeaderText = "City ID";
                dgCity.Columns["CityName"].HeaderText = "City";
                dgCity.Columns["CountryName"].HeaderText = "Country";
                dgCity.Columns["Rating"].HeaderText = "Rating";
                dgCity.Columns["VisitDate"].HeaderText = "Visit Date";
                dgCity.Columns["Notes"].HeaderText = "Notes";
            }

            // إضافة زرّي التعديل والحذف إذا لم يكونا موجودين
            if (dgCity.Columns["Edit"] == null && dgCity.Columns["Delete"] == null)
            {
                // زر التعديل
                DataGridViewButtonColumn editButton = new DataGridViewButtonColumn();
                editButton.Name = "Edit";
                editButton.HeaderText = "";
                editButton.Text = "Edit";
                editButton.UseColumnTextForButtonValue = true;
                dgCity.Columns.Add(editButton);

                // زر الحذف
                DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn();
                deleteButton.Name = "Delete";
                deleteButton.HeaderText = "";
                deleteButton.Text = "Delete";
                deleteButton.UseColumnTextForButtonValue = true;
                dgCity.Columns.Add(deleteButton);

                // تخصيص مظهر زرّي التعديل والحذف
                dgCity.CellPainting += (s, e) =>
                {
                    if (e.RowIndex < 0) return;

                    if (e.ColumnIndex == dgCity.Columns["Edit"].Index)
                    {
                        e.PaintBackground(e.CellBounds, true);
                        using (Brush brush = new SolidBrush(Color.DodgerBlue))
                            e.Graphics.FillRectangle(brush, e.CellBounds);
                        TextRenderer.DrawText(e.Graphics, "Edit", e.CellStyle.Font, e.CellBounds, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        e.Handled = true;
                    }
                    else if (e.ColumnIndex == dgCity.Columns["Delete"].Index)
                    {
                        e.PaintBackground(e.CellBounds, true);
                        using (Brush brush = new SolidBrush(Color.IndianRed))
                            e.Graphics.FillRectangle(brush, e.CellBounds);
                        TextRenderer.DrawText(e.Graphics, "Delete", e.CellStyle.Font, e.CellBounds, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        e.Handled = true;
                    }
                };
            }
        }

        // عند الضغط على زر داخل الجدول (تعديل أو حذف)
        private void dgCity_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // تجاهل الضغط على رؤوس الأعمدة

            int visitID = Convert.ToInt32(dgCity.Rows[e.RowIndex].Cells["VisitID"].Value);
            string cityName = dgCity.Rows[e.RowIndex].Cells["CityName"].Value.ToString();

            // في حالة الضغط على زر الحذف
            if (dgCity.Columns[e.ColumnIndex].Name == "Delete")
            {
                var confirm = MessageBox.Show($"Are you sure you want to delete city '{cityName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    SoftDeleteCity(visitID, cityName); // تنفيذ الحذف الناعم
                    LoadCities(); // إعادة تحميل البيانات
                }
            }
            // في حالة الضغط على زر التعديل
            else if (dgCity.Columns[e.ColumnIndex].Name == "Edit")
            {
                NewCity editForm = new NewCity(visitID); // فتح نموذج التعديل
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCities(); // إعادة تحميل البيانات بعد التعديل
                }
            }
        }

        // تنفيذ الحذف الناعم للمدينة
        private void SoftDeleteCity(int visitID, string cityName)
        {
            using (var con = DbHelper.GetConnection())
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction(); // فتح ترانزكشن

                try
                {
                    // 1. تحديث زيارة المدينة إلى محذوف
                    SqlCommand cmdVisit = new SqlCommand("UPDATE CityVisit SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE VisitID = @VisitID", con, transaction);
                    cmdVisit.Parameters.AddWithValue("@VisitID", visitID);
                    cmdVisit.ExecuteNonQuery();

                    // 2. تحديث كل العبارات المرتبطة بالزيارة إلى محذوف
                    SqlCommand cmdPhrases = new SqlCommand("UPDATE Phrase SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE VisitID = @VisitID", con, transaction);
                    cmdPhrases.Parameters.AddWithValue("@VisitID", visitID);
                    cmdPhrases.ExecuteNonQuery();

                    // 3. تسجيل العملية في سجل النشاط
                    ActivityLogger.Log(con, transaction, "SoftDelete CityVisit", $"Soft-deleted city visit '{cityName}' (VisitID = {visitID})");

                    transaction.Commit(); // تأكيد التغييرات

                    // تحديث البيانات في الذاكرة
                    GlobalData.RefreshCityVisits(ActivityLogger.CurrentTripID);

                    MessageBox.Show("🗑️ City visit deleted successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // التراجع عن التغييرات في حالة خطأ
                    MessageBox.Show("❌ Error during delete: " + ex.Message);
                }
            }
        }

        // زر لإضافة زيارة مدينة جديدة
        private void btnAddCity_Click(object sender, EventArgs e)
        {
            NewCity newCityForm = new NewCity(); // فتح نموذج الإضافة
            if (newCityForm.ShowDialog() == DialogResult.OK)
            {
                LoadCities(); // إعادة تحميل البيانات بعد الإضافة
            }
        }
    }
}
