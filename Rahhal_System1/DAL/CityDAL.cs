// استدعاء المساحات التي تحتوي على الكلاسات المطلوبة للتعامل مع قاعدة البيانات والنماذج
using Rahhal_System1.DAL;
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.DAL
{
    // كلاس يحتوي على العمليات الخاصة بالمدن داخل قاعدة البيانات
    public class CityDAL
    {
        // دالة لجلب قائمة المدن التي تنتمي لدولة معينة (مع تجاهل المحذوفة) - ترجع كائنات City
        public static List<City> GetCitiesByCountry(int countryId)
        {
            List<City> cities = new List<City>();

            using (SqlConnection con = DbHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM City WHERE CountryID = @CountryID AND IsDeleted = 0", con))
                {
                    cmd.Parameters.AddWithValue("@CountryID", countryId);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime? updatedAt = null;
                            if (reader["UpdatedAt"] != DBNull.Value)
                                updatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

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

            return cities;
        }

        // دالة لتنفيذ حذف ناعم (Soft Delete) لمدينة حسب معرفها
        public static bool SoftDeleteCity(int cityId)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE City SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE CityID = @CityID", con))
                {
                    cmd.Parameters.AddWithValue("@CityID", cityId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // NEW دالة لإرجاع المدن كـ DataTable حسب الدولة
        public static DataTable GetCitiesByCountryTable(int countryId)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT CityID, CityName FROM City WHERE IsDeleted = 0 AND CountryID = @CountryID", con))
                {
                    cmd.Parameters.AddWithValue("@CountryID", countryId);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        // NEW دالة لجلب جميع المدن كـ DataTable
        public static DataTable GetAllCitiesTable()
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT CityID, CityName FROM City WHERE IsDeleted = 0", con))
                {
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        //هذه دالة لجلب معرف الدولة (CountryID) بناءً على معرف المدينة (CityID).
        public static int GetCountryIdByCityId(int cityId)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT CountryID FROM City WHERE CityID = @CityID", con);
                cmd.Parameters.AddWithValue("@CityID", cityId);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

    }
}
