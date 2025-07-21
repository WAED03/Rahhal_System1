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
using Rahhal_System1.Data;

namespace Rahhal_System1.Forms
{
    public partial class EventsLogForm : Form
    {
        public EventsLogForm()
        {
            InitializeComponent();
        }

        // متغير لمنع تكرار ربط أحداث التلوين
        private bool eventsCellPaintingAttached = false;

        // جدول لتخزين بيانات السجلات
        private DataTable activityLogTable;

        // عند تحميل الفورم
        private void EventsLogForm_Load(object sender, EventArgs e)
        {
            LoadActivityLog(); // تحميل السجلات
            LoadUsernames();   // تحميل أسماء المستخدمين في الكومبو بوكس
            dtpSearchDate.Checked = false; // إلغاء تفعيل التاريخ مبدئياً
            dgEventsLog.CellClick += dgEventsLog_CellClick; // ربط حدث النقر على الخلية
        }

        // تحميل بيانات سجل النشاطات
        private void LoadActivityLog()
        {
            string query = @"
                SELECT 
                    L.LogID, L.UserID, L.UserName, L.ActionType, L.ActionDetails, 
                    L.Timestamp, L.IsDeleted, 
                    ISNULL(U.IsDeleted, 0) AS UserIsDeleted
                FROM UserActivityLog L
                LEFT JOIN [User] U ON L.UserID = U.UserID
                ORDER BY L.Timestamp DESC";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                activityLogTable = new DataTable();
                adapter.Fill(activityLogTable); // تعبئة الجدول بالبيانات
                dgEventsLog.DataSource = activityLogTable; // ربط البيانات بالجريد
            }

            // إخفاء العمود المسؤول عن الحذف الناعم
            if (dgEventsLog.Columns.Contains("IsDeleted"))
                dgEventsLog.Columns["IsDeleted"].Visible = false;

            // إخفاء العمود الذي يحدد هل المستخدم محذوف
            if (dgEventsLog.Columns.Contains("UserIsDeleted"))
                dgEventsLog.Columns["UserIsDeleted"].Visible = false;

            // إضافة زر حذف داخل الجدول إذا لم يكن موجود
            if (!dgEventsLog.Columns.Contains("Delete"))
            {
                DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn();
                deleteButton.Name = "Delete";
                deleteButton.HeaderText = "";
                deleteButton.Text = "Delete";
                deleteButton.UseColumnTextForButtonValue = true;
                dgEventsLog.Columns.Add(deleteButton);
            }

            // ضبط حجم الأعمدة لملء الجدول
            dgEventsLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // ربط أحداث التلوين مرة واحدة فقط
            if (!eventsCellPaintingAttached)
            {
                dgEventsLog.CellPainting += DgEventsLog_CellPainting;
                dgEventsLog.RowPrePaint += DgEventsLog_RowPrePaint;
                eventsCellPaintingAttached = true;
            }
        }

        // تحميل أسماء المستخدمين داخل الكومبو بوكس
        private void LoadUsernames()
        {
            // تحديث القائمة من GlobalData إذا كانت فارغة
            if (GlobalData.UsersList == null || GlobalData.UsersList.Count == 0)
                GlobalData.RefreshUsers();

            cbSearchUser.Items.Clear();
            cbSearchUser.Items.Add("All"); // خيار لجميع المستخدمين

            // إضافة فقط المستخدمين غير المحذوفين
            foreach (var user in GlobalData.UsersList.Where(u => !u.IsDeleted))
            {
                cbSearchUser.Items.Add(user.UserName);
            }

            cbSearchUser.SelectedIndex = 0; // تحديد أول خيار
        }

        // تطبيق فلترة حسب المستخدم والتاريخ
        private void ApplySearchFilter()
        {
            if (activityLogTable == null) return;

            string userFilter = cbSearchUser.SelectedItem?.ToString();
            DateTime selectedDate = dtpSearchDate.Value.Date;
            DateTime nextDay = selectedDate.AddDays(1);

            string filterExpression = "";

            // فلترة حسب اسم المستخدم
            if (!string.IsNullOrEmpty(userFilter) && userFilter != "All")
                filterExpression += $"UserName = '{userFilter.Replace("'", "''")}'";

            // فلترة حسب التاريخ إذا كان مفعل
            if (dtpSearchDate.Checked)
            {
                if (filterExpression != "") filterExpression += " AND ";
                filterExpression += $"Timestamp >= #{selectedDate}# AND Timestamp < #{nextDay}#";
            }

            // تطبيق الفلترة على البيانات
            DataView dv = new DataView(activityLogTable);
            dv.RowFilter = filterExpression;
            dgEventsLog.DataSource = dv;
        }

