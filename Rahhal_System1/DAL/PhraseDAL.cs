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
    // كلاس مسؤول عن العمليات الخاصة بجمل الترجمة (Phrase) من قاعدة البيانات
    public class PhraseDAL
    {
        // دالة لاسترجاع جميع الجمل المرتبطة بمستخدم معين (حسب الرحلات)
        public static List<Phrase> GetPhrasesByUser(int userId)
        {
            var phrases = new List<Phrase>(); // قائمة لتخزين الجمل المسترجعة

            using (SqlConnection con = DbHelper.GetConnection()) // إنشاء الاتصال بقاعدة البيانات
            {
                con.Open(); // فتح الاتصال

                // إنشاء أمر SQL لاسترجاع بيانات الجمل مع معلومات الزيارة والمدينة والدولة والرحلة
                SqlCommand cmd = new SqlCommand(@"
            SELECT p.PhraseID, p.VisitID, p.OriginalText, p.Translation, p.Language, p.Notes, p.IsDeleted, p.UpdatedAt,
                   v.TripID, v.CityID, v.VisitDate, v.Rating, v.Notes as VisitNotes, v.IsDeleted as VisitIsDeleted, v.UpdatedAt as VisitUpdatedAt,
                   c.CityName, c.CountryID,
                   co.CountryName,
                   t.TripName, t.UserID
            FROM Phrase p
            INNER JOIN CityVisit v ON p.VisitID = v.VisitID
            INNER JOIN City c ON v.CityID = c.CityID
            INNER JOIN Country co ON c.CountryID = co.CountryID
            INNER JOIN Trip t ON v.TripID = t.TripID
            WHERE p.IsDeleted = 0 AND t.UserID = @UserID", con); // فقط الجمل غير المحذوفة والمرتبطة بالمستخدم

                // تمرير معرف المستخدم كـ parameter للأمان ضد SQL Injection
                cmd.Parameters.AddWithValue("@UserID", userId);

                // تنفيذ الأمر وقراءة النتائج
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) // التكرار على كل سجل
                    {
                        // إنشاء كائن الدولة
                        var country = new Country
                        {
                            CountryID = Convert.ToInt32(reader["CountryID"]),
                            CountryName = reader["CountryName"].ToString()
                        };

                        // إنشاء كائن المدينة
                        var city = new City
                        {
                            CityID = Convert.ToInt32(reader["CityID"]),
                            CityName = reader["CityName"].ToString(),
                            CountryID = country.CountryID,
                            Country = country
                        };

                        // إنشاء كائن الرحلة
                        var trip = new Trip
                        {
                            TripID = Convert.ToInt32(reader["TripID"]),
                            TripName = reader["TripName"].ToString(),
                            UserID = Convert.ToInt32(reader["UserID"])
                        };

                        // إنشاء كائن الزيارة
                        var visit = new CityVisit
                        {
                            VisitID = Convert.ToInt32(reader["VisitID"]),
                            TripID = trip.TripID,
                            CityID = city.CityID,
                            VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                            Rating = reader["Rating"].ToString(),
                            Notes = reader["VisitNotes"].ToString(),
                            IsDeleted = Convert.ToBoolean(reader["VisitIsDeleted"]),
                            UpdatedAt = reader["VisitUpdatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VisitUpdatedAt"]),
                            City = city,
                            Trip = trip
                        };

                        // إضافة الجملة إلى القائمة
                        phrases.Add(new Phrase
                        {
                            PhraseID = Convert.ToInt32(reader["PhraseID"]),
                            VisitID = visit.VisitID,
                            OriginalText = reader["OriginalText"].ToString(),
                            Translation = reader["Translation"].ToString(),
                            Language = reader["Language"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                            UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"]),
                            Visit = visit
                        });
                    }
                }
            }

            return phrases; // إرجاع قائمة الجمل
        }

        // دالة لتنفيذ الحذف الناعم (Soft Delete) لجملة معينة
        public static bool SoftDeletePhrase(int phraseID)
        {
            using (SqlConnection con = DbHelper.GetConnection()) // فتح الاتصال بقاعدة البيانات
            {
                // إعداد أمر SQL لتحديث الجملة وتعيين الحقل IsDeleted = 1 وتحديث التاريخ
                using (SqlCommand cmd = new SqlCommand("UPDATE Phrase SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE PhraseID = @PhraseID", con))
                {
                    cmd.Parameters.AddWithValue("@PhraseID", phraseID); // تمرير المعرف
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now); // تمرير التاريخ الحالي كوقت التحديث
                    con.Open(); // فتح الاتصال
                    return cmd.ExecuteNonQuery() > 0; // تنفيذ الأمر وإرجاع true إذا تم تعديل صف واحد على الأقل
                }
            }
        }
    }
}
