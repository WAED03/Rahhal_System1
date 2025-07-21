// استدعاء المساحات المطلوبة (Namespaces)
using Rahhal_System1.DAL; // للوصول إلى كلاس DbHelper أو غيره من ملفات DAL
using Rahhal_System1.Models; // للوصول إلى كلاس Country
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rahhal_System1.DAL
{
    public class CountryDAL
    {
        // دالة لإحضار جميع الدول غير المحذوفة (IsDeleted = 0)
        public static List<Country> GetAllCountries()
        {
            List<Country> countries = new List<Country>(); // إنشاء قائمة لتخزين النتائج

            // إنشاء اتصال بقاعدة البيانات
            using (SqlConnection con = DbHelper.GetConnection())
            {
                // إنشاء أمر SQL لاختيار جميع الدول غير المحذوفة
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Country WHERE IsDeleted = 0", con))
                {
                    con.Open(); // فتح الاتصال

                    // تنفيذ الأمر وجلب النتائج باستخدام SqlDataReader
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) // التكرار على كل صف في النتيجة
                        {
                            DateTime? updatedAt = null;

                            // التحقق إذا كانت القيمة في العمود UpdatedAt غير فارغة
                            if (reader["UpdatedAt"] != DBNull.Value)
                                updatedAt = Convert.ToDateTime(reader["UpdatedAt"]); // تحويلها إلى نوع DateTime

                            // إضافة كائن Country جديد إلى القائمة
                            countries.Add(new Country
                            {
                                CountryID = Convert.ToInt32(reader["CountryID"]), // رقم الدولة
                                CountryName = reader["CountryName"].ToString(),   // اسم الدولة
                                Continent = reader["Continent"].ToString(),       // القارة
                                IsDeleted = Convert.ToBoolean(reader["IsDeleted"]), // حالة الحذف
                                UpdatedAt = updatedAt // تاريخ آخر تعديل (إذا وُجد)
                            });
                        }
                    }
                }
            }

            return countries; // إرجاع القائمة بعد تعبئتها
        }

        // دالة لحذف دولة حذفًا ناعمًا (تحديث العمود IsDeleted إلى 1 بدلاً من حذف السجل فعليًا)
        public static bool SoftDeleteCountry(int countryId)
        {
            // إنشاء اتصال بقاعدة البيانات
            using (SqlConnection con = DbHelper.GetConnection())
            {
                // أمر SQL لتحديث السجل وجعل IsDeleted = 1 وتحديث الوقت
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Country SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE CountryID = @CountryID", con))
                {
                    // تمرير القيم للباراميترات
                    cmd.Parameters.AddWithValue("@CountryID", countryId); // رقم الدولة
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now); // وقت التحديث الحالي

                    con.Open(); // فتح الاتصال
                    return cmd.ExecuteNonQuery() > 0; // تنفيذ الأمر وإرجاع true إذا تم تحديث صف واحد على الأقل
                }
            }
        }
    }
}
