namespace Rahhal_System1.Forms
{
    partial class UsersMessagesForm
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
            this.dgUsersMessages = new System.Windows.Forms.DataGridView();
            this.label = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgUsersMessages)).BeginInit();
            this.SuspendLayout();
            // 
            // dgUsersMessages
            // 
            this.dgUsersMessages.BackgroundColor = System.Drawing.Color.White;
            this.dgUsersMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgUsersMessages.Location = new System.Drawing.Point(34, 85);
            this.dgUsersMessages.Margin = new System.Windows.Forms.Padding(4);
            this.dgUsersMessages.Name = "dgUsersMessages";
            this.dgUsersMessages.Size = new System.Drawing.Size(700, 374);
            this.dgUsersMessages.TabIndex = 0;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Segoe Print", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label.Location = new System.Drawing.Point(45, 33);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(188, 37);
            this.label.TabIndex = 2;
            this.label.Text = "Users Messages :";
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Red;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(762, 393);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(97, 52);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete\r\nMessage";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // UsersMessagesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(883, 483);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.label);
            this.Controls.Add(this.dgUsersMessages);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UsersMessagesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UsersMessagesForm";
            this.Load += new System.EventHandler(this.UsersMessagesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgUsersMessages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgUsersMessages;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button btnDelete;
    }
}