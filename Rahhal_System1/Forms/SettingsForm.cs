using Rahhal_System1.DAL; // استدعاء طبقة الوصول إلى البيانات
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rahhal_System1.Forms
{
    public partial class SettingsForm : Form
    {
        private string currentUser;
        private UserRole currentRole; // متغيرات لتخزين اسم المستخدم والدور الخاص به

        // المُنشئ الذي يستقبل اسم المستخدم ودوره
        public SettingsForm(string user, UserRole role)
        {
            InitializeComponent(); // تهيئة مكونات الفورم

            currentUser = user; // حفظ اسم المستخدم
            currentRole = role; // حفظ دور المستخدم

            // إذا كان المستخدم عادي (ليس Admin)، يتم إخفاء بعض الأزرار المتعلقة بالصلاحيات الإدارية
            if (role == UserRole.Regular)
            {
                btnViewUsers.Visible = false;       // إخفاء زر عرض المستخدمين
                btnUsersMessages.Visible = false;   // إخفاء زر رسائل المستخدمين
                btnEventsLog.Visible = false;       // إخفاء زر سجل الأحداث
            }
        }

        // عند الضغط على زر "سجل الأحداث"
        private void btnEventsLog_Click(object sender, EventArgs e)
        {
            new EventsLogForm().ShowDialog(); // فتح نموذج سجل الأحداث كنافذة حوارية
        }

        // عند الضغط على زر "عرض المستخدمين"
        private void btnViewUsers_Click(object sender, EventArgs e)
        {
            new ViewUsersForm().ShowDialog(); // فتح نموذج عرض المستخدمين
        }

        // عند الضغط على زر "رسائل المستخدمين"
        private void btnUsersMessages_Click(object sender, EventArgs e)
        {
            new UsersMessagesForm().ShowDialog(); // فتح نموذج رسائل المستخدمين
        }

        // عند الضغط على زر الوضع (الذي لم يتم تنفيذه بعد)
        private void btnMode_Click(object sender, EventArgs e)
        {
            // عرض رسالة تفيد بأن الميزة غير مفعلة بعد
            MessageBox.Show("This feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // عند الضغط على زر "تبديل الحساب"
        private void btnSwitchAccount_Click(object sender, EventArgs e)
        {
            // عرض رسالة تأكيد للمستخدم
            DialogResult result = MessageBox.Show(
               "Are you sure you want to switch account?", // نص الرسالة
                "Confirm account switch",                  // عنوان الرسالة
                MessageBoxButtons.YesNo,                   // أزرار نعم/لا
                MessageBoxIcon.Question                    // أيقونة سؤال
            );

            // إذا اختار المستخدم "نعم"
            if (result == DialogResult.Yes)
            {
                // إخفاء جميع الفورمز المفتوحة ما عدا الفورم الحالي
                foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                {
                    if (form != this)
                        form.Hide(); // إخفاء الفورم
                }

                // لا داعي لغلق الاتصال بقاعدة البيانات هنا لأن الطبقة المسؤولة تتعامل مع الاتصالات بشكل آمن

                this.Hide(); // إخفاء فورم الإعدادات الحالي

                // فتح فورم تسجيل الدخول مرة أخرى
                loginForm loginForm = new loginForm();
                loginForm.Show();
            }
            // إذا اختار "لا"، لا يتم تنفيذ أي إجراء
        }
    }
}
