// استدعاء المكتبات المطلوبة
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

// استدعاء الفئات والمكونات من المشروع
using Rahhal_System1.UC;
using Rahhal_System1.DAL;
using Rahhal_System1.Data;
using Rahhal_System1.Models;

namespace Rahhal_System1.Forms
{
    public partial class HomeForm : Form
    {
        // متغيرات لتخزين اسم المستخدم والدور
        private string currentUser, currentRole;

        // دالة البناء - تُستدعى عند فتح الفورم لأول مرة
        public HomeForm(string user, string role)
        {
            InitializeComponent();

            // عرض التاريخ والوقت الحالي في اللابل
            lblDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");

            // إنشاء مؤقت لتحديث الوقت كل ثانية
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 ثانية
            timer.Tick += (sender, e) => lblDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            timer.Start();

            // تخزين بيانات المستخدم الحالي
            currentUser = user;
            currentRole = role;

            // عرض اسم المستخدم والدور في اللابل
            lblUsername.Text = currentUser + "  ( " + currentRole + " )";
        }

        // دالة تقوم بجعل زوايا الصورة دائرية
        void MakeRoundedCorners(PictureBox pic, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle bounds = pic.ClientRectangle;

            // إنشاء مسار بزوايا دائرية
            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            path.AddArc(bounds.Right - radius, bounds.Y, radius, radius, 270, 90);
            path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            // تعيين الشكل الجديد كمنطقة عرض للصورة
            pic.Region = new Region(path);
        }

        // تُنفذ عند تحميل الفورم
        private void HomeForm_Load(object sender, EventArgs e)
        {
            // تطبيق الزوايا الدائرية على الصور
            MakeRoundedCorners(pic1, 25);
            MakeRoundedCorners(pic2, 25);
            MakeRoundedCorners(pic3, 25);
            MakeRoundedCorners(pic4, 25);
            MakeRoundedCorners(pic5, 25);
            MakeRoundedCorners(pic6, 25);
            MakeRoundedCorners(picInter1, 25);
            MakeRoundedCorners(picInter2, 25);
            MakeRoundedCorners(picInter3, 25);
            MakeRoundedCorners(picInter4, 25);

            // عرض آخر الرحلات والزيارات
            ShowLastTripsAndVisits();
        }

        // دالة مساعدة لتغيير محتوى البانل إلى UserControl معين
        private void ShowUserControl(UserControl userControl)
        {
            mainPanel.Controls.Clear(); // مسح المحتوى الحالي
            userControl.Dock = DockStyle.Fill; // تعبئة المساحة بالكامل
            mainPanel.Controls.Add(userControl); // إضافته للبانل
        }

        // زر الرحلات - عرض واجهة الرحلات
        private void btnTrips_Click(object sender, EventArgs e)
        {
            ShowUserControl(new TripsUC());
        }

        // زر المدن - عرض واجهة المدن
        private void btnCity_Click(object sender, EventArgs e)
        {
            ShowUserControl(new CitiesUC());
        }

        // زر العبارات - عرض واجهة العبارات
        private void btnPhrase_Click(object sender, EventArgs e)
        {
            ShowUserControl(new PhraseUC());
        }

        // زر الصفحة الرئيسية - إعادة تحميل الفورم من جديد
        private void btnHome_Click(object sender, EventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm != this)
                    frm.Hide();
            }
            HomeForm homeForm = new HomeForm(currentUser, currentRole);
            homeForm.Show();
        }

        // زر الخروج من التطبيق
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // رابط "المزيد من الرحلات" - يفتح واجهة الرحلات
        private void lblSeeMoreTrip_Click(object sender, EventArgs e)
        {
            ShowUserControl(new TripsUC());
        }

        // رابط "المزيد من الزيارات" - يفتح واجهة المدن
        private void lblSeeMoreVisit_Click(object sender, EventArgs e)
        {
            ShowUserControl(new CitiesUC());
        }

        // زر "حول البرنامج"
        private void btnAbout_Click(object sender, EventArgs e)
        {
            ShowUserControl(new AboutUC());
        }

        // زر "اتصل بنا"
        private void btnCallUs_Click(object sender, EventArgs e)
        {
            ShowUserControl(new CallusUC());
        }

        // زر الإعدادات
        private void btnSettings_Click(object sender, EventArgs e)
        {
            new SettingsForm(currentUser, currentRole).ShowDialog();
        }

        // دالة لعرض آخر 3 رحلات و 3 زيارات للمدن
        private void ShowLastTripsAndVisits()
        {
            try
            {
                // التحقق من وجود مستخدم حالي
                if (ActivityLogger.CurrentUser == null)
                {
                    MessageBox.Show("❌ No user is currently logged in.");
                    return;
                }

                int userId = ActivityLogger.CurrentUser.UserID;

                // ✅ جلب كل الرحلات للمستخدم
                GlobalData.RefreshTrips(userId);
                var userTrips = GlobalData.TripsList;

                // ✅ ترتيب الرحلات حسب آخر تعديل أو بداية
                var last3Trips = userTrips
                    .OrderByDescending(t => t.UpdatedAt ?? t.StartDate)
                    .Take(3)
                    .ToList();

                // ⬇️ عرض أسماء الرحلات في اللابلز (lbl1, lbl2, lbl3)
                Label[] tripLabels = { lbl1, lbl2, lbl3 };
                for (int i = 0; i < tripLabels.Length; i++)
                {
                    if (i < last3Trips.Count)
                    {
                        var trip = last3Trips[i];
                        tripLabels[i].Text = $"{trip.TripName}\n({trip.StartDate:yyyy-MM-dd})";
                    }
                    else
                    {
                        tripLabels[i].Text = "";
                    }
                }

                // ✅ تحميل كل الزيارات من جميع الرحلات
                var allVisits = new List<CityVisit>();
                foreach (var trip in userTrips)
                {
                    var visits = CityVisitDAL.GetVisitsByTrip(trip.TripID);
                    allVisits.AddRange(visits);
                }

                // ✅ ترتيب الزيارات حسب التاريخ
                var last3Visits = allVisits
                    .OrderByDescending(v => v.VisitDate)
                    .Take(3)
                    .ToList();

                // ⬇️ عرض أسماء المدن وتواريخ الزيارة
                Label[] visitLabels = { lbl4, lbl5, lbl6 };
                for (int i = 0; i < visitLabels.Length; i++)
                {
                    if (i < last3Visits.Count)
                    {
                        var visit = last3Visits[i];
                        visitLabels[i].Text = $"{visit.City.CityName}\n({visit.VisitDate:yyyy-MM-dd})";
                    }
                    else
                    {
                        visitLabels[i].Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                // في حالة وجود خطأ، إظهار رسالة توضح السبب
                MessageBox.Show("⚠️ Failed to load recent trips and visits:\n" + ex.Message);
            }
        }
    }
}
