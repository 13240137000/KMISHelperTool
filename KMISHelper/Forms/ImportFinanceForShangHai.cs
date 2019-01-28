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
using KMISHelper.ExcelHelper;

namespace KMISHelper.Forms
{
    public partial class frmImportFinanceForShangHai : Form
    {
        public frmImportFinanceForShangHai()
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
            Students = ExcelHelper.ExcelHelper.Import(FileName, BznsBase.ImportForShangHai);
        }

        private void LoadField(string FileName) {

            // Load Global
        
            cbStudentNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName,BznsBase.ImportForShangHai);
            cbStudentNo.DisplayMember = "Value";
            cbStudentNo.ValueMember = "Key";

            cbYear.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            cbYear.DisplayMember = "Value";
            cbYear.ValueMember = "Key";

            cbStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            cbStartDate.DisplayMember = "Value";
            cbStartDate.ValueMember = "Key";

            tMonth.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            tMonth.DisplayMember = "Value";
            tMonth.ValueMember = "Key";

            // Load pt

            ptMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            ptMoney.DisplayMember = "Value";
            ptMoney.ValueMember = "Key";

            ptRateID.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            ptRateID.DisplayMember = "Value";
            ptRateID.ValueMember = "Key";

            // Pload p

            tMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            tMoney.DisplayMember = "Value";
            tMoney.ValueMember = "Key";

            tMoney1.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            tMoney1.DisplayMember = "Value";
            tMoney1.ValueMember = "Key";

            tMoney2.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            tMoney2.DisplayMember = "Value";
            tMoney2.ValueMember = "Key";

            tMoney3.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            tMoney3.DisplayMember = "Value";
            tMoney3.ValueMember = "Key";

            tMoney4.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            tMoney4.DisplayMember = "Value";
            tMoney4.ValueMember = "Key";

            tMoney5.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            tMoney5.DisplayMember = "Value";
            tMoney5.ValueMember = "Key";

            tRateID.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            tRateID.DisplayMember = "Value";
            tRateID.ValueMember = "Key";

            tStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            tStartDate.DisplayMember = "Value";
            tStartDate.ValueMember = "Key";

            // Load m

            mMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            mMoney.DisplayMember = "Value";
            mMoney.ValueMember = "Key";

            mMoney1.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            mMoney1.DisplayMember = "Value";
            mMoney1.ValueMember = "Key";

            mMoney2.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            mMoney2.DisplayMember = "Value";
            mMoney2.ValueMember = "Key";

            mMoney3.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            mMoney3.DisplayMember = "Value";
            mMoney3.ValueMember = "Key";

            mMoney4.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            mMoney4.DisplayMember = "Value";
            mMoney4.ValueMember = "Key";

            mMoney5.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            mMoney5.DisplayMember = "Value";
            mMoney5.ValueMember = "Key";

            mRateID.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            mRateID.DisplayMember = "Value";
            mRateID.ValueMember = "Key";

            mStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            mStartDate.DisplayMember = "Value";
            mStartDate.ValueMember = "Key";

            mMonth.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            mMonth.DisplayMember = "Value";
            mMonth.ValueMember = "Key";

            // Load sb

            sbMoney.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbMoney.DisplayMember = "Value";
            sbMoney.ValueMember = "Key";

            sbMoney1.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbMoney1.DisplayMember = "Value";
            sbMoney1.ValueMember = "Key";

            sbMoney2.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbMoney2.DisplayMember = "Value";
            sbMoney2.ValueMember = "Key";

            sbMoney3.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbMoney3.DisplayMember = "Value";
            sbMoney3.ValueMember = "Key";

            sbMoney4.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbMoney4.DisplayMember = "Value";
            sbMoney4.ValueMember = "Key";

            sbMoney5.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbMoney5.DisplayMember = "Value";
            sbMoney5.ValueMember = "Key";

            sbRateID.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbRateID.DisplayMember = "Value";
            sbRateID.ValueMember = "Key";

            sbStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbStartDate.DisplayMember = "Value";
            sbStartDate.ValueMember = "Key";

