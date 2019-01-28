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
    public partial class ImportStudent : Form
    {
        public ImportStudent()
        {
            InitializeComponent();
        }

        private BznsStudent si;
        private BznsStudent GetStudentInstance()
        {
            if (si == null)
            {
                si = new BznsStudent();
            }
            return si;
        }

        private DataTable Students = null;

        private bool IsTry2Import = false;


        private void btnPreImport_Click(object sender, EventArgs e)
        {
            IsTry2Import = true;
            btnImport_Click(null, null);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

            btnImport.Enabled = false;
            var Message = "Oh, import class failed please review error log.";
            var Result = false;

            var AcadYear = cbYear.SelectedValue.ToString();
            var StudentNamePos = Convert.ToInt32(cbStudentName.SelectedValue);
            var CardTypePos = Convert.ToInt32(cbCardType.SelectedValue);
            var IdCardPos = Convert.ToInt32(cbIDCard.SelectedValue);
            var LastNamePos = Convert.ToInt32(cbLastName.SelectedValue);
            var FirstNamePos = Convert.ToInt32(cbFirstName.SelectedValue);
            var NationalityPos = Convert.ToInt32(cbNationality.SelectedValue);
            var SexPos = Convert.ToInt32(cbSex.SelectedValue);
            var BirthDayPos = Convert.ToInt32(cbBirthDay.SelectedValue);
            var StudentNoPos = Convert.ToInt32(cbStudentNo.SelectedValue);
            var StatusPos = Convert.ToInt32(cbStatus.SelectedValue);
            var PlanTimePos = Convert.ToInt32(cbPlanTime.SelectedValue);
            var NationPos = Convert.ToInt32(cbNation.SelectedValue);

            var GuardianNamePos = Convert.ToInt32(cbGuardianName.SelectedValue);
            var GuardianMobileNoPos = Convert.ToInt32(cbMobileNo.SelectedValue);
            var GuardianRelationshipPos = Convert.ToInt32(cbRelationship.SelectedValue);
            var GuardianEducationPos = Convert.ToInt32(cbEducation.SelectedValue);
            var GuardianJobPos = Convert.ToInt32(cbJob.SelectedValue);
            var GuardianCompanyPos = Convert.ToInt32(cbCompany.SelectedValue);
            var ParentNamePos = Convert.ToInt32(cbcParnetName.SelectedValue);
            var MobileNoPos = Convert.ToInt32(cbcMobileNo.SelectedValue);
            var RelationshipPos = Convert.ToInt32(cbcRelationship.SelectedValue);
            var EducationPos = Convert.ToInt32(cbcEducation.SelectedValue);
            var JobPos = Convert.ToInt32(cbcJob.SelectedValue);
            var CompanyPos = Convert.ToInt32(cbcCompany.SelectedValue);

            var ClassNamePos = Convert.ToInt32(cbClassName.SelectedValue);

            var ParentName3Pos = Convert.ToInt32(cb3ParentName.SelectedValue);
            var MobileNo3Pos = Convert.ToInt32(cb3MobileNo.SelectedValue);
            var Relationship3Pos = Convert.ToInt32(cb3Relationship.SelectedValue);
            var Education3Pos = Convert.ToInt32(cb3Education.SelectedValue);
            var Job3Pos = Convert.ToInt32(cb3Job.SelectedValue);
            var Company3Pos = Convert.ToInt32(cb3Company.SelectedValue);

            try
            {
                Result = GetStudentInstance().Insert(Students, AcadYear, StudentNamePos, CardTypePos, IdCardPos, LastNamePos, FirstNamePos, NationalityPos, SexPos, BirthDayPos, StudentNoPos, StatusPos, PlanTimePos, NationPos, GuardianNamePos, GuardianMobileNoPos, GuardianRelationshipPos, GuardianEducationPos, GuardianJobPos, GuardianCompanyPos, ParentNamePos, MobileNoPos, RelationshipPos, EducationPos, JobPos, CompanyPos, ClassNamePos, ParentName3Pos,MobileNo3Pos,Relationship3Pos,Education3Pos,Job3Pos,Company3Pos, IsTry2Import);
                Message = "Import Student Successfully!!!";
            }
            catch (Exception ex)
            {

                SysLog.Insert(new SysLogInfo(ex.Message, SysLogType.ERROR, BznsStudent.ModuleName + " FRM"));
            }
            finally {
                IsTry2Import = false;
                MessageBox.Show(Message);
                btnImport.Enabled = true;
            }

        }

        private void ImportUser_Load(object sender, EventArgs e)
        {

        }


        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Upload Student";
            of.InitialDirectory = @BznsBase.InitialDirectory;
            of.Filter = "Excel Files(.xls)| *.xls";
            of.FilterIndex = 2;

            of.RestoreDirectory = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadAcadYear();
                LoadStudentField(of.FileName);
                LoadStudents(of.FileName);
            }
        }

        private void LoadStudents(string FileName) {
            Students = ExcelHelper.ExcelHelper.Import(FileName, BznsBase.ImportStudentSheetName);
        }

        private void LoadAcadYear()
        {
            cbYear.DataSource = InitObject.GetAcadYear();
            cbYear.DisplayMember = "acad_title";
            cbYear.ValueMember = "acad_year_id";

            if (BznsBase.EnableMappingField) cbYear.Text = "2018-2019学年";
        }

        private void LoadStudentField(string FileName) {

            //Load student field
            cbStudentName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName,BznsBase.ImportStudentSheetName); 
            cbStudentName.DisplayMember = "Value";
            cbStudentName.ValueMember = "Key";

            cbCardType.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbCardType.DisplayMember = "Value";
            cbCardType.ValueMember = "Key";

            cbIDCard.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbIDCard.DisplayMember = "Value";
            cbIDCard.ValueMember = "Key";

            cbLastName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbLastName.DisplayMember = "Value";
            cbLastName.ValueMember = "Key";

            cbFirstName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbFirstName.DisplayMember = "Value";
            cbFirstName.ValueMember = "Key";

            cbNationality.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbNationality.DisplayMember = "Value";
            cbNationality.ValueMember = "Key";

            cbSex.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbSex.DisplayMember = "Value";
            cbSex.ValueMember = "Key";

            cbBirthDay.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbBirthDay.DisplayMember = "Value";
            cbBirthDay.ValueMember = "Key";

            cbStudentNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbStudentNo.DisplayMember = "Value";
            cbStudentNo.ValueMember = "Key";

            cbStatus.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbStatus.DisplayMember = "Value";
            cbStatus.ValueMember = "Key";

            cbNation.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbNation.DisplayMember = "Value";
            cbNation.ValueMember = "Key";

            cbPlanTime.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbPlanTime.DisplayMember = "Value";
            cbPlanTime.ValueMember = "Key";

            //Load parent field
            cbGuardianName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbGuardianName.DisplayMember = "Value";
            cbGuardianName.ValueMember = "Key";

            cbMobileNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbMobileNo.DisplayMember = "Value";
            cbMobileNo.ValueMember = "Key";

            cbRelationship.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbRelationship.DisplayMember = "Value";
            cbRelationship.ValueMember = "Key";

            cbEducation.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbEducation.DisplayMember = "Value";
            cbEducation.ValueMember = "Key";

            cbJob.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbJob.DisplayMember = "Value";
            cbJob.ValueMember = "Key";

            cbCompany.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName); 
            cbCompany.DisplayMember = "Value";
            cbCompany.ValueMember = "Key";


            cbcParnetName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbcParnetName.DisplayMember = "Value";
            cbcParnetName.ValueMember = "Key";

            cbcMobileNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbcMobileNo.DisplayMember = "Value";
            cbcMobileNo.ValueMember = "Key";

            cbcRelationship.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbcRelationship.DisplayMember = "Value";
            cbcRelationship.ValueMember = "Key";

            cbcEducation.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbcEducation.DisplayMember = "Value";
            cbcEducation.ValueMember = "Key";

            cbcJob.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbcJob.DisplayMember = "Value";
            cbcJob.ValueMember = "Key";

            cbcCompany.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbcCompany.DisplayMember = "Value";
            cbcCompany.ValueMember = "Key";

            // Load class field

            cbClassName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cbClassName.DisplayMember = "Value";
            cbClassName.ValueMember = "Key";

            // Load Parent 3

            cb3ParentName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cb3ParentName.DisplayMember = "Value";
            cb3ParentName.ValueMember = "Key";

            cb3MobileNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cb3MobileNo.DisplayMember = "Value";
            cb3MobileNo.ValueMember = "Key";

            cb3Relationship.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cb3Relationship.DisplayMember = "Value";
            cb3Relationship.ValueMember = "Key";

            cb3Education.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cb3Education.DisplayMember = "Value";
            cb3Education.ValueMember = "Key";

            cb3Job.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cb3Job.DisplayMember = "Value";
            cb3Job.ValueMember = "Key";

            cb3Company.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportStudentSheetName);
            cb3Company.DisplayMember = "Value";
            cb3Company.ValueMember = "Key";

            if (BznsBase.EnableMappingField) {
                cbStudentName.Text = "姓名";
                cbSex.Text = "性别";
                cbStudentNo.Text = "学号";
                cbBirthDay.Text = "出生日期";
                cbPlanTime.Text = "入校时间";
                cbGuardianName.Text = "监护人姓名1";
                cbMobileNo.Text = "监护人电话1";
                cbRelationship.Text = "监护人关系1";
                cbCompany.Text = "监护人单位1";
                cbcParnetName.Text = "监护人姓名2";
                cbcMobileNo.Text = "监护人电话2";
                cbcRelationship.Text = "监护人关系2";
                cbcCompany.Text = "监护人单位2";
                cbClassName.Text = "班级";
            }

        }

        private void btnUpdateRelationship_Click(object sender, EventArgs e)
        {
            GetStudentInstance().UpdateRelationship(Students);
        }

    }
}
