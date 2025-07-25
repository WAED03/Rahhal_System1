// استيراد المساحات الضرورية للتعامل مع البيانات، القاعدة، النماذج، والواجهة
using Rahhal_System1.DAL;
using Rahhal_System1.Data;
using Rahhal_System1.Models;
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
            InitializeComponent(); // تحميل مكونات الفورم
        }

        // دالة لتطبيق حواف دائرية على أي عنصر واجهة (زر، بانل، ...إلخ)
        void ApplyRoundedRegion(Control ctl, int radius)
        {
            Rectangle rect = ctl.ClientRectangle;
            GraphicsPath path = new GraphicsPath();

            // رسم الحواف الدائرية حسب الزوايا الأربع
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90); // الزاوية العليا اليسرى
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90); // العليا اليمنى
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90); // السفلى اليمنى
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90); // السفلى اليسرى
            path.CloseFigure();

            ctl.Region = new Region(path); // تطبيق الشكل النهائي على العنصر
        }

        // عند تحميل الفورم يتم عرض واجهة تسجيل الدخول فقط
        private void loginForm_Load(object sender, EventArgs e)
        {
            SignInPanel.Visible = true;
            SignUpPanel.Visible = false;
            SignInPanel.BringToFront(); // جعل Panel تسجيل الدخول في الواجهة الأمامية
        }

        // حدث عند الضغط على زر تسجيل الدخول
        private void btnSignIn_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear(); // مسح الأخطاء السابقة من الواجهة

            string username = txtInUsername.Text.Trim(); // اسم المستخدم من التكست
            string password = txtInPassword.Text;        // كلمة المرور من التكست

            // التحقق من أن اسم المستخدم وكلمة المرور غير فارغين
            if (string.IsNullOrEmpty(username))
            {
                errorProvider1.SetError(txtInUsername, "please enter username");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                errorProvider1.SetError(txtInPassword, "please enter password");
                return;
            }

            User user;
            try
            {
                // جلب بيانات المستخدم من قاعدة البيانات عن طريق DAL
                user = UserDAL.GetUserByUsername(username);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error accessing database: " + ex.Message);
                return;
            }

            if (user == null)
            {
                // المستخدم غير موجود
                ActivityLogger.CurrentUser = null;
                MessageBox.Show("User not found");
                return;
            }

            // التحقق من عدد المحاولات الفاشلة وتحديد مدة الحظر المؤقت
            if (user.FailedAttempts >= 5 && DateTime.Now < user.LastAttempt?.AddHours(12))
            {
                MessageBox.Show("Account banned for 12 hours");
                return;
            }
            if (user.FailedAttempts == 4 && DateTime.Now < user.LastAttempt?.AddMinutes(5))
            {
                MessageBox.Show("Account banned for 5 minutes");
                return;
            }
            if (user.FailedAttempts == 3 && DateTime.Now < user.LastAttempt?.AddMinutes(1))
            {
                MessageBox.Show("Account banned for 1 minute");
                return;
            }

            // تشفير كلمة المرور المدخلة لمقارنتها مع المخزنة
            string hash = SecurityHelper.HashSHA256(password);

            if (user.Password == hash)
            {
                // كلمة المرور صحيحة - تسجيل دخول ناجح
                ActivityLogger.CurrentUser = user;

                using (var con = DbHelper.GetConnection())
                {
                    con.Open();
                    ActivityLogger.Log(con, "Login", "User logged in successfully."); // تسجيل الحدث
                }

                UserDAL.ResetFailedAttempts(username); // إعادة تعيين عدد المحاولات

                this.Hide(); // إخفاء الفورم الحالي
                new HomeForm(user.UserName, user.Role).Show(); // فتح الفورم الرئيسي
            }
            else
            {
                // كلمة المرور غير صحيحة
                UserDAL.UpdateFailedAttempts(username, user.FailedAttempts + 1); // تحديث عدد المحاولات

                using (var con = DbHelper.GetConnection())
                {
                    con.Open();
                    ActivityLogger.Log(con, "Failed Login", $"Incorrect password (Attempt {user.FailedAttempts + 1})."); // تسجيل فشل الدخول
                }

                MessageBox.Show("The password is incorrect");
            }
        }

        // حدث عند الضغط على زر إنشاء حساب (Sign Up)
        private void btnSignUp_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear(); // مسح الأخطاء السابقة

            string username = txtUpUsername.Text.Trim();
            string email = txtUpEmail.Text.Trim();
            string password = txtUpPass.Text;
            string confirm = txtConfirmPassword.Text;

            // تعابير Regex للتحقق من صحة البيانات المدخلة
            Regex rUser = new Regex(@"^[A-Za-z]\w{4,}$");              // اسم المستخدم يبدأ بحرف وطوله ≥ 5
            Regex rPass = new Regex(@"^(?=.*\d)(?=.*\W).{8,}$");       // كلمة المرور تحتوي على رقم ورمز وطول ≥ 8
            Regex rEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");   // بريد إلكتروني صحيح

            // التحقق من صلاحية اسم المستخدم
            if (!rUser.IsMatch(username))
            {
                errorProvider1.SetError(txtUpUsername, "Username must start with a letter and contain at least 5 characters.");
                return;
            }

            // التحقق من صلاحية البريد الإلكتروني
            if (!rEmail.IsMatch(email))
            {
                errorProvider1.SetError(txtUpEmail, "Invalid email format");
                return;
            }

            // التحقق من قوة كلمة المرور
            if (!rPass.IsMatch(password))
            {
                errorProvider1.SetError(txtUpPass, "Password must contain at least 8 characters including numbers and symbols.");
                return;
            }

            // التأكد من تطابق كلمتي المرور
            if (password != confirm)
            {
                errorProvider1.SetError(txtConfirmPassword, "Password does not match");
                return;
            }

            // تشفير كلمة المرور
            string hash = SecurityHelper.HashSHA256(password);

            // إنشاء كائن مستخدم جديد
            var newUser = new User
            {
                UserName = username,
                Email = email,
                Password = hash,
                JoinDate = DateTime.Now.Date,
                Role = UserRole.Regular
            };

            try
            {
                // محاولة حفظ المستخدم الجديد
                int? newUserId = UserDAL.AddUser(newUser);

                if (newUserId == null)
                {
                    MessageBox.Show("⚠️ Failed to retrieve new user ID.");
                    return;
                }

                // تسجيل المستخدم كـ currentUser
                ActivityLogger.CurrentUser = new User
                {
                    UserID = newUserId.Value,
                    UserName = username,
                    Role = UserRole.Regular
                };

                using (var con = DbHelper.GetConnection())
                {
                    con.Open();
                    ActivityLogger.Log(con, "Register", "New user registered.");
                }

                MessageBox.Show("The account has been created successfully. You can now log in.");

                // العودة إلى واجهة تسجيل الدخول
                lblSignIn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving data: " + ex.Message);
            }
        }

        // عند الضغط على زر "Sign In" يتم عرض واجهة تسجيل الدخول
        private void lblSignIn_Click(object sender, EventArgs e)
        {
            SignInPanel.Visible = true;
            SignInPanel.BringToFront();
            SignUpPanel.Visible = false;
        }

        // عند الضغط على زر "Sign Up" يتم عرض واجهة إنشاء الحساب
        private void lblSignUp_Click(object sender, EventArgs e)
        {
            SignUpPanel.Visible = true;
            SignUpPanel.BringToFront();
            SignInPanel.Visible = false;
        }
    }
}
