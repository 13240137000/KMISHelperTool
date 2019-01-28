namespace KMISHelper.Forms
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.ms = new System.Windows.Forms.MenuStrip();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importStudentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teacherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interestClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.warningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ss = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.financeForShangHaiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ms.SuspendLayout();
            this.ss.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ms
            // 
            this.ms.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.ms.Location = new System.Drawing.Point(0, 0);
            this.ms.Name = "ms";
            this.ms.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.ms.Size = new System.Drawing.Size(1011, 28);
            this.ms.TabIndex = 0;
            this.ms.Text = "menuStrip1";
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalToolStripMenuItem});
            this.configurationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("configurationToolStripMenuItem.Image")));
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(76, 24);
            this.configurationToolStripMenuItem.Text = "Setting";
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("generalToolStripMenuItem.Image")));
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.generalToolStripMenuItem.Text = "General";
            this.generalToolStripMenuItem.Click += new System.EventHandler(this.generalToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importClassToolStripMenuItem,
            this.importStudentToolStripMenuItem,
            this.teacherToolStripMenuItem,
            this.importFinToolStripMenuItem,
            this.interestClassToolStripMenuItem,
            this.usersToolStripMenuItem,
            this.financeForShangHaiToolStripMenuItem});
            this.importToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importToolStripMenuItem.Image")));
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(75, 24);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // importClassToolStripMenuItem
            // 
            this.importClassToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importClassToolStripMenuItem.Image")));
            this.importClassToolStripMenuItem.Name = "importClassToolStripMenuItem";
            this.importClassToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.importClassToolStripMenuItem.Text = "Class";
            this.importClassToolStripMenuItem.Click += new System.EventHandler(this.importClassToolStripMenuItem_Click);
            // 
            // importStudentToolStripMenuItem
            // 
            this.importStudentToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importStudentToolStripMenuItem.Image")));
            this.importStudentToolStripMenuItem.Name = "importStudentToolStripMenuItem";
            this.importStudentToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.importStudentToolStripMenuItem.Text = "Student";
            this.importStudentToolStripMenuItem.Click += new System.EventHandler(this.importStudentToolStripMenuItem_Click);
            // 
            // teacherToolStripMenuItem
            // 
            this.teacherToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("teacherToolStripMenuItem.Image")));
            this.teacherToolStripMenuItem.Name = "teacherToolStripMenuItem";
            this.teacherToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.teacherToolStripMenuItem.Text = "Teacher";
            this.teacherToolStripMenuItem.Click += new System.EventHandler(this.teacherToolStripMenuItem_Click);
            // 
            // importFinToolStripMenuItem
            // 
            this.importFinToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importFinToolStripMenuItem.Image")));
            this.importFinToolStripMenuItem.Name = "importFinToolStripMenuItem";
            this.importFinToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.importFinToolStripMenuItem.Text = "Finance";
            this.importFinToolStripMenuItem.Click += new System.EventHandler(this.importFinToolStripMenuItem_Click);
            // 
            // interestClassToolStripMenuItem
            // 
            this.interestClassToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("interestClassToolStripMenuItem.Image")));
            this.interestClassToolStripMenuItem.Name = "interestClassToolStripMenuItem";
            this.interestClassToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.interestClassToolStripMenuItem.Text = "Interest Class";
            this.interestClassToolStripMenuItem.Click += new System.EventHandler(this.interestClassToolStripMenuItem_Click);
            // 
            // usersToolStripMenuItem
            // 
            this.usersToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("usersToolStripMenuItem.Image")));
            this.usersToolStripMenuItem.Name = "usersToolStripMenuItem";
            this.usersToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.usersToolStripMenuItem.Text = "Users";
            this.usersToolStripMenuItem.Click += new System.EventHandler(this.usersToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportClassToolStripMenuItem});
            this.exportToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportToolStripMenuItem.Image")));
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // exportClassToolStripMenuItem
            // 
            this.exportClassToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportClassToolStripMenuItem.Image")));
            this.exportClassToolStripMenuItem.Name = "exportClassToolStripMenuItem";
            this.exportClassToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.exportClassToolStripMenuItem.Text = "Export Class";
            this.exportClassToolStripMenuItem.Click += new System.EventHandler(this.exportClassToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem,
            this.warningToolStripMenuItem,
            this.errorToolStripMenuItem});
            this.helpToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripMenuItem.Image")));
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(59, 24);
            this.helpToolStripMenuItem.Text = "Log";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("infoToolStripMenuItem.Image")));
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.infoToolStripMenuItem.Text = "Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click_1);
            // 
            // warningToolStripMenuItem
            // 
            this.warningToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("warningToolStripMenuItem.Image")));
            this.warningToolStripMenuItem.Name = "warningToolStripMenuItem";
            this.warningToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.warningToolStripMenuItem.Text = "Warning";
            this.warningToolStripMenuItem.Click += new System.EventHandler(this.warningToolStripMenuItem_Click);
            // 
            // errorToolStripMenuItem
            // 
            this.errorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("errorToolStripMenuItem.Image")));
            this.errorToolStripMenuItem.Name = "errorToolStripMenuItem";
            this.errorToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.errorToolStripMenuItem.Text = "Error";
            this.errorToolStripMenuItem.Click += new System.EventHandler(this.errorToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripMenuItem1.Image")));
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(64, 24);
            this.helpToolStripMenuItem1.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(217, 17);
            this.toolStripStatusLabel1.Text = "Copyright (c) JEDU Group Code By Jack.";
            // 
            // ss
            // 
            this.ss.AllowDrop = true;
            this.ss.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ss.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3});
            this.ss.Location = new System.Drawing.Point(0, 564);
            this.ss.Name = "ss";
            this.ss.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.ss.ShowItemToolTips = true;
            this.ss.Size = new System.Drawing.Size(1011, 22);
            this.ss.TabIndex = 1;
            this.ss.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(745, 17);
            this.toolStripStatusLabel2.Text = resources.GetString("toolStripStatusLabel2.Text");
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(28, 17);
            this.toolStripStatusLabel3.Text = "v1.0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 33);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1011, 530);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // financeForShangHaiToolStripMenuItem
            // 
            this.financeForShangHaiToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("financeForShangHaiToolStripMenuItem.Image")));
            this.financeForShangHaiToolStripMenuItem.Name = "financeForShangHaiToolStripMenuItem";
            this.financeForShangHaiToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.financeForShangHaiToolStripMenuItem.Text = "Finance For ShangHai";
            this.financeForShangHaiToolStripMenuItem.Click += new System.EventHandler(this.financeForShangHaiToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 586);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ss);
            this.Controls.Add(this.ms);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.ms;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.Text = "KMIS Helper";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ms.ResumeLayout(false);
            this.ms.PerformLayout();
            this.ss.ResumeLayout(false);
            this.ss.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip ms;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importStudentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem warningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.StatusStrip ss;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teacherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem interestClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem financeForShangHaiToolStripMenuItem;
    }
}