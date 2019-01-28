using System;
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
    public partial class ImportClass : Form
    {
        public ImportClass()
        {
            InitializeComponent();
        }

        private BznsClass ci;
        private BznsClass GetClassInstance() {
            if (ci == null) {
                ci = new BznsClass();
            }
            return ci;
        }

        private DataTable Classes = null;

        private DataTable Plans = null;

        #region Import Plan

        private bool IsTry2Import = false;
        private void btnUpload_Click(object sender, EventArgs e)
        {

            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Upload Class";
            of.InitialDirectory = @BznsBase.InitialDirectory;
            of.Filter = "Excel Files(.xls)| *.xls";
            of.FilterIndex = 2;

            of.RestoreDirectory = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadPlanField(of.FileName);
                LoadClassPlan(of.FileName);
            }

        }

        private void LoadPlanField(string FileName)
        {
            try
            {

                var PlanTitleDatasrouce = ExcelHelper.ExcelHelper.GetExcelHeader(FileName,BznsBase.ImportClassPlanSheetName);
                var PlanDescriptionDatasrouce = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportClassPlanSheetName);

                // Load Acac Year
                LoadAcadYear();

                // Load Plan Title
                cbPlanTitle.DataSource = PlanTitleDatasrouce;
                cbPlanTitle.DisplayMember = "Value";
                cbPlanTitle.ValueMember = "Key";

                //Load Plan Desc
                cbPlanDescription.DataSource = PlanDescriptionDatasrouce;
                cbPlanDescription.DisplayMember = "Value";
                cbPlanDescription.ValueMember = "Key";

                if (BznsBase.EnableMappingField)
                {
                    cbPlanTitle.Text = "Plan Name";
                    cbPlanDescription.Text = "Description";
                }


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }

        }

        private void LoadClassPlan(string FileName)
        {
            Plans = ExcelHelper.ExcelHelper.Import(FileName,BznsBase.ImportClassPlanSheetName);
        }

        private void LoadAcadYear()
        {
            cbYear.DataSource = InitObject.GetAcadYear();
            cbYear.DisplayMember = "acad_title";
            cbYear.ValueMember = "acad_year_id";

            if (BznsBase.EnableMappingField) {
                cbYear.Text = "2018-2019学年";
            }
        }

        private void ImportClass_Load(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {

            var AcadYear = cbYear.SelectedValue.ToString();
            var PlanTitlePos = int.Parse(cbPlanTitle.SelectedValue.ToString());
            var PlanDescriptionPos = int.Parse(cbPlanDescription.SelectedValue.ToString());
            var Result = false;
            var Message = "Oh, import failed please review error log.";

            try
            {
                Result = GetClassInstance().InsertPlan(AcadYear, PlanTitlePos, PlanDescriptionPos, Plans, IsTry2Import);

                if (Result)
                {
                    Message = "Import Class Plan Successfully!";
                }
                else
                {
                    SysLog.Insert(new SysLogInfo("Import Plan Fail.", SysLogType.ERROR, "Import Class FRM"));
                }
            }
            catch (Exception ex)
            {
                SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, "Import Class FRM"));
            }
            finally
            {
                IsTry2Import = false;
                MessageBox.Show(Message);
            }

        }

        private void btnPreImport_Click(object sender, EventArgs e)
        {
            IsTry2Import = true;
            btnImport_Click(null, null);
        }


        #endregion

        #region Import Class

        private void btnClass_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Upload Class";
            of.InitialDirectory = @BznsBase.InitialDirectory;
            of.Filter = "Excel Files(.xls)| *.xls";
            of.FilterIndex = 2;

            of.RestoreDirectory = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadAcadYearForImportClass();
                LoadPlan();
                LoadClassField(of.FileName);
                LoadClass(of.FileName);
            }
        }

        private void LoadAcadYearForImportClass() {
            cbcYear.DataSource = InitObject.GetAcadYear();
            cbcYear.DisplayMember = "acad_title";
            cbcYear.ValueMember = "acad_year_id";

            if (BznsBase.EnableMappingField) {
                cbcYear.Text = "2018-2019学年";
            }

        }

        private void LoadPlan()
        {
            cbcPlanName.DataSource = GetClassInstance().GetClassPlanList();
            cbcPlanName.DisplayMember = "class_plan_title";
            cbcPlanName.ValueMember = "class_plan_id";
        }

        private void LoadClassField(string FileName) {
            try
            {

                cbcSchoolCal.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName,BznsBase.ImportClassSheetName); ;
                cbcSchoolCal.DisplayMember = "Value";
                cbcSchoolCal.ValueMember = "Key";

                cbcClassType.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportClassSheetName); ;
                cbcClassType.DisplayMember = "Value";
                cbcClassType.ValueMember = "Key";

                cbcClassName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportClassSheetName); ;
                cbcClassName.DisplayMember = "Value";
                cbcClassName.ValueMember = "Key";

                cbcCapacity.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportClassSheetName); ;
                cbcCapacity.DisplayMember = "Value";
                cbcCapacity.ValueMember = "Key";

                cbcTeachStartTime.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportClassSheetName); ;
                cbcTeachStartTime.DisplayMember = "Value";
                cbcTeachStartTime.ValueMember = "Key";

                cbcTeachEndTime.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportClassSheetName); ;
                cbcTeachEndTime.DisplayMember = "Value";
                cbcTeachEndTime.ValueMember = "Key";

                cbcClassAlias.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportClassSheetName); ;
                cbcClassAlias.DisplayMember = "Value";
                cbcClassAlias.ValueMember = "Key";

                cbcGradeType.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportClassSheetName); ;
                cbcGradeType.DisplayMember = "Value";
                cbcGradeType.ValueMember = "Key";

                cbcOverbooked.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportClassSheetName); ;
                cbcOverbooked.DisplayMember = "Value";
                cbcOverbooked.ValueMember = "Key";

                if (BznsBase.EnableMappingField) {
                    cbcSchoolCal.Text = "School Cal";
                    cbcClassType.Text = "Class Type";
                    cbcClassName.Text = "Class_nm";
                    cbcCapacity.Text = "Capacity";
                    cbcTeachStartTime.Text = "Start Time";
                    cbcTeachEndTime.Text = "End Time";
                    cbcClassAlias.Text = "Class Alias";
                    cbcGradeType.Text = "Grade Type";
                    cbcOverbooked.Text = "Allow Overbooked(N/Y)";
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }

        private void LoadClass(string FileName)
        {
            Classes = ExcelHelper.ExcelHelper.Import(FileName, BznsBase.ImportClassSheetName);
        }

        private void btnImportClass_Click(object sender, EventArgs e)
        {

            var AcadYear = cbcYear.SelectedValue.ToString();
            var PlanID = cbcPlanName.SelectedValue.ToString();
            var SchoolColPos = int.Parse(cbcSchoolCal.SelectedValue.ToString());
            var ClassTypePos = int.Parse(cbcClassType.SelectedValue.ToString());
            var ClassNamePos = int.Parse(cbcClassName.SelectedValue.ToString());
            var CapacityPos = int.Parse(cbcCapacity.SelectedValue.ToString());
            var StartTimePos = int.Parse(cbcTeachStartTime.SelectedValue.ToString());
            var EndTimePos = int.Parse(cbcTeachEndTime.SelectedValue.ToString());
            var ClassAliasPos = int.Parse(cbcClassAlias.SelectedValue.ToString());
            var GradeTypePos = int.Parse(cbcGradeType.SelectedValue.ToString());
            var AllowOverbookedPos = int.Parse(cbcOverbooked.SelectedValue.ToString());
            var Result = false;
            var Message = "Oh, import class failed please review error log.";

            try
            {

                Result = GetClassInstance().InsertClass(AcadYear,PlanID,SchoolColPos,ClassTypePos,ClassNamePos,CapacityPos,StartTimePos,EndTimePos,ClassAliasPos,GradeTypePos,AllowOverbookedPos,Classes,IsTry2Import);

                if (Result)
                {
                    Message = "Import Class Successfully!";
                }
                else
                {
                    SysLog.Insert(new SysLogInfo("Import Class Fail.", SysLogType.ERROR, "Import Class FRM"));
                }
            }
            catch (Exception ex)
            {
                SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, "Import Class FRM"));
            }
            finally
            {
                IsTry2Import = false;
                MessageBox.Show(Message);
            }

        }


        #endregion

        private void btnPreClass_Click(object sender, EventArgs e)
        {
            IsTry2Import = true;
            btnImportClass_Click(null, null);
        }
    }
}
