using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KMISHelper.Business;
using KMISHelper.ExcelHelper;
using KMISHelper.HelpGlobal;

namespace KMISHelper.Forms
{
    public partial class ImportFinance : Form
    {
        public ImportFinance()
        {
            InitializeComponent();
        }

        private BznsFinance bz;
        private BznsFinance GetFinanceInstance()
        {
            if (bz == null)
            {
                bz = new BznsFinance();
            }
            return bz;
        }

        private DataTable Students = null;

        private bool IsTry2Import = false;

        private void btnUpload_Click(object sender, EventArgs e)
        {

            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Upload Finance";
            of.InitialDirectory = @BznsBase.InitialDirectory;
            of.Filter = "Excel Files(.xls)| *.xls";
            of.FilterIndex = 2;

            of.RestoreDirectory = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadField(of.FileName);
                LoadPlans(of.FileName);

            }

        }

        private void LoadPlans(string FileName)
        {
            Students = ExcelHelper.ExcelHelper.Import(FileName, BznsBase.ImportFinanceSheetName);
        }

        private void LoadField(string FileName) {

            // Load Plan

            cbStudentNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName,BznsBase.ImportFinanceSheetName);
            cbStudentNo.DisplayMember = "Value";
            cbStudentNo.ValueMember = "Key";

            //cbStudentNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            //cbStudentNo.DisplayMember = "Value";
            //cbStudentNo.ValueMember = "Key";

            cbYear.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbYear.DisplayMember = "Value";
            cbYear.ValueMember = "Key";

            cbMonth.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbMonth.DisplayMember = "Value";
            cbMonth.ValueMember = "Key";

            cbBillStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbBillStartDate.DisplayMember = "Value";
            cbBillStartDate.ValueMember = "Key";

            if (BznsBase.EnableMappingField) {
                cbStudentNo.Text = "学号";
                cbYear.Text = "学年";
                cbMonth.Text = "月份数";
                cbBillStartDate.Text = "入园时间";
            }

            // Load Tuition Fee

            cbtfPaymentMethod.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbtfPaymentMethod.DisplayMember = "Value";
            cbtfPaymentMethod.ValueMember = "Key";

            cbtfTuitionFeeMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbtfTuitionFeeMoney.DisplayMember = "Value";
            cbtfTuitionFeeMoney.ValueMember = "Key";

            cbtfDiscountMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbtfDiscountMoney.DisplayMember = "Value";
            cbtfDiscountMoney.ValueMember = "Key";

            cbDiscountName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbDiscountName.DisplayMember = "Value";
            cbDiscountName.ValueMember = "Key";

            cbtfStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbtfStartDate.DisplayMember = "Value";
            cbtfStartDate.ValueMember = "Key";

            if (BznsBase.EnableMappingField)
            {
                cbtfPaymentMethod.Text = "缴费方式";
                cbtfTuitionFeeMoney.Text = "标准学费";
                cbtfDiscountMoney.Text = "折扣学费";
                cbDiscountName.Text = "折扣名称";
                cbtfStartDate.Text = "入园时间";
            }

            // Load Per Tuition Fee

            cbptfmPerTuitionFeeMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbptfmPerTuitionFeeMoney.DisplayMember = "Value";
            cbptfmPerTuitionFeeMoney.ValueMember = "Key";

            if (BznsBase.EnableMappingField) {
                cbptfmPerTuitionFeeMoney.Text = "预交学费";
            }

            // Meals Fee 

            cbmfMethod.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbmfMethod.DisplayMember = "Value";
            cbmfMethod.ValueMember = "Key";

            cbmfMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbmfMoney.DisplayMember = "Value";
            cbmfMoney.ValueMember = "Key";

            cbmfStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbmfStartDate.DisplayMember = "Value";
            cbmfStartDate.ValueMember = "Key";

            if (BznsBase.EnableMappingField) {
                cbmfMethod.Text = "缴费方式";
                cbmfMoney.Text = "餐费标准";
                cbmfStartDate.Text = "入园时间";
            }