        // زر البحث
        private void btnSearch_Click(object sender, EventArgs e)
        {
            ApplySearchFilter(); // تنفيذ الفلترة
        }

        // زر مسح الفلترة
        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            cbSearchUser.SelectedIndex = 0; // إعادة الكومبو بوكس إلى All
            dtpSearchDate.Value = DateTime.Now;
            dtpSearchDate.Checked = false;
            dgEventsLog.DataSource = activityLogTable; // إعادة كل البيانات
        }

        // تخصيص رسم زر الحذف داخل الجدول
        private void DgEventsLog_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // تحقق إذا كانت الخلية هي زر الحذف
            if (e.ColumnIndex == dgEventsLog.Columns["Delete"].Index)
            {
                e.PaintBackground(e.CellBounds, true);

                using (Brush brush = new SolidBrush(Color.IndianRed))
                {
                    e.Graphics.FillRectangle(brush, e.CellBounds); // خلفية حمراء
                }

                // رسم النص "Delete" بلون أبيض ومركزي
                TextRenderer.DrawText(e.Graphics, "Delete", e.CellStyle.Font, e.CellBounds, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true; // منع النظام من إعادة الرسم
            }
        }

        // تلوين الصف قبل عرضه (لتغيير لون أو خط الصف)
        private void DgEventsLog_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgEventsLog.Rows[e.RowIndex];

            bool isUserDeleted = false;
            bool isLogDeleted = false;

            // التأكد إن المستخدم محذوف
            if (row.Cells["UserIsDeleted"].Value != DBNull.Value)
                isUserDeleted = Convert.ToBoolean(row.Cells["UserIsDeleted"].Value);

            // التأكد إن السجل محذوف
            if (row.Cells["IsDeleted"].Value != DBNull.Value)
                isLogDeleted = Convert.ToBoolean(row.Cells["IsDeleted"].Value);

            // إذا كان محذوف، يتم تغيير لون وخط الصف
            if (isUserDeleted || isLogDeleted)
            {
                row.DefaultCellStyle.ForeColor = Color.Gray;
                row.DefaultCellStyle.Font = new Font(dgEventsLog.Font, FontStyle.Strikeout);
            }
            else
            {
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.Font = new Font(dgEventsLog.Font, FontStyle.Regular);
            }

            // إظهار حالة الحذف في Tooltip
            row.Cells["UserName"].ToolTipText = isUserDeleted ? "❌ User is deleted" : "";
            row.Cells["ActionDetails"].ToolTipText = isLogDeleted ? "🗑️ Log is soft deleted" : "";
        }

        // عند النقر على خلية داخل الجدول
        private void dgEventsLog_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            // إذا كانت الخلية هي زر الحذف
            if (dgEventsLog.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (dgEventsLog.Rows[e.RowIndex].Cells["LogID"].Value == null)
                    return;

                int logId = Convert.ToInt32(dgEventsLog.Rows[e.RowIndex].Cells["LogID"].Value);

                // تأكيد الحذف من المستخدم
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this record? ⚠️",
                    "Confirm deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        SoftDeleteLogEntry(logId); // تنفيذ الحذف
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("❌ An error occurred while attempting to delete:\n\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // تنفيذ الحذف الناعم للسجل (تحديث الحقل فقط)
        private void SoftDeleteLogEntry(int logId)
        {
            string query = @"
                UPDATE UserActivityLog
                SET IsDeleted = 1, UpdatedAt = @updatedAt
                WHERE LogID = @id";

            using (SqlConnection con = DbHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@id", logId);
                cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                con.Open();
                cmd.ExecuteNonQuery(); // تنفيذ أمر التحديث
            }

            LoadActivityLog(); // إعادة تحميل البيانات بعد الحذف
        }
    }
}