            sbMonth.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbMonth.DisplayMember = "Value";
            sbMonth.ValueMember = "Key";

            // Load o

            oMoney1.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            oMoney1.DisplayMember = "Value";
            oMoney1.ValueMember = "Key";

            oMoney2.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            oMoney2.DisplayMember = "Value";
            oMoney2.ValueMember = "Key";

            oMoney3.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            oMoney3.DisplayMember = "Value";
            oMoney3.ValueMember = "Key";

            oMoney4.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            oMoney4.DisplayMember = "Value";
            oMoney4.ValueMember = "Key";

            oMoney5.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            oMoney5.DisplayMember = "Value";
            oMoney5.ValueMember = "Key";

            oMoney6.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            oMoney6.DisplayMember = "Value";
            oMoney6.ValueMember = "Key";

            oMoney7.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            oMoney7.DisplayMember = "Value";
            oMoney7.ValueMember = "Key";

            sbStartDate.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportForShangHai);
            sbStartDate.DisplayMember = "Value";
            sbStartDate.ValueMember = "Key";

            if (BznsBase.EnableMappingField)
            {

                cbStudentNo.Text = "学号";
                cbYear.Text = "学年";
                cbStartDate.Text = "开始日期";
                

                ptMoney.Text = "预缴学费";
                ptRateID.Text = "预缴学费标准ID";

                tMoney.Text = "学费标准";
                tMoney1.Text = "学费1";
                tMoney2.Text = "学费2";
                tMoney3.Text = "学费3";
                tMoney4.Text = "学费4";
                tMoney5.Text = "学费5";
                tRateID.Text = "学费标准ID";
                tStartDate.Text = "学费开始日期";
                tMonth.Text = "学费月";

                mMoney.Text = "餐费标准";
                mMoney1.Text = "餐费1";
                mMoney2.Text = "餐费2";
                mMoney3.Text = "餐费3";
                mMoney4.Text = "餐费4";
                mMoney5.Text = "餐费5";
                mRateID.Text = "餐费标准ID";
                mStartDate.Text = "餐费开始日期";
                mMonth.Text = "餐费月";

                sbMoney.Text = "校车费标准";
                sbMoney1.Text = "校车费1";
                sbMoney2.Text = "校车费2";
                sbMoney3.Text = "校车费3";
                sbMoney4.Text = "校车费4";
                sbMoney5.Text = "校车费5";
                sbRateID.Text = "校车费标准ID";
                sbStartDate.Text = "校车费开始日期";
                sbMonth.Text = "校车月";

                oMoney1.Text = "校服费";
                oMoney2.Text = "被褥费";
                oMoney3.Text = "书本费";
                oMoney4.Text = "书包费";
                oMoney5.Text = "保险费";
                oMoney6.Text = "空调费";
                oMoney7.Text = "其它收费";

            }

        }

        private void btnPreImport_Click(object sender, EventArgs e)
        {
            IsTry2Import = true;
            btnImport_Click(null, null);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

            btnImport.Enabled = false;
            var Result = false;
            var Message = "Oh, import failed please review error log.";

            var StudentNoPos = Convert.ToInt32(cbStudentNo.SelectedValue);
            var YearPos = Convert.ToInt32(cbYear.SelectedValue);
            var StartDatePos = Convert.ToInt32(cbStartDate.SelectedValue);


            var PTMoneyPos = Convert.ToInt32(ptMoney.SelectedValue);
            var PTRateIDPos = Convert.ToInt32(ptRateID.SelectedValue);

            var TMoneyPos = Convert.ToInt32(tMoney.SelectedValue);
            var TMoney1Pos = Convert.ToInt32(tMoney1.SelectedValue);
            var TMoney2Pos = Convert.ToInt32(tMoney2.SelectedValue);
            var TMoney3Pos = Convert.ToInt32(tMoney3.SelectedValue);
            var TMoney4Pos = Convert.ToInt32(tMoney4.SelectedValue);
            var TMoney5Pos = Convert.ToInt32(tMoney5.SelectedValue);
            var TRateIDPos = Convert.ToInt32(tRateID.SelectedValue);
            var TStartDatePos = Convert.ToInt32(tStartDate.SelectedValue);
            var TMonthPos = Convert.ToInt32(tMonth.SelectedValue);

            var MMoneyPos = Convert.ToInt32(mMoney.SelectedValue);
            var MMoney1Pos = Convert.ToInt32(mMoney1.SelectedValue);
            var MMoney2Pos = Convert.ToInt32(mMoney2.SelectedValue);
            var MMoney3Pos = Convert.ToInt32(mMoney3.SelectedValue);
            var MMoney4Pos = Convert.ToInt32(mMoney4.SelectedValue);
            var MMoney5Pos = Convert.ToInt32(mMoney5.SelectedValue);
            var MRateIDPos = Convert.ToInt32(mRateID.SelectedValue);
            var MStartDatePos = Convert.ToInt32(mStartDate.SelectedValue);
            var MMonthPos = Convert.ToInt32(mMonth.SelectedValue);

            var SBMoneyPos = Convert.ToInt32(sbMoney.SelectedValue);
            var SBMoney1Pos = Convert.ToInt32(sbMoney1.SelectedValue);
            var SBMoney2Pos = Convert.ToInt32(sbMoney2.SelectedValue);
            var SBMoney3Pos = Convert.ToInt32(sbMoney3.SelectedValue);
            var SBMoney4Pos = Convert.ToInt32(sbMoney4.SelectedValue);
            var SBMoney5Pos = Convert.ToInt32(sbMoney5.SelectedValue);
            var SBRateIDPos = Convert.ToInt32(sbRateID.SelectedValue);
            var SBStartDatePos = Convert.ToInt32(sbStartDate.SelectedValue);
            var SBMonthPos = Convert.ToInt32(sbMonth.SelectedValue);

            var OMoney1Pos = Convert.ToInt32(oMoney1.SelectedValue);
            var OMoney2Pos = Convert.ToInt32(oMoney2.SelectedValue);
            var OMoney3Pos = Convert.ToInt32(oMoney3.SelectedValue);
            var OMoney4Pos = Convert.ToInt32(oMoney4.SelectedValue);
            var OMoney5Pos = Convert.ToInt32(oMoney5.SelectedValue);
            var OMoney6Pos = Convert.ToInt32(oMoney6.SelectedValue);
            var OMoney7Pos = Convert.ToInt32(oMoney7.SelectedValue);

            var tmi = new List<MoneyInfo>();

            if (TMoney1Pos != -1)
            {
                tmi.Add(new MoneyInfo() { Month = GetMonth("TMoney1Pos"), MoneyPos = TMoney1Pos });
            }

            if (TMoney2Pos != -1)
            {
                tmi.Add(new MoneyInfo() { Month = GetMonth("TMoney2Pos"), MoneyPos = TMoney2Pos });
            }

            if (TMoney3Pos != -1)
            {
                tmi.Add(new MoneyInfo() { Month = GetMonth("TMoney3Pos"), MoneyPos = TMoney3Pos });
            }

            if (TMoney4Pos != -1)
            {
                tmi.Add(new MoneyInfo() { Month = GetMonth("TMoney4Pos"), MoneyPos = TMoney4Pos });
            }

            if (TMoney5Pos != -1)
            {
                tmi.Add(new MoneyInfo() { Month = GetMonth("TMoney5Pos"), MoneyPos = TMoney5Pos });
            }

            var mmi = new List<MoneyInfo>();

            if (MMoney1Pos != -1)
            {
                mmi.Add(new MoneyInfo() { Month = GetMonth("MMoney1Pos"), MoneyPos = MMoney1Pos });
            }

            if (MMoney2Pos != -1)
            {
                mmi.Add(new MoneyInfo() { Month = GetMonth("MMoney2Pos"), MoneyPos = MMoney2Pos });
            }

            if (MMoney3Pos != -1)
            {
                mmi.Add(new MoneyInfo() { Month = GetMonth("MMoney3Pos"), MoneyPos = MMoney3Pos });
            }

            if (MMoney4Pos != -1)
            {
                mmi.Add(new MoneyInfo() { Month = GetMonth("MMoney4Pos"), MoneyPos = MMoney4Pos });
            }

            if (MMoney5Pos != -1)
            {
                mmi.Add(new MoneyInfo() { Month = GetMonth("MMoney5Pos"), MoneyPos = MMoney5Pos });
            }

            var sbmi = new List<MoneyInfo>();

            if (SBMoney1Pos != -1)
            {
                sbmi.Add(new MoneyInfo() { Month = GetMonth("SBMoney1Pos"), MoneyPos = SBMoney1Pos });
            }

            if (SBMoney2Pos != -1)
            {
                sbmi.Add(new MoneyInfo() { Month = GetMonth("SBMoney2Pos"), MoneyPos = SBMoney2Pos });
            }

            if (SBMoney3Pos != -1)
            {
                sbmi.Add(new MoneyInfo() { Month = GetMonth("SBMoney3Pos"), MoneyPos = SBMoney3Pos });
            }

            if (SBMoney4Pos != -1)
            {
                sbmi.Add(new MoneyInfo() { Month = GetMonth("SBMoney4Pos"), MoneyPos = SBMoney4Pos });
            }

            if (SBMoney5Pos != -1)
            {
                sbmi.Add(new MoneyInfo() { Month = GetMonth("SBMoney5Pos"), MoneyPos = SBMoney5Pos });
            }

            var omi = new List<OnceInfo>();

            if (OMoney1Pos != -1)
            {
                omi.Add(new OnceInfo() { Key = OMoney1Pos, Value = oMoney1.Text.Trim() });
            }

            if (OMoney2Pos != -1)
            {
                omi.Add(new OnceInfo() { Key = OMoney2Pos, Value = oMoney2.Text.Trim() });
            }

            if (OMoney3Pos != -1)
            {
                omi.Add(new OnceInfo() { Key = OMoney3Pos, Value = oMoney3.Text.Trim() });
            }

            if (OMoney4Pos != -1)
            {
                omi.Add(new OnceInfo() { Key = OMoney4Pos, Value = oMoney4.Text.Trim() });
            }

            if (OMoney5Pos != -1)
            {
                omi.Add(new OnceInfo() { Key = OMoney5Pos, Value = oMoney5.Text.Trim() });
            }

            if (OMoney6Pos != -1)
            {
                omi.Add(new OnceInfo() { Key = OMoney6Pos, Value = oMoney6.Text.Trim() });
            }

            if (OMoney7Pos != -1)
            {
                omi.Add(new OnceInfo() { Key = OMoney7Pos, Value = oMoney7.Text.Trim() });
            }

            try
            {

                Result = GetFinanceInstance().InsertForShanghai(IsTry2Import, Students, StudentNoPos, YearPos, StartDatePos, PTMoneyPos, PTRateIDPos, TMoneyPos, TRateIDPos, TStartDatePos, tmi, MMoneyPos, MRateIDPos, MStartDatePos, mmi, SBMoneyPos, SBRateIDPos, SBStartDatePos, sbmi, omi, TMonthPos, MMonthPos, SBMonthPos);

                if (Result && !IsTry2Import)
                {
                    Result = GetFinanceInstance().ChargeInsert(Students, StudentNoPos, false);
                }

                if (Result) {
                    Message = "Import Finance Successfully!";
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally {
                IsTry2Import = false;
                btnImport.Enabled = true;
                MessageBox.Show(Message);
            }
            
        }

        private string GetMonth(string name) {
            var m = int.Parse(name.Substring(name.Length - 4).Substring(0,1));
            var ret = string.Empty;
            switch (m) {
                case 1:
                    ret = "2018/09/01";
                    break;
                case 2:
                    ret = "2018/10/01";
                    break;
                case 3:
                    ret = "2018/11/01";
                    break;
                case 4:
                    ret = "2018/12/01";
                    break;
                case 5:
                    ret = "2019/01/01";
                    break;
                default:
                    ret = "2019/02/01";
                    break;
            }
            return ret;
        }



    }
}
