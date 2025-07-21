using Rahhal_System1.DAL;
using Rahhal_System1.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rahhal_System1.Forms
{
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }

        // عند تحميل الفورم، يتم عرض Panel تسجيل الدخول وإخفاء Panel التسجيل
        private void loginForm_Load(object sender, EventArgs e)
        {
            SignInPanel.Visible = true;
            SignUpPanel.Visible = false;
            SignInPanel.BringToFront(); // إظهار تسجيل الدخول في المقدمة
        }

        // دالة لتطبيق حواف دائرية على أي عنصر (مثل زر أو Panel)
        void ApplyRoundedRegion(Control ctl, int radius)
        {
            Rectangle rect = ctl.ClientRectangle;
            GraphicsPath path = new GraphicsPath();

            // رسم الحواف المنحنية
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            ctl.Region = new Region(path); // تطبيق الشكل الدائري على العنصر
        }

        // حدث الضغط على زر تسجيل الدخول
        private void btnSignIn_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear(); // مسح الأخطاء السابقة

            string username = txtInUsername.Text.Trim();
            string password = txtInPassword.Text;

            // التحقق من إدخال اسم المستخدم وكلمة المرور
            if (username == "")
            {
                errorProvider1.SetError(txtInUsername, "please enter username");
                return;
            }
            else if (password == "")
            {
                errorProvider1.SetError(txtInPassword, "please enter password");
                return;
            }

            // تشفير كلمة المرور المدخلة
            string hash = SecurityHelper.HashSHA256(password);

            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();

                // البحث عن المستخدم حسب الاسم وتجاهل المحذوفين
                SqlCommand cmd = new SqlCommand("SELECT * FROM [User] WHERE UserName = @u AND IsDeleted = 0", con);
                cmd.Parameters.AddWithValue("@u", username);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read()) // تم العثور على المستخدم
                {
                    // قراءة البيانات من قاعدة البيانات
                    int userId = Convert.ToInt32(dr["UserID"]);
                    string userName = dr["UserName"].ToString();
                    string dbHash = dr["Password"].ToString();
                    string role = dr["role"].ToString();
                    int attempts = Convert.ToInt32(dr["failed_attempts"]);
                    DateTime lastTry = dr["last_attempt"] != DBNull.Value ? Convert.ToDateTime(dr["last_attempt"]) : DateTime.MinValue;

                    dr.Close();

                    // تحقق من الحظر المؤقت بناءً على عدد المحاولات السابقة
                    if (attempts >= 5 && DateTime.Now < lastTry.AddHours(12))
                    {
                        MessageBox.Show("Account banned for 12 hours");
                        return;
                    }
                    if (attempts == 4 && DateTime.Now < lastTry.AddMinutes(5))
                    {
                        MessageBox.Show("Account banned for 5 minutes");
                        return;
                    }
                    if (attempts == 3 && DateTime.Now < lastTry.AddMinutes(1))
                    {
                        MessageBox.Show("Account banned for 1 minute");
                        return;
                    }

                    // مقارنة كلمة المرور المشفرة مع الموجودة في قاعدة البيانات
                    if (dbHash == hash)
                    {
                        // تسجيل الدخول الناجح
                        ActivityLogger.CurrentUser = new Rahhal_System1.Models.User
                        {
                            UserID = userId,
                            UserName = userName,
                            Role = role
                        };

                        ActivityLogger.Log(con, "Login", "User logged in successfully.");

                        // إعادة تعيين عدد المحاولات إلى 0
                        SqlCommand reset = new SqlCommand("UPDATE [User] SET failed_attempts = 0 WHERE UserName = @u AND IsDeleted = 0", con);
                        reset.Parameters.AddWithValue("@u", username);
                        reset.ExecuteNonQuery();

                        // فتح الفورم الرئيسي
                        this.Hide();
                        new HomeForm(username, role).Show();
                    }
                    else
                    {
                        // تسجيل محاولة فاشلة
                        UpdateAttempts(con, username, attempts + 1);

                        ActivityLogger.CurrentUser = new Rahhal_System1.Models.User
                        {
                            UserID = userId,
                            UserName = userName,
                            Role = role
                        };
                        ActivityLogger.Log(con, "Failed Login", $"Incorrect password (Attempt {attempts + 1}).");

                        MessageBox.Show("The password is incorrect");
                    }
                }
                else
                {
                    // المستخدم غير موجود
                    ActivityLogger.CurrentUser = null;
                    ActivityLogger.Log(con, "Failed Login", $"Login attempt for non-existent user: {username}");

                    MessageBox.Show("User not found");
                }
            }
        }

        // دالة لتحديث عدد المحاولات وتاريخ آخر محاولة فاشلة
        private void UpdateAttempts(SqlConnection con, string username, int count)
        {
            SqlCommand cmd = new SqlCommand("UPDATE [User] SET failed_attempts = @a, last_attempt = @t WHERE UserName = @u AND IsDeleted = 0", con);
            cmd.Parameters.AddWithValue("@a", count);
            cmd.Parameters.AddWithValue("@t", DateTime.Now);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.ExecuteNonQuery();
        }

        // حدث الضغط على زر "Sign Up"
        private void btnSignUp_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear(); // مسح الأخطاء السابقة

            string username = txtUpUsername.Text.Trim();
            string email = txtUpEmail.Text.Trim();
            string password = txtUpPass.Text;
            string confirm = txtConfirmPassword.Text;

            // تعابير Regex للتحقق من صحة البيانات
            Regex rUser = new Regex(@"^[A-Za-z]\w{4,}$"); // يبدأ بحرف وطوله ≥ 5
            Regex rPass = new Regex(@"^(?=.*\d)(?=.*\W).{8,}$"); // يحتوي على رقم ورمز وطوله ≥ 8
            Regex rEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"); // تحقق من صيغة البريد

            // التحقق من البيانات واحدة واحدة
            if (!rUser.IsMatch(username))
            {
                errorProvider1.SetError(txtUpUsername, "Username must start with a letter and contain at least 5 characters.");
                return;
            }
            if (!rEmail.IsMatch(email))
            {
                errorProvider1.SetError(txtUpEmail, "Invalid email format");
                return;
            }
            if (!rPass.IsMatch(password))
            {
                errorProvider1.SetError(txtUpPass, "Password must contain at least 8 characters including numbers and symbols.");
                return;
            }
            if (password != confirm)
            {
                errorProvider1.SetError(txtConfirmPassword, "Password does not match");
                return;
            }

            string hash = SecurityHelper.HashSHA256(password); // تشفير كلمة المرور

            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();

                // إضافة مستخدم جديد وإرجاع UserID الجديد
                string sql = @"
            INSERT INTO [User] (UserName, Email, Password, JoinDate, Role, failed_attempts, last_attempt)
            OUTPUT INSERTED.UserID
            VALUES (@username, @email, @password, @date, @role, 0, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", hash);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@role", "Regular");

                    try
                    {
                        object result = cmd.ExecuteScalar(); // استرجاع ID

                        if (result == null || result == DBNull.Value)
                        {
                            MessageBox.Show("⚠️ Failed to retrieve new user ID using OUTPUT INSERTED.");
                            return;
                        }

                        int newUserId = Convert.ToInt32(result);

                        // حفظ المستخدم الحالي لغرض التسجيل
                        ActivityLogger.CurrentUser = new Rahhal_System1.Models.User
                        {
                            UserID = newUserId,
                            UserName = username,
                            Role = "Regular"
                        };

                        ActivityLogger.Log(con, "Register", "New user registered.");

                        MessageBox.Show("The account has been created successfully. You can now log in.");

                        lblSignIn_Click(null, null); // الانتقال إلى واجهة تسجيل الدخول
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627)
                            MessageBox.Show("Email is already in use.");
                        else
                            MessageBox.Show("An error occurred while saving data: " + ex.Message);
                    }
                }
            }
        }

        // زر التبديل إلى واجهة تسجيل الدخول
        private void lblSignIn_Click(object sender, EventArgs e)
        {
            SignInPanel.Visible = true;
            SignInPanel.BringToFront();
            SignUpPanel.Visible = false;
        }

        // زر التبديل إلى واجهة التسجيل
        private void lblSignUp_Click(object sender, EventArgs e)
        {
            SignUpPanel.Visible = true;
            SignUpPanel.BringToFront();
            SignInPanel.Visible = false;
        }
    }
}
