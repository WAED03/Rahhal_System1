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

namespace Rahhal_System1
{
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();
            lblDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");

            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (sender, e) => lblDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            timer.Start();
        }

        void MakeRoundedCorners(PictureBox pic, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle bounds = pic.ClientRectangle;

            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            path.AddArc(bounds.Right - radius, bounds.Y, radius, radius, 270, 90);
            path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            pic.Region = new Region(path);
        }
        private void ShowUserControl(UserControl userControl)
        {
            mainPanel.Controls.Clear();
            userControl.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(userControl);
        }
        private void btnAccount_Click(object sender, EventArgs e)
        {
            ShowUserControl(new AccountUC());
        }

        private void btnTrips_Click(object sender, EventArgs e)
        {
            ShowUserControl(new TripsUC());
        }

        private void btnCity_Click(object sender, EventArgs e)
        {
            ShowUserControl(new CitiesUC());
        }

        private void btnPhrase_Click(object sender, EventArgs e)
        {
            ShowUserControl(new PhraseUC());
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm != this)
                    frm.Hide();
            }
            HomeForm homeForm = new HomeForm();
            homeForm.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblSeeMoreTrip_Click(object sender, EventArgs e)
        {
            ShowUserControl(new TripsUC());
        }

        private void lblSeeMoreVisit_Click(object sender, EventArgs e)
        {
            ShowUserControl(new CitiesUC());
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
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
        }
    }
}
