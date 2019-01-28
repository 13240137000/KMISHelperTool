namespace KMISHelper.Forms
{
    partial class Options
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.General = new System.Windows.Forms.TabPage();
            this.Export = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnGeneralSave = new System.Windows.Forms.Button();
            this.cbBrand = new System.Windows.Forms.ComboBox();
            this.cbKindergarten = new System.Windows.Forms.ComboBox();
            this.cbBrandUser = new System.Windows.Forms.ComboBox();
            this.cbKindergartenUser = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtClassPath = new System.Windows.Forms.TextBox();
            this.txtErrorLog = new System.Windows.Forms.TextBox();
            this.txtInfoLog = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtWarningLog = new System.Windows.Forms.TextBox();
            this.lblWarningLog = new System.Windows.Forms.Label();
            this.btnExportSave = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.General.SuspendLayout();
            this.Export.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.General);
            this.tabControl1.Controls.Add(this.Export);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(987, 562);
            this.tabControl1.TabIndex = 0;
            // 
            // General
            // 
            this.General.Controls.Add(this.cbKindergartenUser);
            this.General.Controls.Add(this.cbBrandUser);
            this.General.Controls.Add(this.cbKindergarten);
            this.General.Controls.Add(this.cbBrand);
            this.General.Controls.Add(this.btnGeneralSave);
            this.General.Controls.Add(this.label4);
            this.General.Controls.Add(this.label3);
            this.General.Controls.Add(this.label2);
            this.General.Controls.Add(this.label1);
            this.General.Location = new System.Drawing.Point(4, 22);
            this.General.Name = "General";
            this.General.Padding = new System.Windows.Forms.Padding(3);
            this.General.Size = new System.Drawing.Size(979, 536);
            this.General.TabIndex = 0;
            this.General.Text = "General";
            this.General.UseVisualStyleBackColor = true;
            // 
            // Export
            // 
            this.Export.Controls.Add(this.btnExportSave);
            this.Export.Controls.Add(this.txtWarningLog);
            this.Export.Controls.Add(this.lblWarningLog);
            this.Export.Controls.Add(this.txtInfoLog);
            this.Export.Controls.Add(this.label7);
            this.Export.Controls.Add(this.txtErrorLog);
            this.Export.Controls.Add(this.txtClassPath);
            this.Export.Controls.Add(this.label6);
            this.Export.Controls.Add(this.label5);
            this.Export.Location = new System.Drawing.Point(4, 22);
            this.Export.Name = "Export";
            this.Export.Padding = new System.Windows.Forms.Padding(3);
            this.Export.Size = new System.Drawing.Size(979, 536);
            this.Export.TabIndex = 1;
            this.Export.Text = "Export";
            this.Export.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Brand：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(538, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Kindergarten：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Brand User：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(538, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Kindergarten User：";
            // 
            // btnGeneralSave
            // 
            this.btnGeneralSave.Location = new System.Drawing.Point(863, 493);
            this.btnGeneralSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnGeneralSave.Name = "btnGeneralSave";
            this.btnGeneralSave.Size = new System.Drawing.Size(94, 28);
            this.btnGeneralSave.TabIndex = 8;
            this.btnGeneralSave.Text = "Save";
            this.btnGeneralSave.UseVisualStyleBackColor = true;
            this.btnGeneralSave.Click += new System.EventHandler(this.btnGeneralSave_Click);
            // 
            // cbBrand
            // 
            this.cbBrand.FormattingEnabled = true;
            this.cbBrand.Location = new System.Drawing.Point(20, 55);
            this.cbBrand.Name = "cbBrand";
            this.cbBrand.Size = new System.Drawing.Size(416, 21);
            this.cbBrand.TabIndex = 9;
            // 
            // cbKindergarten
            // 
            this.cbKindergarten.FormattingEnabled = true;
            this.cbKindergarten.Location = new System.Drawing.Point(541, 55);
            this.cbKindergarten.Name = "cbKindergarten";
            this.cbKindergarten.Size = new System.Drawing.Size(416, 21);
            this.cbKindergarten.TabIndex = 10;
            // 
            // cbBrandUser
            // 
            this.cbBrandUser.FormattingEnabled = true;
            this.cbBrandUser.Location = new System.Drawing.Point(20, 124);
            this.cbBrandUser.Name = "cbBrandUser";
            this.cbBrandUser.Size = new System.Drawing.Size(416, 21);
            this.cbBrandUser.TabIndex = 11;
            // 
            // cbKindergartenUser
            // 
            this.cbKindergartenUser.FormattingEnabled = true;
            this.cbKindergartenUser.Location = new System.Drawing.Point(541, 124);
            this.cbKindergartenUser.Name = "cbKindergartenUser";
            this.cbKindergartenUser.Size = new System.Drawing.Size(416, 21);
            this.cbKindergartenUser.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Save Class Path：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(532, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Save Error Log Path：";
            // 
            // txtClassPath
            // 
            this.txtClassPath.Location = new System.Drawing.Point(27, 53);
            this.txtClassPath.Name = "txtClassPath";
            this.txtClassPath.Size = new System.Drawing.Size(416, 20);
            this.txtClassPath.TabIndex = 14;
            // 
            // txtErrorLog
            // 
            this.txtErrorLog.Location = new System.Drawing.Point(535, 53);
            this.txtErrorLog.Name = "txtErrorLog";
            this.txtErrorLog.Size = new System.Drawing.Size(416, 20);
            this.txtErrorLog.TabIndex = 15;
            // 
            // txtInfoLog
            // 
            this.txtInfoLog.Location = new System.Drawing.Point(27, 123);
            this.txtInfoLog.Name = "txtInfoLog";
            this.txtInfoLog.Size = new System.Drawing.Size(416, 20);
            this.txtInfoLog.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(111, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Save Info Log Path：";
            // 
            // txtWarningLog
            // 
            this.txtWarningLog.Location = new System.Drawing.Point(535, 123);
            this.txtWarningLog.Name = "txtWarningLog";
            this.txtWarningLog.Size = new System.Drawing.Size(416, 20);
            this.txtWarningLog.TabIndex = 19;
            // 
            // lblWarningLog
            // 
            this.lblWarningLog.AutoSize = true;
            this.lblWarningLog.Location = new System.Drawing.Point(532, 93);
            this.lblWarningLog.Name = "lblWarningLog";
            this.lblWarningLog.Size = new System.Drawing.Size(133, 13);
            this.lblWarningLog.TabIndex = 18;
            this.lblWarningLog.Text = "Save Warning Log Path：";
            // 
            // btnExportSave
            // 
            this.btnExportSave.Location = new System.Drawing.Point(857, 493);
            this.btnExportSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportSave.Name = "btnExportSave";
            this.btnExportSave.Size = new System.Drawing.Size(94, 28);
            this.btnExportSave.TabIndex = 20;
            this.btnExportSave.Text = "Save";
            this.btnExportSave.UseVisualStyleBackColor = true;
            this.btnExportSave.Click += new System.EventHandler(this.btnExportSave_Click);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 586);
            this.Controls.Add(this.tabControl1);
            this.Name = "Options";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.tabControl1.ResumeLayout(false);
            this.General.ResumeLayout(false);
            this.General.PerformLayout();
            this.Export.ResumeLayout(false);
            this.Export.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage General;
        private System.Windows.Forms.TabPage Export;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGeneralSave;
        private System.Windows.Forms.ComboBox cbKindergartenUser;
        private System.Windows.Forms.ComboBox cbBrandUser;
        private System.Windows.Forms.ComboBox cbKindergarten;
        private System.Windows.Forms.ComboBox cbBrand;
        private System.Windows.Forms.TextBox txtWarningLog;
        private System.Windows.Forms.Label lblWarningLog;
        private System.Windows.Forms.TextBox txtInfoLog;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtErrorLog;
        private System.Windows.Forms.TextBox txtClassPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnExportSave;
    }
}