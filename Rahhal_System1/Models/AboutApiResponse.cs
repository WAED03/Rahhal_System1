using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Models
{
    // هذا الكلاس يمثل الاستجابة الكاملة القادمة من الـ API الخاصة بقسم "من نحن"
    public class AboutApiResponse
    {
        // خاصية تشير إلى نجاح الطلب (true أو false)
        public bool success { get; set; }

        // رسالة توضيحية من الـ API (مثل: "تم بنجاح" أو "حدث خطأ")
        public string message { get; set; }

        // البيانات الفعلية التي تم إرجاعها من الـ API
        public AboutData data { get; set; }
    }

    // هذا الكلاس يمثل محتوى بيانات "من نحن" التي يتم جلبها من الـ API
    public class AboutData
    {
        // معرف السجل
        public int id { get; set; }

        // عنوان القسم (مثلاً: "عن النظام")
        public string title { get; set; }

        // الوصف التفصيلي للمحتوى
        public string description { get; set; }

        // إصدار النظام الحالي
        public string system_version { get; set; }

        // تاريخ إنشاء السجل
        public string created_at { get; set; }

        // تاريخ آخر تعديل على السجل
        public string updated_at { get; set; }
    }
}
