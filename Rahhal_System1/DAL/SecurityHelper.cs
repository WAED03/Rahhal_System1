using System;                         // توفر أنواع أساسية مثل String وException
using System.Collections.Generic;     // توفر أنواع القوائم والمجموعات
using System.Linq;                    // توفر وظائف LINQ للاستعلام عن البيانات
using System.Text;                    // توفر أنواع لترميز وتحويل النصوص
using System.Threading.Tasks;         // تدعم البرمجة غير المتزامنة
using System.Security.Cryptography;   // توفر وظائف التشفير مثل SHA256

// تعريف مساحة الأسماء الخاصة بطبقة الوصول إلى البيانات (Data Access Layer)
namespace Rahhal_System1.DAL
{
    // تعريف كلاس ثابت يحتوي على وظائف أمنية (مثل التشفير)
    public static class SecurityHelper
    {
        // دالة تقوم بتحويل أي نص إلى قيمة مشفرة باستخدام خوارزمية SHA256
        public static string HashSHA256(string input)
        {
            // إنشاء كائن من نوع SHA256 داخل كتلة using لضمان التخلص منه بعد الاستخدام
            using (SHA256 sha256 = SHA256.Create())
            {
                // تحويل النص إلى مصفوفة بايت باستخدام UTF8 ثم حساب القيمة المشفرة
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // إنشاء StringBuilder لتجميع الناتج النصي النهائي
                StringBuilder sb = new StringBuilder();

                // تحويل كل بايت إلى تمثيله الست عشري (Hex) وإضافته للنص
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2")); // x2 تعني تمثيل من رقمين في النظام الست عشري

                // إرجاع النص النهائي المشفر
                return sb.ToString();
            }
        }
    }
}
