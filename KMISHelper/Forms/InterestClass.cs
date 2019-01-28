using KMISHelper.Business;
using KMISHelper.HelpGlobal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KMISHelper.Forms
{
    public partial class InterestClass : Form
    {
        public InterestClass()
        {
            InitializeComponent();
        }

        private BznsInterestClass ic;
        private BznsInterestClass GetInterestClassInstance()
        {
            if (ic == null)
            {
                ic = new BznsInterestClass();
            }
            return ic;
        }

        private DataTable Classes = null;

        private DataTable Students = null;

        private bool IsTry2Import = false;

        private bool IsTry2 = false;

        #region Interest

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Upload Interest Class";
            of.InitialDirectory = @"c:\";
            of.Filter = "Excel Files(.xls)| *.xls";
            of.FilterIndex = 2;

            of.RestoreDirectory = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadField(of.FileName);
                LoadData(of.FileName);
            }
        }

        private void LoadField(string FileName)
        {

            cbClassName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbClassName.DisplayMember = "Value";
            cbClassName.ValueMember = "Key";

            cbType.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbType.DisplayMember = "Value";
            cbType.ValueMember = "Key";

            cbStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbStartDate.DisplayMember = "Value";
            cbStartDate.ValueMember = "Key";

            cbEndDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbEndDate.DisplayMember = "Value";
            cbEndDate.ValueMember = "Key";

            cbHours.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbHours.DisplayMember = "Value";
            cbHours.ValueMember = "Key";

            cbPrice.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbPrice.DisplayMember = "Value";
            cbPrice.ValueMember = "Key";

            cbAmount.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbAmount.DisplayMember = "Value";
            cbAmount.ValueMember = "Key";

            cbCategory.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbCategory.DisplayMember = "Value";
            cbCategory.ValueMember = "Key";

            cbStatus.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbStatus.DisplayMember = "Value";
            cbStatus.ValueMember = "Key";

            cbChargeType.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassSheetName);
            cbChargeType.DisplayMember = "Value";
            cbChargeType.ValueMember = "Key";            

        }

        private void LoadData(string FileName)
        {
            Classes = ExcelHelper.ExcelHelper.Import(FileName, BznsBase.ImportInterestClassSheetName);
        }

        private void btnPreImport_Click(object sender, EventArgs e)
        {
            IsTry2Import = true;
            btnImport_Click(null, null);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

            var Result = false;
            var Message = "Oh, import failed please review error log.";
            var ClassNamePos = Convert.ToInt32(cbClassName.SelectedValue);
            var TypePos = Convert.ToInt32(cbType.SelectedValue);
            var StartDatePos = Convert.ToInt32(cbStartDate.SelectedValue);
            var EndDatePos = Convert.ToInt32(cbEndDate.SelectedValue);
            var HoursPos = Convert.ToInt32(cbHours.SelectedValue);
            var PricePos = Convert.ToInt32(cbPrice.SelectedValue);
            var AmountPos = Convert.ToInt32(cbAmount.SelectedValue);
            var CategoryNamePos = Convert.ToInt32(cbCategory.SelectedValue);
            var StatusPos = Convert.ToInt32(cbStatus.SelectedValue);
            var ChargeTypePos = Convert.ToInt32(cbChargeType.SelectedValue);

            try
            {

                if (GetInterestClassInstance().InterestInsert(Classes,ClassNamePos, TypePos, StartDatePos, EndDatePos, HoursPos, PricePos, AmountPos, CategoryNamePos, StatusPos, ChargeTypePos, IsTry2Import))
                {
                    Message = "Import Interest Class Successfully!";
                }
                else
                {
                    SysLog.Insert(new SysLogInfo("Import Interest Class Fail.", SysLogType.ERROR, "Import Interest Class FRM"));
                }

            }
            catch (Exception ex)
            {
                SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, "Import Interest Class FRM"));
            }
            finally {
                IsTry2Import = false;
                MessageBox.Show(Message);
            }
                
        }


        #endregion

        #region Interest Student
        private void isbtnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Upload Interest Students";
            of.InitialDirectory = @BznsBase.InitialDirectory;
            of.Filter = "Excel Files(.xls)| *.xls";
            of.FilterIndex = 2;

            of.RestoreDirectory = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadStudentField(of.FileName);
                LoadStudentData(of.FileName);
            }

        }

        private void LoadStudentField(string FileName) {

            iscbStudentNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassFinanceSheetName);
            iscbStudentNo.DisplayMember = "Value";
            iscbStudentNo.ValueMember = "Key";

            iscbYear.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassFinanceSheetName);
            iscbYear.DisplayMember = "Value";
            iscbYear.ValueMember = "Key";

            iscbCourseName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassFinanceSheetName);
            iscbCourseName.DisplayMember = "Value";
            iscbCourseName.ValueMember = "Key";

            iscbClassHours.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassFinanceSheetName);
            iscbClassHours.DisplayMember = "Value";
            iscbClassHours.ValueMember = "Key";

            iscbMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassFinanceSheetName);
            iscbMoney.DisplayMember = "Value";
            iscbMoney.ValueMember = "Key";

            iscbPaid.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassFinanceSheetName);
            iscbPaid.DisplayMember = "Value";
            iscbPaid.ValueMember = "Key";

            iscbStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassFinanceSheetName);
            iscbStartDate.DisplayMember = "Value";
            iscbStartDate.ValueMember = "Key";

            iscbEndDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportInterestClassFinanceSheetName);
            iscbEndDate.DisplayMember = "Value";
            iscbEndDate.ValueMember = "Key";
        }

        private void LoadStudentData(string FileName) {
            Students = ExcelHelper.ExcelHelper.Import(FileName, BznsBase.ImportInterestClassFinanceSheetName);
        }


        #endregion

        private void isbtnImport_Click(object sender, EventArgs e)
        {

            var Result = false;
            var Message = "Oh, import failed please review error log.";
            var StudentNoPos = Convert.ToInt32(iscbStudentNo.SelectedValue);
            var YearPos = Convert.ToInt32(iscbYear.SelectedValue);
            var CourseNamePos = Convert.ToInt32(iscbCourseName.SelectedValue);
            var ClassHoursPos = Convert.ToInt32(iscbClassHours.SelectedValue);
            var MoneyPos = Convert.ToInt32(iscbMoney.SelectedValue);
            var PaidPos = Convert.ToInt32(iscbPaid.SelectedValue);
            var StartDatePos = Convert.ToInt32(iscbStartDate.SelectedValue);
            var EndDatePos = Convert.ToInt32(iscbEndDate.SelectedValue);

            try
            {

                if (StudentNoPos == -1)
                {
                    Message = "Please select your student no.";
                    return;
                }

                if (YearPos == -1)
                {
                    Message = "Please select your acad year.";
                    return;
                }

                if (CourseNamePos == -1)
                {
                    Message = "Please select your course name.";
                    return;
                }

                if (ClassHoursPos == -1)
                {
                    Message = "Please select your class hours.";
                    return;
                }

                if (MoneyPos == -1)
                {
                    Message = "Please select your money.";
                    return;
                }

                if (GetInterestClassInstance().StudentInsert(Students,StudentNoPos,YearPos,CourseNamePos,ClassHoursPos,MoneyPos, PaidPos, StartDatePos,EndDatePos,IsTry2))
                {
                    Message = "Import Interest Class Student Successfully!";
                }
                else
                {
                    SysLog.Insert(new SysLogInfo("Import Interest Class Student Fail.", SysLogType.ERROR, "Import Interest Class FRM"));
                }

            }
            catch (Exception ex)
            {
                SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, "Import Interest Class FRM"));
            }
            finally
            {
                IsTry2 = false;
                MessageBox.Show(Message);
            }

        }

        private void isbtnPreImport_Click(object sender, EventArgs e)
        {
            IsTry2 = true;
            isbtnImport_Click(null, null);
        }
    }
}
