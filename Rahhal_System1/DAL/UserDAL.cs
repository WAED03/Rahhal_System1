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

        // دالة لإضافة مستخدم جديد إلى قاعدة البيانات
        public static bool AddUser(User user)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                // أمر SQL للإضافة مع تحديد الحقول والقيم باستخدام باراميترات
                using (SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO [User] 
                      (UserName, Email, Password, JoinDate, Role, failed_attempts, last_attempt, IsDeleted, UpdatedAt) 
                      VALUES 
                      (@UserName, @Email, @Password, @JoinDate, @Role, @FailedAttempts, @LastAttempt, @IsDeleted, @UpdatedAt)", con))
                {
                    // تعيين قيم الباراميترات من كائن المستخدم
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@JoinDate", user.JoinDate);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@FailedAttempts", user.FailedAttempts);
                    // التعامل مع إمكانية وجود قيمة فارغة للتاريخ
                    cmd.Parameters.AddWithValue("@LastAttempt", user.LastAttempt.HasValue ? (object)user.LastAttempt.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsDeleted", user.IsDeleted);
                    cmd.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt.HasValue ? (object)user.UpdatedAt.Value : DBNull.Value);

                    con.Open(); // فتح الاتصال
                    // تنفيذ الأمر وإرجاع true إذا تم الإضافة بنجاح (عدد الصفوف المتأثرة أكبر من 0)
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
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
    }
}