            // School Car

            cbscMethod.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbscMethod.DisplayMember = "Value";
            cbscMethod.ValueMember = "Key";

            cbscMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbscMoney.DisplayMember = "Value";
            cbscMoney.ValueMember = "Key";

            cbscStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbscStartDate.DisplayMember = "Value";
            cbscStartDate.ValueMember = "Key";

            cbscMonth.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbscMonth.DisplayMember = "Value";
            cbscMonth.ValueMember = "Key";

            if (BznsBase.EnableMappingField) {
                cbscMethod.Text = "缴费方式";
                cbscMoney.Text = "车费标准";
                cbscStartDate.Text = "校车时间";
                cbscMonth.Text = "校车月份";
            }

            // Once Fee

            cbofMoney1.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbofMoney1.DisplayMember = "Value";
            cbofMoney1.ValueMember = "Key";

            cbofMoney2.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbofMoney2.DisplayMember = "Value";
            cbofMoney2.ValueMember = "Key";

            cbofMoney3.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbofMoney3.DisplayMember = "Value";
            cbofMoney3.ValueMember = "Key";

            cbofMoney4.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbofMoney4.DisplayMember = "Value";
            cbofMoney4.ValueMember = "Key";

            cbofMoney5.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbofMoney5.DisplayMember = "Value";
            cbofMoney5.ValueMember = "Key";

            cbofMoney6.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbofMoney6.DisplayMember = "Value";
            cbofMoney6.ValueMember = "Key";

