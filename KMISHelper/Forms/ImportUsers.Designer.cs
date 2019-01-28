namespace KMISHelper.Forms
{
    partial class ImportUsers
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbSex = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPreImport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.cbEmail = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbMobile = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnUpload = new System.Windows.Forms.Button();
            this.cbRole = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbKindergarten = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbKindergarten);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cbRole);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cbMobile);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbSex);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cbEmail);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(34, 22);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(941, 186);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MAPPING Users";
            // 
            // cbSex
            // 
            this.cbSex.FormattingEnabled = true;
            this.cbSex.Location = new System.Drawing.Point(483, 54);
            this.cbSex.Margin = new System.Windows.Forms.Padding(2);
            this.cbSex.Name = "cbSex";
            this.cbSex.Size = new System.Drawing.Size(200, 21);
            this.cbSex.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(481, 27);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Sex：";
            // 
            // btnPreImport
            // 
            this.btnPreImport.Location = new System.Drawing.Point(770, 225);
            this.btnPreImport.Margin = new System.Windows.Forms.Padding(2);
            this.btnPreImport.Name = "btnPreImport";
            this.btnPreImport.Size = new System.Drawing.Size(94, 28);
            this.btnPreImport.TabIndex = 7;
            this.btnPreImport.Text = "Pre Import";
            this.btnPreImport.UseVisualStyleBackColor = true;
            this.btnPreImport.Click += new System.EventHandler(this.btnPreImport_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(881, 225);
            this.btnImport.Margin = new System.Windows.Forms.Padding(2);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(94, 28);
            this.btnImport.TabIndex = 8;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // cbEmail
            // 
            this.cbEmail.FormattingEnabled = true;
            this.cbEmail.Location = new System.Drawing.Point(249, 54);
            this.cbEmail.Margin = new System.Windows.Forms.Padding(2);
            this.cbEmail.Name = "cbEmail";
            this.cbEmail.Size = new System.Drawing.Size(200, 21);
            this.cbEmail.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(247, 27);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Email：";
            // 
            // cbName
            // 
            this.cbName.FormattingEnabled = true;
            this.cbName.Location = new System.Drawing.Point(15, 54);
            this.cbName.Margin = new System.Windows.Forms.Padding(2);
            this.cbName.Name = "cbName";
            this.cbName.Size = new System.Drawing.Size(200, 21);
            this.cbName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name：";
            // 
            // cbMobile
            // 
            this.cbMobile.FormattingEnabled = true;
            this.cbMobile.Location = new System.Drawing.Point(718, 54);
            this.cbMobile.Margin = new System.Windows.Forms.Padding(2);
            this.cbMobile.Name = "cbMobile";
            this.cbMobile.Size = new System.Drawing.Size(200, 21);
            this.cbMobile.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(716, 27);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Mobile：";
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(658, 225);
            this.btnUpload.Margin = new System.Windows.Forms.Padding(2);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(94, 28);
            this.btnUpload.TabIndex = 9;
            this.btnUpload.Text = "Upload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // cbRole
            // 
            this.cbRole.FormattingEnabled = true;
            this.cbRole.Location = new System.Drawing.Point(16, 125);
            this.cbRole.Margin = new System.Windows.Forms.Padding(2);
            this.cbRole.Name = "cbRole";
            this.cbRole.Size = new System.Drawing.Size(200, 21);
            this.cbRole.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 98);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Role：";
            // 
            // cbKindergarten
            // 
            this.cbKindergarten.FormattingEnabled = true;
            this.cbKindergarten.Location = new System.Drawing.Point(250, 125);
            this.cbKindergarten.Margin = new System.Windows.Forms.Padding(2);
            this.cbKindergarten.Name = "cbKindergarten";
            this.cbKindergarten.Size = new System.Drawing.Size(200, 21);
            this.cbKindergarten.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(248, 98);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Kindergarten：";
            // 
            // ImportUsers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 586);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnPreImport);
            this.Controls.Add(this.btnImport);
            this.Name = "ImportUsers";
            this.Text = "Import Users";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbSex;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbEmail;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPreImport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.ComboBox cbMobile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.ComboBox cbRole;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbKindergarten;
        private System.Windows.Forms.Label label6;
    }
}