using Rahhal_System1.DAL;
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Rahhal_System1.Models
{
    public class UserDAL
    {
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM [User]", DbHelper.GetConnection());

            try
            {
                DbHelper.OpenConnection();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString(),
                        JoinDate = Convert.ToDateTime(reader["JoinDate"]),
                        Role = reader["Role"].ToString()
                    });
                }
                reader.Close();
            }
            finally
            {
                DbHelper.CloseConnection();
            }

            return users;
        }

        public static bool AddUser(User user)
        {
            SqlCommand cmd = new SqlCommand(
                "INSERT INTO [User] (UserName, Email, Password, JoinDate, Role) VALUES (@UserName, @Email, @Password, @JoinDate, @Role)",
                DbHelper.GetConnection());

            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password); // ⚠️ لاحقًا استخدم التشفير
            cmd.Parameters.AddWithValue("@JoinDate", user.JoinDate);
            cmd.Parameters.AddWithValue("@Role", user.Role);

            try
            {
                DbHelper.OpenConnection();
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                DbHelper.CloseConnection();
            }
        }

        // يمكن إضافة المزيد مثل UpdateUser, DeleteUser لاحقًا
    }
}

