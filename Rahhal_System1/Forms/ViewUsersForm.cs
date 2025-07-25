using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rahhal_System1.Data;      // لاستدعاء البيانات العامة (مثل قائمة المستخدمين)
using Rahhal_System1.Models;    // لاستدعاء نموذج المستخدم User

namespace Rahhal_System1.Forms
{
    public partial class ViewUsersForm : Form
    {
        public ViewUsersForm()
        {
            InitializeComponent();

            // ربط الأحداث عند تحميل الفورم أو التفاعل مع DataGridView
            this.Load += ViewUsersForm_Load;
            this.dgViewUsers.CellContentClick += dgViewUsers_CellContentClick;
            this.dgViewUsers.RowPrePaint += dgViewUsers_RowPrePaint;
        }

        // عند تحميل الفورم يتم تحميل المستخدمين في الجدول
        private void ViewUsersForm_Load(object sender, EventArgs e)
        {
            LoadUsersToGrid();
        }

        // تحميل المستخدمين في DataGridView
        private void LoadUsersToGrid()
        {
            // جلب جميع المستخدمين (بما فيهم المحذوفين منطقيًا)
            GlobalData.RefreshUsers();

            dgViewUsers.DataSource = null;
            dgViewUsers.AutoGenerateColumns = true;
            dgViewUsers.DataSource = GlobalData.UsersList;

            // إخفاء بعض الأعمدة غير الضرورية
            dgViewUsers.Columns["Password"].Visible = false;
            dgViewUsers.Columns["IsDeleted"].Visible = false;

            // تغيير رؤوس الأعمدة لتكون أوضح للمستخدم
            dgViewUsers.Columns["UserID"].HeaderText = "ID";
            dgViewUsers.Columns["UserName"].HeaderText = "Username";
            dgViewUsers.Columns["Email"].HeaderText = "Email";
            dgViewUsers.Columns["JoinDate"].HeaderText = "Join Date";
            dgViewUsers.Columns["Role"].HeaderText = "Role";
            dgViewUsers.Columns["FailedAttempts"].HeaderText = "Failed Attempts";
            dgViewUsers.Columns["LastAttempt"].HeaderText = "Last Attempt";
            dgViewUsers.Columns["UpdatedAt"].HeaderText = "Last Update";

            // حذف العمود النصي القديم إن وجد
            if (dgViewUsers.Columns.Contains("IsDeletedText"))
                dgViewUsers.Columns.Remove("IsDeletedText");

            // إنشاء عمود جديد يعرض إذا ما كان المستخدم محذوفًا بطريقة نصية (Yes/No)
            DataGridViewTextBoxColumn deletedTextCol = new DataGridViewTextBoxColumn();
            deletedTextCol.Name = "IsDeletedText";
            deletedTextCol.HeaderText = "Deleted?";
            deletedTextCol.ReadOnly = true;

            // إدراج العمود في موقع معين قبل آخر عمود (UpdatedAt)
            int insertPos = dgViewUsers.Columns["UpdatedAt"].Index;
            dgViewUsers.Columns.Insert(insertPos, deletedTextCol);

            // تعبئة عمود "Deleted?" بالقيم المناسبة لكل مستخدم
            foreach (DataGridViewRow row in dgViewUsers.Rows)
            {
                if (row.DataBoundItem is User user)
                {
                    row.Cells["IsDeletedText"].Value = user.IsDeleted ? "Yes" : "No";
                }
            }

            // إضافة أعمدة الأزرار (تعديل، حذف)
            AddActionButtons();
        }

        // إضافة أعمدة أزرار التعديل والحذف إلى DataGridView
        private void AddActionButtons()
        {
            // التحقق من عدم وجود الأعمدة مسبقًا
            if (!dgViewUsers.Columns.Contains("Delete"))
            {

                // زر الحذف
                DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn();
                deleteButton.Name = "Delete";
                deleteButton.HeaderText = "";
                deleteButton.Text = "Delete";
                deleteButton.UseColumnTextForButtonValue = true;
                deleteButton.DefaultCellStyle.BackColor = Color.LightCoral;

                // إضافة الأزرار إلى الجدول
                dgViewUsers.Columns.Add(deleteButton);
            }

            // تخصيص شكل الأزرار داخل الخلايا
            dgViewUsers.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    // تخصيص زر الحذف
                    if (e.ColumnIndex == dgViewUsers.Columns["Delete"].Index)
                    {
                        e.PaintBackground(e.CellBounds, true);
                        using (Brush brush = new SolidBrush(Color.IndianRed))
                        {
                            e.Graphics.FillRectangle(brush, e.CellBounds);
                        }
                        TextRenderer.DrawText(e.Graphics, "Delete", e.CellStyle.Font, e.CellBounds, Color.White,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        e.Handled = true;
                    }
                }
            };
        }

        // حدث عند الضغط على زر داخل DataGridView
        private void dgViewUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedUser = (User)dgViewUsers.Rows[e.RowIndex].DataBoundItem;

                // عند الضغط على زر "Delete"
                if (dgViewUsers.Columns[e.ColumnIndex].Name == "Delete")
                {
                    var confirm = MessageBox.Show($"❌ Are you sure you want to delete {selectedUser.UserName}?",
                                                  "Confirm Delete", MessageBoxButtons.YesNo);
                    if (confirm == DialogResult.Yes)
                    {
                        // حذف منطقي (soft delete)
                        UserDAL.SoftDeleteUser(selectedUser.UserID);
                        LoadUsersToGrid(); // تحديث الجدول بعد الحذف
                    }
                }
            }
        }

        // تغيير ألوان الصف إذا كان المستخدم محذوف
        private void dgViewUsers_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgViewUsers.Rows[e.RowIndex];
            if (row.DataBoundItem is User user && user.IsDeleted)
            {
                row.DefaultCellStyle.BackColor = Color.LightGray;
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
            }
            else
            {
                // استعادة ألوان الصف العادية
                row.DefaultCellStyle.BackColor = dgViewUsers.DefaultCellStyle.BackColor;
                row.DefaultCellStyle.ForeColor = dgViewUsers.DefaultCellStyle.ForeColor;
            }
        }

        // زر التحديث اليدوي لواجهة المستخدمين
        private void btnRefreshUsers_Click(object sender, EventArgs e)
        {
            LoadUsersToGrid(); // إعادة تحميل المستخدمين في الجدول
        }
    }
}
