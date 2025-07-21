using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Rahhal_System1.Models;

namespace Rahhal_System1.DAL
{
    public static class ActivityLogger
    {
        // ✅ المستخدم الحالي (يتم تعيينه عند تسجيل الدخول)
        public static User CurrentUser;

        // ✅ رقم الزيارة الحالية (مطلوب عند التعامل مع العبارات)
        public static int CurrentVisitID { get; set; }

        // ✅ رقم الرحلة الحالية (مطلوب عند التعامل مع الزيارات)
        public static int CurrentTripID { get; set; }


        // يتم استدعاء هذا الميثود لتسجيل أي نشاط
        public static void Log(SqlConnection connection, SqlTransaction transaction, string actionType, string actionDetails)
        {
            if (CurrentUser == null) return;

            string sql = @"INSERT INTO UserActivityLog (UserID, UserName, ActionType, ActionDetails)
                   VALUES (@UserID, @UserName, @ActionType, @ActionDetails)";

            using (SqlCommand cmd = new SqlCommand(sql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserID);
                cmd.Parameters.AddWithValue("@UserName", CurrentUser.UserName);
                cmd.Parameters.AddWithValue("@ActionType", actionType);
                cmd.Parameters.AddWithValue("@ActionDetails", actionDetails);

                cmd.ExecuteNonQuery();
            }
        }
        // نسخة إضافية بدون Transaction
        public static void Log(SqlConnection connection, string actionType, string actionDetails)
        {
            Log(connection, null, actionType, actionDetails);
        }
    }
}

