// استدعاء المساحات التي تحتوي على الكلاسات المطلوبة للتعامل مع قاعدة البيانات والنماذج
using Rahhal_System1.DAL;
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.DAL
{
    // كلاس يحتوي على العمليات الخاصة بالمدن داخل قاعدة البيانات
    public class CityDAL
    {
        // دالة لجلب قائمة المدن التي تنتمي لدولة معينة (مع تجاهل المحذوفة)
        public static List<City> GetCitiesByCountry(int countryId)
        {
            // إنشاء قائمة لتخزين المدن المسترجعة من قاعدة البيانات
            List<City> cities = new List<City>();

            // إنشاء اتصال بقاعدة البيانات باستخدام DbHelper
            using (SqlConnection con = DbHelper.GetConnection())
            {
                // تجهيز الاستعلام لجلب المدن حسب رقم الدولة مع تجاهل المدن المحذوفة (IsDeleted = 0)
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM City WHERE CountryID = @CountryID AND IsDeleted = 0", con))
                {
                    // تمرير قيمة معرف الدولة إلى الاستعلام كمعامل
                    cmd.Parameters.AddWithValue("@CountryID", countryId);

                    // فتح الاتصال بقاعدة البيانات
                    con.Open();

                    // تنفيذ الاستعلام وقراءة النتائج
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // التكرار على كل صف تم استرجاعه من قاعدة البيانات
                        while (reader.Read())
                        {
                            // قراءة التاريخ المحدث إن وجد (قد يكون فارغ)
                            DateTime? updatedAt = null;
                            if (reader["UpdatedAt"] != DBNull.Value)
                                updatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            // إنشاء كائن جديد من نوع City وتعبئة خصائصه من البيانات المقروءة
                            cities.Add(new City
                            {
                                CityID = Convert.ToInt32(reader["CityID"]),
                                CountryID = Convert.ToInt32(reader["CountryID"]),
                                CityName = reader["CityName"].ToString(),
                                IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                                UpdatedAt = updatedAt
                            });
                        }
                    }
                }
            }

            // إرجاع قائمة المدن المسترجعة
            return cities;
        }

        // دالة لتنفيذ حذف ناعم (Soft Delete) لمدينة حسب معرفها
        public static bool SoftDeleteCity(int cityId)
        {
            // إنشاء اتصال بقاعدة البيانات
            using (SqlConnection con = DbHelper.GetConnection())
            {
                // تجهيز استعلام التحديث لتغيير حالة المدينة إلى محذوفة وتحديث تاريخ التحديث
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE City SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE CityID = @CityID", con))
                {
                    // تمرير قيمة معرف المدينة إلى الاستعلام
                    cmd.Parameters.AddWithValue("@CityID", cityId);

                    // تمرير التاريخ الحالي لتحديثه في الحقل UpdatedAt
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    // فتح الاتصال وتنفيذ الاستعلام
                    con.Open();

                    // إرجاع true إذا تم التحديث بنجاح (أي تأثر صف واحد على الأقل)
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
