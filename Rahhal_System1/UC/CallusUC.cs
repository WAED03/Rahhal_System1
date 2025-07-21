using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;                  // مكتبة لتحويل الكائنات إلى JSON والعكس
using System.Net.Http;                 // لإرسال الطلبات عبر HTTP
using Rahhal_System1.Models;          // استدعاء النماذج (Models) من المشروع
using Rahhal_System1.DAL;             // استدعاء طبقة الوصول إلى البيانات
using Newtonsoft.Json.Linq;           // مكتبة لمعالجة كائنات JSON بشكل ديناميكي

namespace Rahhal_System1.UC
{
    // عنصر واجهة المستخدم المسؤول عن صفحة "اتصل بنا"
    public partial class CallusUC : UserControl
    {
        // مُنشئ الصفحة
        public CallusUC()
        {
            InitializeComponent(); // تهيئة مكونات الواجهة
        }

        // ✅ دالة لإرسال رسالة بشكل غير متزامن (Async)
        public async Task<string> SendMessageAsync(MessageModel msg)
        {
            using (HttpClient client = new HttpClient()) // إنشاء كائن لإرسال طلبات HTTP
            {
                // رابط الـ API الذي سيتم إرسال الرسالة إليه
                string url = "http://dev2.alashiq.com/send.php?systemId=98123817126661234";

                // تحويل الرسالة إلى صيغة JSON
                string json = JsonConvert.SerializeObject(msg);

                // إنشاء محتوى الطلب من النوع JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // إرسال الطلب وانتظار الرد من الخادم
                HttpResponseMessage response = await client.PostAsync(url, content);

                // قراءة الرد كسلسلة نصية
                string result = await response.Content.ReadAsStringAsync();

                // تحليل الرد واستخراج فقط الرسالة من كائن JSON
                var obj = JObject.Parse(result);
                string message = obj["message"]?.ToString(); // علامة الاستفهام تمنع الخطأ إذا كانت null

                return message; // إرجاع نص الرسالة
            }
        }

        // ✅ حدث الضغط على زر "إرسال"
        private async void btnSend_Click(object sender, EventArgs e)
        {
            // التحقق من تسجيل الدخول
            if (ActivityLogger.CurrentUser == null)
            {
                MessageBox.Show("Please log in first."); // تنبيه للمستخدم
                return;
            }

            // التحقق من أن حقل الرسالة غير فارغ
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                MessageBox.Show("The message cannot be empty."); // رسالة تنبيه إذا كانت الرسالة فارغة
                return;
            }

            // إنشاء كائن يمثل الرسالة لاحتوائه على بيانات المستخدم والرسالة
            var msg = new MessageModel
            {
                user_id = ActivityLogger.CurrentUser.UserID,
                username = ActivityLogger.CurrentUser.UserName,
                message = txtMessage.Text
            };

            // إرسال الرسالة إلى الخادم واستلام الرد
            string serverMessage = await SendMessageAsync(msg);

            // عرض رسالة تأكيد بعد الإرسال
            MessageBox.Show(serverMessage, "Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // تفريغ حقل الرسالة بعد الإرسال
            txtMessage.Clear();
        }
    }
}
