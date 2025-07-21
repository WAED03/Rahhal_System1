namespace Rahhal_System1.Forms
{
    partial class SettingsForm
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
            this.btnSwitchAccount = new System.Windows.Forms.Button();
            this.btnMode = new System.Windows.Forms.Button();
            this.btnViewUsers = new System.Windows.Forms.Button();
            this.btnUsersMessages = new System.Windows.Forms.Button();
            this.btnEventsLog = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSwitchAccount
            // 
            this.btnSwitchAccount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSwitchAccount.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSwitchAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnSwitchAccount.Location = new System.Drawing.Point(129, 118);
            this.btnSwitchAccount.Name = "btnSwitchAccount";
            this.btnSwitchAccount.Size = new System.Drawing.Size(234, 53);
            this.btnSwitchAccount.TabIndex = 0;
            this.btnSwitchAccount.Text = "Switch Account";
            this.btnSwitchAccount.UseVisualStyleBackColor = true;
            this.btnSwitchAccount.Click += new System.EventHandler(this.btnSwitchAccount_Click);
            // 
            // btnMode
            // 
            this.btnMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMode.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnMode.Location = new System.Drawing.Point(129, 177);
            this.btnMode.Name = "btnMode";
            this.btnMode.Size = new System.Drawing.Size(234, 53);
            this.btnMode.TabIndex = 1;
            this.btnMode.Text = "Dark / Light Mode";
            this.btnMode.UseVisualStyleBackColor = true;
            this.btnMode.Click += new System.EventHandler(this.btnMode_Click);
            // 
            // btnViewUsers
            // 
            this.btnViewUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewUsers.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnViewUsers.Location = new System.Drawing.Point(129, 354);
            this.btnViewUsers.Name = "btnViewUsers";
            this.btnViewUsers.Size = new System.Drawing.Size(234, 53);
            this.btnViewUsers.TabIndex = 9;
            this.btnViewUsers.Text = "View Users";
            this.btnViewUsers.UseVisualStyleBackColor = true;
            this.btnViewUsers.Click += new System.EventHandler(this.btnViewUsers_Click);
            // 
            // btnUsersMessages
            // 
            this.btnUsersMessages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsersMessages.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUsersMessages.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnUsersMessages.Location = new System.Drawing.Point(129, 295);
            this.btnUsersMessages.Name = "btnUsersMessages";
            this.btnUsersMessages.Size = new System.Drawing.Size(234, 53);
            this.btnUsersMessages.TabIndex = 10;
            this.btnUsersMessages.Text = "Users Messages";
            this.btnUsersMessages.UseVisualStyleBackColor = true;
            this.btnUsersMessages.Click += new System.EventHandler(this.btnUsersMessages_Click);
            // 
            // btnEventsLog
            // 
            this.btnEventsLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEventsLog.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEventsLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnEventsLog.Location = new System.Drawing.Point(129, 236);
            this.btnEventsLog.Name = "btnEventsLog";
            this.btnEventsLog.Size = new System.Drawing.Size(234, 53);
            this.btnEventsLog.TabIndex = 11;
            this.btnEventsLog.Text = "Events Log";
            this.btnEventsLog.UseVisualStyleBackColor = true;
            this.btnEventsLog.Click += new System.EventHandler(this.btnEventsLog_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Rahhal_System1.Properties.Resources.Darkmode;
            this.pictureBox2.Location = new System.Drawing.Point(71, 177);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(52, 53);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Rahhal_System1.Properties.Resources.account;
            this.pictureBox1.Location = new System.Drawing.Point(71, 118);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(52, 53);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(497, 494);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnViewUsers);
            this.Controls.Add(this.btnUsersMessages);
            this.Controls.Add(this.btnEventsLog);
            this.Controls.Add(this.btnMode);
            this.Controls.Add(this.btnSwitchAccount);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSwitchAccount;
        private System.Windows.Forms.Button btnMode;
        private System.Windows.Forms.Button btnViewUsers;
        private System.Windows.Forms.Button btnUsersMessages;
        private System.Windows.Forms.Button btnEventsLog;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}