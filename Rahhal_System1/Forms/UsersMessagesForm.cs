using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;                      // لاستدعاء API خارجي باستخدام HttpClient
using Newtonsoft.Json;                    // لتحويل JSON إلى كائنات C#
using Rahhal_System1.Models;             // نماذج البيانات المستخدمة

namespace Rahhal_System1.Forms
{
    public partial class UsersMessagesForm : Form
    {
        // قائمة لتخزين كل الرسائل القادمة من API
        private List<MessageModel> allMessages = new List<MessageModel>();

        // مُنشئ الفورم
        public UsersMessagesForm()
        {
            InitializeComponent(); // تهيئة مكونات الفورم
        }

        // ✅ دالة غير متزامنة لجلب الرسائل من API
        public async Task<List<MessageModel>> GetUserMessagesAsync()
        {
            // إنشاء كائن HttpClient داخل using لضمان تحرير الموارد بعد الاستخدام
            using (HttpClient client = new HttpClient())
            {
                // رابط الـ API لجلب الرسائل
                string url = "http://dev2.alashiq.com/message.php?systemId=98123817126661234";

                // تنفيذ الطلب وانتظار النتيجة كسلسلة نصية
                var response = await client.GetStringAsync(url);

                // تحويل نتيجة الـ JSON إلى كائن من نوع MessagesApiResponse
                var result = JsonConvert.DeserializeObject<MessagesApiResponse>(response);

                // إعادة قائمة الرسائل من داخل كائن النتيجة
                return result.data.messages;
            }
        }

        // ✅ حدث تحميل الفورم (يُستدعى تلقائيًا عند فتح النموذج)
        private async void UsersMessagesForm_Load(object sender, EventArgs e)
        {
            try
            {
                // جلب الرسائل من API وتخزينها في القائمة
                allMessages = await GetUserMessagesAsync();

                // إعادة ضبط مصدر البيانات للـ DataGridView
                dgUsersMessages.DataSource = null;
                dgUsersMessages.DataSource = allMessages;

                // ✅ تعيين عناوين الأعمدة بشكل أوضح
                dgUsersMessages.Columns["user_id"].HeaderText = "User ID";
                dgUsersMessages.Columns["username"].HeaderText = "Username";
                dgUsersMessages.Columns["message"].HeaderText = "Message";
            }
            catch (Exception ex)
            {
                // عرض رسالة خطأ في حالة حدوث استثناء
                MessageBox.Show("An error occurred while loading messages: " + ex.Message);
            }
        }

        // ✅ زر الحذف: لحذف الرسالة المحددة من DataGridView
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // التأكد من أن هناك صفًا محددًا حاليًا
            if (dgUsersMessages.CurrentRow != null)
            {
                // الحصول على الكائن المرتبط بالصف المحدد وتحويله إلى MessageModel
                var selectedMessage = dgUsersMessages.CurrentRow.DataBoundItem as MessageModel;

                if (selectedMessage != null)
                {
                    // حذف الرسالة من القائمة
                    allMessages.Remove(selectedMessage);

                    // إعادة تحميل البيانات في DataGridView
                    dgUsersMessages.DataSource = null;
                    dgUsersMessages.DataSource = allMessages;

                    // عرض رسالة تأكيد بالحذف
                    MessageBox.Show("Message deleted from view successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                // تنبيه المستخدم في حال عدم تحديد أي صف
                MessageBox.Show("Please select a message first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
