namespace Rahhal_System1.Forms
{
    partial class loginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SignUpPanel = new System.Windows.Forms.Panel();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblSignIn = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSignup = new System.Windows.Forms.Button();
            this.txtUpPass = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUpEmail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUpUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SignInPanel = new System.Windows.Forms.Panel();
            this.lblSignUp = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSignin = new System.Windows.Forms.Button();
            this.txtInPassword = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtInUsername = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.SignUpPanel.SuspendLayout();
            this.SignInPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // SignUpPanel
            // 
            this.SignUpPanel.BackColor = System.Drawing.Color.White;
            this.SignUpPanel.Controls.Add(this.txtConfirmPassword);
            this.SignUpPanel.Controls.Add(this.label6);
            this.SignUpPanel.Controls.Add(this.lblSignIn);
            this.SignUpPanel.Controls.Add(this.label5);
            this.SignUpPanel.Controls.Add(this.btnSignup);
            this.SignUpPanel.Controls.Add(this.txtUpPass);
            this.SignUpPanel.Controls.Add(this.label4);
            this.SignUpPanel.Controls.Add(this.txtUpEmail);
            this.SignUpPanel.Controls.Add(this.label3);
            this.SignUpPanel.Controls.Add(this.txtUpUsername);
            this.SignUpPanel.Controls.Add(this.label2);
            this.SignUpPanel.Controls.Add(this.label1);
            this.SignUpPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SignUpPanel.Location = new System.Drawing.Point(0, 0);
            this.SignUpPanel.Name = "SignUpPanel";
            this.SignUpPanel.Size = new System.Drawing.Size(618, 510);
            this.SignUpPanel.TabIndex = 0;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Location = new System.Drawing.Point(188, 340);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(230, 20);
            this.txtConfirmPassword.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(185, 320);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 17);
            this.label6.TabIndex = 10;
            this.label6.Text = "Confirm Password :";
            // 
            // lblSignIn
            // 
            this.lblSignIn.AutoSize = true;
            this.lblSignIn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSignIn.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblSignIn.Location = new System.Drawing.Point(356, 467);
            this.lblSignIn.Name = "lblSignIn";
            this.lblSignIn.Size = new System.Drawing.Size(43, 13);
            this.lblSignIn.TabIndex = 9;
            this.lblSignIn.Text = "Sign In";
            this.lblSignIn.Click += new System.EventHandler(this.lblSignIn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(200, 467);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Already have an account ?";
            // 
            // btnSignup
            // 
            this.btnSignup.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnSignup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSignup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSignup.ForeColor = System.Drawing.Color.White;
            this.btnSignup.Location = new System.Drawing.Point(246, 399);
            this.btnSignup.Name = "btnSignup";
            this.btnSignup.Size = new System.Drawing.Size(116, 42);
            this.btnSignup.TabIndex = 8;
            this.btnSignup.Text = "Sign Up";
            this.btnSignup.UseVisualStyleBackColor = false;
            this.btnSignup.Click += new System.EventHandler(this.btnSignUp_Click);
            // 
            // txtUpPass
            // 
            this.txtUpPass.Location = new System.Drawing.Point(188, 266);
            this.txtUpPass.Name = "txtUpPass";
            this.txtUpPass.PasswordChar = '*';
            this.txtUpPass.Size = new System.Drawing.Size(230, 20);
            this.txtUpPass.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(185, 246);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 17);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password :";
            // 
            // txtUpEmail
            // 
            this.txtUpEmail.Location = new System.Drawing.Point(188, 201);
            this.txtUpEmail.Name = "txtUpEmail";
            this.txtUpEmail.Size = new System.Drawing.Size(230, 20);
            this.txtUpEmail.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(185, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Email :";
            // 
            // txtUpUsername
            // 
            this.txtUpUsername.Location = new System.Drawing.Point(188, 137);
            this.txtUpUsername.Name = "txtUpUsername";
            this.txtUpUsername.Size = new System.Drawing.Size(230, 20);
            this.txtUpUsername.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(185, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Username :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe Script", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label1.Location = new System.Drawing.Point(168, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(306, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome , Sign Up Here !  ";
            // 
            // SignInPanel
            // 
            this.SignInPanel.BackColor = System.Drawing.Color.White;
            this.SignInPanel.Controls.Add(this.lblSignUp);
            this.SignInPanel.Controls.Add(this.label7);
            this.SignInPanel.Controls.Add(this.btnSignin);
            this.SignInPanel.Controls.Add(this.txtInPassword);
            this.SignInPanel.Controls.Add(this.label8);
            this.SignInPanel.Controls.Add(this.txtInUsername);
            this.SignInPanel.Controls.Add(this.label10);
            this.SignInPanel.Controls.Add(this.label11);
            this.SignInPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SignInPanel.Location = new System.Drawing.Point(0, 0);
            this.SignInPanel.Name = "SignInPanel";
            this.SignInPanel.Size = new System.Drawing.Size(618, 510);
            this.SignInPanel.TabIndex = 10;
            // 
            // lblSignUp
            // 
            this.lblSignUp.AutoSize = true;
            this.lblSignUp.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSignUp.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblSignUp.Location = new System.Drawing.Point(346, 444);
            this.lblSignUp.Name = "lblSignUp";
            this.lblSignUp.Size = new System.Drawing.Size(48, 13);
            this.lblSignUp.TabIndex = 9;
            this.lblSignUp.Text = "Sign Up";
            this.lblSignUp.Click += new System.EventHandler(this.lblSignUp_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(201, 444);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(131, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Don\'t have an account ?";
            // 
            // btnSignin
            // 
            this.btnSignin.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnSignin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSignin.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSignin.ForeColor = System.Drawing.Color.White;
            this.btnSignin.Location = new System.Drawing.Point(245, 355);
            this.btnSignin.Name = "btnSignin";
            this.btnSignin.Size = new System.Drawing.Size(116, 42);
            this.btnSignin.TabIndex = 7;
            this.btnSignin.Text = "Sign In";
            this.btnSignin.UseVisualStyleBackColor = false;
            this.btnSignin.Click += new System.EventHandler(this.btnSignIn_Click);
            // 
            // txtInPassword
            // 
            this.txtInPassword.Location = new System.Drawing.Point(188, 279);
            this.txtInPassword.Name = "txtInPassword";
            this.txtInPassword.PasswordChar = '*';
            this.txtInPassword.Size = new System.Drawing.Size(230, 20);
            this.txtInPassword.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(185, 259);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 17);
            this.label8.TabIndex = 5;
            this.label8.Text = "Password :";
            // 
            // txtInUsername
            // 
            this.txtInUsername.Location = new System.Drawing.Point(187, 171);
            this.txtInUsername.Name = "txtInUsername";
            this.txtInUsername.Size = new System.Drawing.Size(230, 20);
            this.txtInUsername.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(184, 151);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 17);
            this.label10.TabIndex = 1;
            this.label10.Text = "Username :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe Script", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label11.Location = new System.Drawing.Point(180, 53);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(242, 44);
            this.label11.TabIndex = 0;
            this.label11.Text = "Welcome Back !";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // loginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(618, 510);
            this.Controls.Add(this.SignInPanel);
            this.Controls.Add(this.SignUpPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "loginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.loginForm_Load);
            this.SignUpPanel.ResumeLayout(false);
            this.SignUpPanel.PerformLayout();
            this.SignInPanel.ResumeLayout(false);
            this.SignInPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel SignUpPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUpUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSignup;
        private System.Windows.Forms.TextBox txtUpPass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUpEmail;
        private System.Windows.Forms.Label lblSignIn;
        private System.Windows.Forms.Panel SignInPanel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSignin;
        private System.Windows.Forms.TextBox txtInPassword;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtInUsername;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblSignUp;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Label label6;
    }
}