            cbofMoney7.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportFinanceSheetName);
            cbofMoney7.DisplayMember = "Value";
            cbofMoney7.ValueMember = "Key";

            if (BznsBase.EnableMappingField) {
                cbofMoney1.Text = "校服费";
                cbofMoney2.Text = "被褥费";
                cbofMoney3.Text = "书本费";
                cbofMoney4.Text = "书包费";
                cbofMoney5.Text = "保险费";
                cbofMoney6.Text = "空调费";
                cbofMoney7.Text = "其它收费";
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {

            btnImport.Enabled = false;

            var Result = false;
            var Message = "Oh, import failed please review error log.";
            var StudentNoPos = Convert.ToInt32(cbStudentNo.SelectedValue);
            var YearTitlePos = Convert.ToInt32(cbYear.SelectedValue);
            var MonthPos = Convert.ToInt32(cbMonth.SelectedValue);
            var BillStartDatePos = Convert.ToInt32(cbBillStartDate.SelectedValue);

            var PaymentMethodPos = Convert.ToInt32(cbtfPaymentMethod.SelectedValue);
            var TuitionFeeMoneyPos = Convert.ToInt32(cbtfTuitionFeeMoney.SelectedValue);
            var DiscountMoneyPos = Convert.ToInt32(cbtfDiscountMoney.SelectedValue);
            var DiscountNamePos = Convert.ToInt32(cbDiscountName.SelectedValue);
            var StartDatePos = Convert.ToInt32(cbtfStartDate.SelectedValue);
            var PerTuitionFeeMoneyPos = Convert.ToInt32(cbptfmPerTuitionFeeMoney.SelectedValue);

            var mfMethodPos = Convert.ToInt32(cbmfMethod.SelectedValue);
            var mfMoneyPos = Convert.ToInt32(cbmfMoney.SelectedValue);
            var mfStartDatePos = Convert.ToInt32(cbmfStartDate.SelectedValue);

            var scMethodPos = Convert.ToInt32(cbscMethod.SelectedValue);
            var scMoneyPos = Convert.ToInt32(cbscMoney.SelectedValue);
            var scStartDatePos = Convert.ToInt32(cbscStartDate.SelectedValue);
            var scMonthPos = Convert.ToInt32(cbscMonth.SelectedValue);

            var ofMoney1Pos = Convert.ToInt32(cbofMoney1.SelectedValue);
            var ofMoney2Pos = Convert.ToInt32(cbofMoney2.SelectedValue);
            var ofMoney3Pos = Convert.ToInt32(cbofMoney3.SelectedValue);
            var ofMoney4Pos = Convert.ToInt32(cbofMoney4.SelectedValue);
            var ofMoney5Pos = Convert.ToInt32(cbofMoney5.SelectedValue);
            var ofMoney6Pos = Convert.ToInt32(cbofMoney6.SelectedValue);
            var ofMoney7Pos = Convert.ToInt32(cbofMoney7.SelectedValue);

            var ofs = new List<OnceInfo>();

            try
            {

                if (BillStartDatePos == -1) {
                    Message = "Please select your bill start date.";
                    return;
                }

                if (StudentNoPos == -1)
                {
                    Message = "Please select your student no.";
                    return;
                }

                if (YearTitlePos == -1)
                {
                    Message = "Please select your school year.";
                    return;
                }

                if (MonthPos == -1)
                {
                    Message = "Please select your school year.";
                    return;
                }

                if (ofMoney1Pos != -1) {
                    ofs.Add(new OnceInfo() { Key = ofMoney1Pos,Value = cbofMoney1.Text.Trim()});
                }

                if (ofMoney2Pos != -1)
                {
                    ofs.Add(new OnceInfo() { Key = ofMoney2Pos, Value = cbofMoney2.Text.Trim() });
                }

                if (ofMoney3Pos != -1)
                {
                    ofs.Add(new OnceInfo() { Key = ofMoney3Pos, Value = cbofMoney3.Text.Trim() });
                }

                if (ofMoney4Pos != -1)
                {
                    ofs.Add(new OnceInfo() { Key = ofMoney4Pos, Value = cbofMoney4.Text.Trim() });
                }

                if (ofMoney5Pos != -1)
                {
                    ofs.Add(new OnceInfo() { Key = ofMoney5Pos, Value = cbofMoney5.Text.Trim() });
                }

                if (ofMoney6Pos != -1)
                {
                    ofs.Add(new OnceInfo() { Key = ofMoney6Pos, Value = cbofMoney6.Text.Trim() });
                }

                if (ofMoney7Pos != -1)
                {
                    ofs.Add(new OnceInfo() { Key = ofMoney7Pos, Value = cbofMoney7.Text.Trim() });
                }


                Result = GetFinanceInstance().ChargeInsert(Students, StudentNoPos, true);

                //Result = GetFinanceInstance().BillInsert(Students, StudentNoPos, YearTitlePos, MonthPos, BillStartDatePos, PaymentMethodPos, TuitionFeeMoneyPos, DiscountMoneyPos, DiscountNamePos, StartDatePos, PerTuitionFeeMoneyPos, mfMethodPos, mfMoneyPos, mfStartDatePos, scMethodPos, scMoneyPos, scMonthPos, scStartDatePos, ofs, IsTry2Import);


                //if (Result && !IsTry2Import)
                //{
                //    Result = GetFinanceInstance().ChargeInsert(Students, StudentNoPos, false);
                //}
                //else
                //{
                //    SysLog.Insert(new SysLogInfo("Import Payment Fail.", SysLogType.ERROR, "Import Finance FRM"));
                //}

                if (Result)
                {
                    Message = "Import Finance Successfully!";
                }
                else
                {
                    SysLog.Insert(new SysLogInfo("Import Finance Fail.", SysLogType.ERROR, "Import Finance FRM"));
                }

            }
            catch (Exception ex)
            {
                SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, "Import Finance FRM"));
            }
            finally
            {
                IsTry2Import = false;
                ofs = null;
                btnImport.Enabled = true;
                MessageBox.Show(Message);
            }

        }

        private void btnPreImport_Click(object sender, EventArgs e)
        {
            IsTry2Import = true;
            btnImport_Click(null, null);
        }

    }
}
