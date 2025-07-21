// استدعاء المساحات الاسمية اللازمة
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json; // لاستخدام تحويل JSON إلى كائنات والعكس
using System.Net.Http; // لإجراء طلبات HTTP
using Rahhal_System1.Models; // لاستيراد الكلاسات من المشروع (مثل AboutData)

namespace Rahhal_System1.UC
{
    // تعريف عنصر تحكم مخصص من نوع UserControl لعرض معلومات "حول النظام"
    public partial class AboutUC : UserControl
    {
        // المُنشئ الخاص بالعنصر، يتم استدعاؤه عند إنشائه
        public AboutUC()
        {
            InitializeComponent(); // تهيئة مكونات الواجهة
        }

        // دالة غير متزامنة (async) تقوم بجلب بيانات "حول النظام" من API خارجي
        public async Task<AboutData> GetAboutContentAsync()
        {
            // إنشاء كائن من HttpClient لإجراء الاتصال
            using (HttpClient client = new HttpClient())
            {
                // تحديد رابط API الذي يحتوي على بيانات حول النظام
                string url = "http://dev2.alashiq.com/about.php";

                // إرسال الطلب وجلب النص JSON كـ string
                var response = await client.GetStringAsync(url);

                // تحويل البيانات النصية (JSON) إلى كائن من نوع AboutApiResponse
                var result = JsonConvert.DeserializeObject<AboutApiResponse>(response);

                // إرجاع بيانات "حول" الفعلية
                return result.data;
            }
        }

        // الحدث الذي يتم تنفيذه عند تحميل عنصر التحكم (UserControl)
        private async void AboutUC_Load(object sender, EventArgs e)
        {
            try
            {
                // انتظار تحميل البيانات من الإنترنت
                var aboutData = await GetAboutContentAsync();

                // عرض البيانات في العناصر المخصصة لها على الواجهة
                lblID.Text = aboutData.id.ToString();
                lblTitle.Text = aboutData.title;
                lblDescription.Text = aboutData.description;
                lblSystem_version.Text = aboutData.system_version;
                lblCreated_at.Text = aboutData.created_at;
                lblUpdated_at.Text = aboutData.updated_at;
            }
            catch (Exception ex)
            {
                // في حالة حدوث خطأ أثناء الاتصال أو الجلب، يتم عرض رسالة خطأ
                MessageBox.Show("Failed to fetch system data : " + ex.Message);
            }
        }
    }
}
