namespace Rahhal_System1.UC
{
    partial class PhraseUC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgPhrase = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAddPhrase = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgPhrase)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgPhrase
            // 
            this.dgPhrase.BackgroundColor = System.Drawing.Color.White;
            this.dgPhrase.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPhrase.Location = new System.Drawing.Point(22, 0);
            this.dgPhrase.Name = "dgPhrase";
            this.dgPhrase.Size = new System.Drawing.Size(888, 465);
            this.dgPhrase.TabIndex = 0;
            this.dgPhrase.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgPhrase_CellClick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.btnAddPhrase);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1115, 600);
            this.panel1.TabIndex = 1;
            // 
            // btnAddPhrase
            // 
            this.btnAddPhrase.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnAddPhrase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddPhrase.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddPhrase.ForeColor = System.Drawing.Color.White;
            this.btnAddPhrase.Location = new System.Drawing.Point(968, 491);
            this.btnAddPhrase.Name = "btnAddPhrase";
            this.btnAddPhrase.Size = new System.Drawing.Size(102, 46);
            this.btnAddPhrase.TabIndex = 7;
            this.btnAddPhrase.Text = "Add Phrase";
            this.btnAddPhrase.UseVisualStyleBackColor = false;
            this.btnAddPhrase.Click += new System.EventHandler(this.btnAddPhrase_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dgPhrase);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 116);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(913, 484);
            this.panel3.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1115, 116);
            this.panel2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe Print", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(16, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 36);
            this.label1.TabIndex = 0;
            this.label1.Text = "My Words :";
            // 
            // PhraseUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PhraseUC";
            this.Size = new System.Drawing.Size(1115, 600);
            ((System.ComponentModel.ISupportInitialize)(this.dgPhrase)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgPhrase;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAddPhrase;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
    }
}
