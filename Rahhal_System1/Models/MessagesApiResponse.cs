// استيراد النيمسبيس الذي يحتوي على تعريف الكلاس MessageModel
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Models
{
    // هذا الكلاس يمثل استجابة واجهة برمجة التطبيقات (API) الخاصة بالرسائل
    public class MessagesApiResponse
    {
        // متغير يدل على نجاح أو فشل الطلب
        public bool success { get; set; }

        // رسالة توضيحية من السيرفر (نجح - فشل - سبب الخطأ... إلخ)
        public string message { get; set; }

        // البيانات الفعلية التي تم إرجاعها من السيرفر (رسائل المستخدمين)
        public MessageData data { get; set; } // ✅ ملاحظة: هذا كائن مفرد وليس List!
    }

    // هذا الكلاس يحتوي على تفاصيل الرسائل والعدد الإجمالي لها
    public class MessageData
    {
        // قائمة بجميع الرسائل التي تم استرجاعها
        public List<MessageModel> messages { get; set; }

        // العدد الإجمالي للرسائل (يمكن استخدامه للصفحات أو الإحصائيات)
        public int total_count { get; set; }
    }
}
