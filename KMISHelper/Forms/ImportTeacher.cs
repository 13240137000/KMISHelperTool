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
    public partial class ImportTeacher : Form
    {
        public ImportTeacher()
        {
            InitializeComponent();
        }

        private BznsTeacher ti;
        private BznsTeacher GetTeacherInstance()
        {
            if (ti == null)
            {
                ti = new BznsTeacher();
            }
            return ti;
        }

        private DataTable Teachers = null;

        private bool IsTry2Import = false;




        #region Load Fields and Teachers

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Upload Teacher";
            of.InitialDirectory = @BznsBase.InitialDirectory;
            of.Filter = "Excel Files(.xls)| *.xls";
            of.FilterIndex = 2;

            of.RestoreDirectory = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadTeacherField(of.FileName);
                LoadTeacher(of.FileName);
            }
        }

        private void LoadTeacherField(string FileName)
        {

            cbName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName,BznsBase.ImportTeacherSheetName);
            cbName.DisplayMember = "Value";
            cbName.ValueMember = "Key";

            cbLastName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbLastName.DisplayMember = "Value";
            cbLastName.ValueMember = "Key";

            cbFirstName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbFirstName.DisplayMember = "Value";
            cbFirstName.ValueMember = "Key";

            cbSex.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbSex.DisplayMember = "Value";
            cbSex.ValueMember = "Key";

            cbNationality.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbNationality.DisplayMember = "Value";
            cbNationality.ValueMember = "Key";

            cbNation.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbNation.DisplayMember = "Value";
            cbNation.ValueMember = "Key";

            cbIDCardType.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbIDCardType.DisplayMember = "Value";
            cbIDCardType.ValueMember = "Key";

            cbIDCardNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbIDCardNo.DisplayMember = "Value";
            cbIDCardNo.ValueMember = "Key";

            cbBirthday.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbBirthday.DisplayMember = "Value";
            cbBirthday.ValueMember = "Key";

            cbMobileNo.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbMobileNo.DisplayMember = "Value";
            cbMobileNo.ValueMember = "Key";

            cbEmailAddress.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbEmailAddress.DisplayMember = "Value";
            cbEmailAddress.ValueMember = "Key";

            cbIsPreEdu.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbIsPreEdu.DisplayMember = "Value";
            cbIsPreEdu.ValueMember = "Key";

            cbForeignNationality.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbForeignNationality.DisplayMember = "Value";
            cbForeignNationality.ValueMember = "Key";

            cbTeacherLevel.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbTeacherLevel.DisplayMember = "Value";
            cbTeacherLevel.ValueMember = "Key";

            cbEduQualification.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbEduQualification.DisplayMember = "Value";
            cbEduQualification.ValueMember = "Key";

            cbProfession.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbProfession.DisplayMember = "Value";
            cbProfession.ValueMember = "Key";

            cbUniversity.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbUniversity.DisplayMember = "Value";
            cbUniversity.ValueMember = "Key";

            cbClassId.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbClassId.DisplayMember = "Value";
            cbClassId.ValueMember = "Key";

            cbKindergartenId.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbKindergartenId.DisplayMember = "Value";
            cbKindergartenId.ValueMember = "Key";

            cbAccount.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportTeacherSheetName);
            cbAccount.DisplayMember = "Value";
            cbAccount.ValueMember = "Key";

            if (BznsBase.EnableMappingField) {
                cbName.Text = "Name";
                cbLastName.Text = "Last Name";
                cbFirstName.Text = "First Name";
                cbSex.Text = "Sex";
                cbNationality.Text = "Nationality";
                cbNation.Text = "Nation";
                cbIDCardType.Text = "ID Card Type";
                cbIDCardNo.Text = "ID Card No";
                cbClassId.Text = "ClassID";
                cbAccount.Text = "Account";
            }


        }

        private void LoadTeacher(string FileName)
        {
            Teachers = ExcelHelper.ExcelHelper.Import(FileName, BznsBase.ImportTeacherSheetName);
        }

        #endregion

        #region preimport

        private void btnPreImport_Click(object sender, EventArgs e)
        {
            IsTry2Import = true;
            btnImport_Click(null, null);
        }

        #endregion

        #region import

        private void btnImport_Click(object sender, EventArgs e)
        {

            var Result = false;
            var Message = "Oh, import failed please review error log.";
            var NamePos = Convert.ToInt32(cbName.SelectedValue);
            var LastNamePos = Convert.ToInt32(cbLastName.SelectedValue);
            var FirstNamePos = Convert.ToInt32(cbFirstName.SelectedValue);
            var SexPos = Convert.ToInt32(cbSex.SelectedValue);
            var NationalityPos = Convert.ToInt32(cbNationality.SelectedValue);
            var NationPos = Convert.ToInt32(cbNation.SelectedValue);
            var IDCardTypePos = Convert.ToInt32(cbIDCardType.SelectedValue);
            var IDCardNoPos = Convert.ToInt32(cbIDCardNo.SelectedValue);
            var BirthdayPos = Convert.ToInt32(cbBirthday.SelectedValue);
            var MobileNoPos = Convert.ToInt32(cbMobileNo.SelectedValue);
            var EmailAddressPos = Convert.ToInt32(cbEmailAddress.SelectedValue);
            var IsPreEduPos = Convert.ToInt32(cbIsPreEdu.SelectedValue);
            var ForeignNationalityPos = Convert.ToInt32(cbForeignNationality.SelectedValue);
            var TeacherLevelPos = Convert.ToInt32(cbTeacherLevel.SelectedValue);
            var EduQualificationPos = Convert.ToInt32(cbEduQualification.SelectedValue);
            var ProfessionPos = Convert.ToInt32(cbProfession.SelectedValue);
            var UniversityPos = Convert.ToInt32(cbUniversity.SelectedValue);
            var ClassIdPos = Convert.ToInt32(cbClassId.SelectedValue);
            var KindergartenIdPos = Convert.ToInt32(cbKindergartenId.SelectedValue);
            var AccountPos = Convert.ToInt32(cbAccount.SelectedValue);

            try
            {

                Result = GetTeacherInstance().InsertTeacher(IsTry2Import,Teachers,NamePos,LastNamePos,FirstNamePos,SexPos,NationalityPos, NationPos, IDCardTypePos, IDCardNoPos, BirthdayPos, MobileNoPos, EmailAddressPos, IsPreEduPos, ForeignNationalityPos, TeacherLevelPos, EduQualificationPos, ProfessionPos, UniversityPos,ClassIdPos,KindergartenIdPos,AccountPos);

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
                MessageBox.Show(Message);
            }

        }


        #endregion


    }
}
