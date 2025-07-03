using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rahhal_System1
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
            btnSignin.Visible = false;
            btnSignup.Visible = false;
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            RoundedButton btnSignIn = new RoundedButton();
            btnSignIn.Text = "Sign In";
            btnSignIn.BorderRadius = 15;
            btnSignIn.Size = new Size(120, 40);
            btnSignIn.Location = new Point(113, 340);
            btnSignIn.BackColor = Color.DodgerBlue;
            btnSignIn.ForeColor = Color.White;
            btnSignIn.FlatStyle = FlatStyle.Flat;
            btnSignIn.FlatAppearance.BorderSize = 0;
            btnSignIn.Click += btnSignIn_Click;

            SignInPanel.Controls.Add(btnSignIn);


            RoundedButton btnSignUp = new RoundedButton();
            btnSignUp.Text = "Sign Up";
            btnSignUp.BorderRadius = 15;
            btnSignUp.Size = new Size(120, 40);
            btnSignUp.Location = new Point(113, 365);
            btnSignUp.BackColor = Color.DodgerBlue;
            btnSignUp.ForeColor = Color.White;
            btnSignUp.FlatStyle = FlatStyle.Flat;
            btnSignUp.FlatAppearance.BorderSize = 0;
            btnSignUp.Click += btnSignUp_Click;

            SignUpPanel.Controls.Add(btnSignUp);

            ApplyRoundedRegion(SignInPanel, 25); 
            SignInPanel.Resize += (s, ev) => ApplyRoundedRegion(SignInPanel, 25);

            ApplyRoundedRegion(SignUpPanel, 25);
            SignUpPanel.Resize += (s, ev) => ApplyRoundedRegion(SignUpPanel, 25);

        }

        void ApplyRoundedRegion(Control ctl, int radius)
        {
            Rectangle rect = ctl.ClientRectangle;
            GraphicsPath path = new GraphicsPath();

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            ctl.Region = new Region(path);
        }
        private void btnSignIn_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Hide();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            SignInPanel.BringToFront();
        }

        private void lblSignIn_Click(object sender, EventArgs e)
        {
            SignInPanel.BringToFront();
        }

        private void lblSignUp_Click(object sender, EventArgs e)
        {
            SignUpPanel.BringToFront();
        }
    }
}
