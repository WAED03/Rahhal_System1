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
    // كلاس خاص بالتعامل مع بيانات المستخدمين (Data Access Layer)
    public class UserDAL
    {
        // دالة لجلب جميع المستخدمين من قاعدة البيانات
        public static List<User> GetAllUsers()
        {
            // إنشاء قائمة لتخزين المستخدمين
            List<User> users = new List<User>();

            // فتح اتصال بقاعدة البيانات باستخدام دالة مساعدة
            using (SqlConnection con = DbHelper.GetConnection())
            {
                // إنشاء أمر SQL لاستدعاء جميع بيانات المستخدمين من جدول [User]
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [User] ", con))
                {
                    con.Open(); // فتح الاتصال

                    // تنفيذ الأمر والحصول على نتيجة القارئ (DataReader)
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // قراءة كل صف من النتائج
                        while (reader.Read())
                        {
                            // إضافة مستخدم جديد للقائمة مع تحويل البيانات من الصف الحالي
                            users.Add(new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),  // تحويل معرف المستخدم إلى int
                                UserName = reader["UserName"].ToString(),   // اسم المستخدم كنص
                                Email = reader["Email"].ToString(),         // البريد الإلكتروني
                                Password = reader["Password"].ToString(),   // كلمة المرور
                                JoinDate = Convert.ToDateTime(reader["JoinDate"]), // تاريخ الانضمام
                                Role = (UserRole)Enum.Parse(typeof(UserRole), reader["Role"].ToString()),// دور المستخدم (Role)
                                FailedAttempts = Convert.ToInt32(reader["failed_attempts"]), // عدد محاولات الدخول الفاشلة
                                // التحقق إذا كان هناك تاريخ لمحاولة الدخول الأخيرة، وإلا وضع قيمة فارغة
                                LastAttempt = reader["last_attempt"] != DBNull.Value
                                             ? (DateTime?)Convert.ToDateTime(reader["last_attempt"])
                                             : null,
                                IsDeleted = Convert.ToBoolean(reader["IsDeleted"]), // هل المستخدم محذوف (حذف ناعم)
                                // التحقق من وجود تاريخ تحديث، أو تعيين null
                                UpdatedAt = reader["UpdatedAt"] != DBNull.Value
                                            ? (DateTime?)Convert.ToDateTime(reader["UpdatedAt"])
                                            : null,
                            });
                        }
                    }
                }
            }

            // إرجاع قائمة المستخدمين
            return users;
        }


        // دالة لتحديث بيانات مستخدم موجود
        public static bool UpdateUser(User user)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                // أمر تحديث البيانات بحسب UserID
                using (SqlCommand cmd = new SqlCommand(
                    @"UPDATE [User] SET 
                        UserName = @UserName, 
                        Email = @Email, 
                        Password = @Password, 
                        Role = @Role, 
                        failed_attempts = @FailedAttempts, 
                        last_attempt = @LastAttempt, 
                        UpdatedAt = @UpdatedAt 
                      WHERE UserID = @UserID", con))
                {
                    // تعيين قيم الباراميترات من كائن المستخدم
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@FailedAttempts", user.FailedAttempts);
                    cmd.Parameters.AddWithValue("@LastAttempt", user.LastAttempt.HasValue ? (object)user.LastAttempt.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now); // تعيين تاريخ التحديث الحالي
                    cmd.Parameters.AddWithValue("@UserID", user.UserID);

                    con.Open(); // فتح الاتصال
                    // تنفيذ الأمر وإرجاع true إذا تم التحديث بنجاح
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // دالة للحذف الناعم للمستخدم (تغيير حالة IsDeleted بدلاً من حذف السجل)
        public static bool SoftDeleteUser(int userId)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                // أمر تحديث لتعيين IsDeleted إلى true وتحديث تاريخ التحديث
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE [User] SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE UserID = @UserID", con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    con.Open(); // فتح الاتصال
                    // تنفيذ الأمر وإرجاع true إذا تم التحديث (الحذف الناعم) بنجاح
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // دالة لجلب مستخدم واحد حسب اسم المستخدم، مع تعبئة كل البيانات اللازمة
        public static User GetUserByUsername(string username)
        {
            User user = null;
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [User] WHERE UserName = @u AND IsDeleted = 0", con))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            user = new User
                            {
                                UserID = Convert.ToInt32(dr["UserID"]),
                                UserName = dr["UserName"].ToString(),
                                Password = dr["Password"].ToString(),
                                Role = (UserRole)Enum.Parse(typeof(UserRole), dr["Role"].ToString()),
                                FailedAttempts = Convert.ToInt32(dr["failed_attempts"]),
                                LastAttempt = dr["last_attempt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["last_attempt"]) : null,
                                IsDeleted = Convert.ToBoolean(dr["IsDeleted"]),
                                UpdatedAt = dr["UpdatedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["UpdatedAt"]) : null,
                                Email = dr["Email"].ToString(),
                                JoinDate = Convert.ToDateTime(dr["JoinDate"])
                            };
                        }
                    }
                }
            }
            return user;
        }

        // دالة لتحديث عدد محاولات الدخول الفاشلة وتاريخ آخر محاولة
        public static bool UpdateFailedAttempts(string username, int attempts)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE [User] SET failed_attempts = @a, last_attempt = @t WHERE UserName = @u AND IsDeleted = 0", con))
                {
                    cmd.Parameters.AddWithValue("@a", attempts);
                    cmd.Parameters.AddWithValue("@t", DateTime.Now);
                    cmd.Parameters.AddWithValue("@u", username);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // دالة لإعادة تعيين محاولات الدخول إلى صفر بعد تسجيل دخول ناجح
        public static bool ResetFailedAttempts(string username)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE [User] SET failed_attempts = 0 WHERE UserName = @u AND IsDeleted = 0", con))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // دالة لإضافة مستخدم جديد مع إرجاع معرف المستخدم الجديد (ID)
        public static int? AddUser(User user)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();

                string sql = @"
                    INSERT INTO [User] (UserName, Email, Password, JoinDate, Role, failed_attempts, last_attempt)
                    OUTPUT INSERTED.UserID
                    VALUES (@username, @email, @password, @date, @role, 0, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@username", user.UserName);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@date", user.JoinDate);
                    cmd.Parameters.AddWithValue("@role", user.Role.ToString());

                    try
                    {
                        object result = cmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            return null;
                        }
                        return Convert.ToInt32(result);
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627)
                            throw new Exception("Email is already in use.");
                        else
                            throw;
                    }
                }
            }
        }
    }
}

