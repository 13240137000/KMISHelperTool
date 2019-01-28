using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KMISHelper.DBScript;
using KMISHelper.DBHelper;
using KMISHelper.HelpGlobal;
using KMISHelper.Business;
using System.Configuration;

namespace KMISHelper.Forms
{
    public partial class Main : Form
    {

        public Main()
        {
            InitializeComponent();

           

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {

        
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Type your code.
            // InitObject.SplitMonth(DateTime.Parse("2018-11-01"), 4);
            // InitObject.TotalStudyOfMonth(DateTime.Parse("2018-10-01"),DateTime.Parse("2018-10-31"), "fcee561085a8492e9071d9ba1e6ece69", "101");
            //InitObject.GetPaymentInfo();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var frmSysLog = new SystemLog();
            frmSysLog.Show();

        }

        private void importClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmImportClass = new ImportClass();
            frmImportClass.Show();
        }

        private void importStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmImportStudent = new ImportStudent();
            frmImportStudent.Show();
        }

        private void exportClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var myClass = new BznsClass();
            var FilePath = ConfigurationManager.AppSettings["ExportClassFilePath"];
            var Message = "We're sorry, export class list error.";

            try
            {
                var dt = myClass.GetClassList();
                ExcelHelper.ExcelHelper.DataTableToExcel(dt, string.Empty, FilePath);
                Message = string.Concat("Successfully,Please goto your location of ",FilePath," to view.");
            }
            catch (Exception ex)
            {
                SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, "Export Class FRM"));                
            }
            finally {
                myClass = null;
                MessageBox.Show(Message);
            }
            
        }

        private void infoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            var Message = "We're Sorry,Export Error!";

            if (SysLog.Export(SysLogType.INFO))
            {
                Message = "Successfully!";
            }

            MessageBox.Show(Message);

        }

        private void warningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Message = "We're Sorry,Export Error!";

            if (SysLog.Export(SysLogType.WARNING))
            {
                Message = "Successfully!";
            }

            MessageBox.Show(Message);
        }

        private void errorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Message = "We're Sorry,Export Error!";

            if (SysLog.Export(SysLogType.ERROR))
            {
                Message = "Successfully!";
            }

            MessageBox.Show(Message);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Cell:132 4013 7000");
        }

        private void importFinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmImportFinance = new ImportFinance();
            frmImportFinance.Show();
        }

        private void generalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmOptions = new Options();
            frmOptions.Show();
        }

        private void teacherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmImportTeacher = new ImportTeacher();
            frmImportTeacher.Show();
        }

        private void interestClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmInterestClass = new InterestClass();
            frmInterestClass.Show();
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmUsers = new ImportUsers();
            frmUsers.Show();
        }

        private void financeForShangHaiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmFFSH = new frmImportFinanceForShangHai();
            frmFFSH.Show();
        }
    }
}
