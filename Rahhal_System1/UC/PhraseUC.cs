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
using Rahhal_System1.DAL;       // للوصول إلى طبقة البيانات (Data Access Layer)
using Rahhal_System1.Forms;     // للوصول إلى النماذج (Forms)
using Rahhal_System1.Models;    // للوصول إلى الكلاسات (Models)
using Rahhal_System1.Data;      // للوصول إلى البيانات العامة (GlobalData)

namespace Rahhal_System1.UC
{
    public partial class PhraseUC : UserControl
    {
        public PhraseUC()
        {
            InitializeComponent();
            LoadPhrases(); // تحميل قائمة العبارات عند تحميل الواجهة
        }

        // عند الضغط على زر "إضافة عبارة جديدة"
        private void btnAddPhrase_Click(object sender, EventArgs e)
        {
            NewWord newPhraseForm = new NewWord(); // إنشاء نموذج إضافة عبارة
            var result = newPhraseForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                LoadPhrases(); // إعادة تحميل العبارات بعد الإضافة
            }
        }

        // تحميل العبارات من قاعدة البيانات وعرضها في DataGridView
        private void LoadPhrases()
        {
            // تحديث العبارات من قاعدة البيانات للمستخدم الحالي
            GlobalData.RefreshPhrases(ActivityLogger.CurrentUser.UserID);

            // تجهيز البيانات لعرضها في الجدول
            var phrasesData = GlobalData.PhrasesList.Select(p => new
            {
                p.PhraseID,
                TripName = p.Visit.Trip.TripName,      // اسم الرحلة المرتبطة
                CityName = p.Visit.City.CityName,      // اسم المدينة المرتبطة
                p.OriginalText,                         // النص الأصلي
                p.Translation,                          // الترجمة
                p.Language,                             // اللغة
                p.Notes                                 // ملاحظات
            }).ToList();

            dgPhrase.DataSource = phrasesData; // عرض البيانات في DataGridView

            // تغيير أسماء الأعمدة الظاهرة للمستخدم
            if (dgPhrase.Columns.Count > 0)
            {
                dgPhrase.Columns["PhraseID"].HeaderText = "ID";
                dgPhrase.Columns["TripName"].HeaderText = "Trip";
                dgPhrase.Columns["CityName"].HeaderText = "City";
                dgPhrase.Columns["OriginalText"].HeaderText = "Original";
                dgPhrase.Columns["Translation"].HeaderText = "Translation";
                dgPhrase.Columns["Language"].HeaderText = "Language";
                dgPhrase.Columns["Notes"].HeaderText = "Notes";
            }

            // إضافة أعمدة أزرار "Edit" و "Delete" إذا لم تكن موجودة مسبقاً
            if (dgPhrase.Columns["Edit"] == null && dgPhrase.Columns["Delete"] == null)
            {
                var editButton = new DataGridViewButtonColumn
                {
                    Name = "Edit",
                    HeaderText = "",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true
                };
                dgPhrase.Columns.Add(editButton);

                var deleteButton = new DataGridViewButtonColumn
                {
                    Name = "Delete",
                    HeaderText = "",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true
                };
                dgPhrase.Columns.Add(deleteButton);

                // تنسيق الأزرار بالألوان داخل الخلايا
                dgPhrase.CellPainting += (s, e) =>
                {
                    if (e.RowIndex < 0) return; // تجاهل رأس الجدول

                    if (e.ColumnIndex == dgPhrase.Columns["Edit"].Index)
                    {
                        e.PaintBackground(e.CellBounds, true);
                        using (Brush brush = new SolidBrush(Color.DodgerBlue))
                            e.Graphics.FillRectangle(brush, e.CellBounds);

                        TextRenderer.DrawText(e.Graphics, "Edit", e.CellStyle.Font, e.CellBounds, Color.White,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        e.Handled = true;
                    }
                    else if (e.ColumnIndex == dgPhrase.Columns["Delete"].Index)
                    {
                        e.PaintBackground(e.CellBounds, true);
                        using (Brush brush = new SolidBrush(Color.IndianRed))
                            e.Graphics.FillRectangle(brush, e.CellBounds);

                        TextRenderer.DrawText(e.Graphics, "Delete", e.CellStyle.Font, e.CellBounds, Color.White,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        e.Handled = true;
                    }
                };
            }
        }

        // عند الضغط على زر داخل الجدول (Edit أو Delete)
        private void dgPhrase_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // تجاهل الرأس

            int phraseID = Convert.ToInt32(dgPhrase.Rows[e.RowIndex].Cells["PhraseID"].Value);
            string original = dgPhrase.Rows[e.RowIndex].Cells["OriginalText"].ToString();

            if (dgPhrase.Columns[e.ColumnIndex].Name == "Delete")
            {
                // تأكيد الحذف
                var confirm = MessageBox.Show($"Are you sure you want to delete phrase: \"{original}\"?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    SoftDeletePhrase(phraseID, original); // تنفيذ الحذف
                    LoadPhrases(); // تحديث الجدول بعد الحذف
                }
            }
            else if (dgPhrase.Columns[e.ColumnIndex].Name == "Edit")
            {
                // فتح النموذج لتعديل العبارة
                NewWord editForm = new NewWord(phraseID);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadPhrases(); // تحديث الجدول بعد التعديل
                }
            }
        }

        // تنفيذ الحذف الناعم (Soft Delete) للعبارة
        private void SoftDeletePhrase(int phraseID, string original)
        {
            if (PhraseDAL.SoftDeletePhrase(phraseID))
            {
                // ✅ تحديث القائمة من قاعدة البيانات
                GlobalData.RefreshPhrases(ActivityLogger.CurrentVisitID);

                // ✅ تسجيل عملية الحذف في سجل النشاطات
                using (var con = DbHelper.GetConnection())
                {
                    con.Open();
                    ActivityLogger.Log(con, "SoftDelete Phrase", $"Deleted phrase '{original}' (ID = {phraseID})");
                }

                MessageBox.Show("🗑️ Phrase deleted successfully.");
            }
            else
            {
                MessageBox.Show("❌ Failed to delete phrase.");
            }
        }
    }
}
