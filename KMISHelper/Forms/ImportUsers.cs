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
    public partial class ImportUsers : Form
    {
        public ImportUsers()
        {
            InitializeComponent();
        }

        private BznsUsers bu;
        private BznsUsers GetUsersInstance()
        {
            if (bu == null)
            {
                bu = new BznsUsers();
            }
            return bu;
        }

        private DataTable Users = null;

        private bool IsTry2Import = false;

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Upload Users";
            of.InitialDirectory = @BznsBase.InitialDirectory;
            of.Filter = "Excel Files(.xls)| *.xls";
            of.FilterIndex = 2;

            of.RestoreDirectory = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadUserField(of.FileName);
                LoadUsers(of.FileName);
            }
        }

        private void LoadUserField(string FileName) {

            cbName.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportUser);
            cbName.DisplayMember = "Value";
            cbName.ValueMember = "Key";

            cbEmail.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportUser);
            cbEmail.DisplayMember = "Value";
            cbEmail.ValueMember = "Key";

            cbSex.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportUser);
            cbSex.DisplayMember = "Value";
            cbSex.ValueMember = "Key";

            cbMobile.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportUser);
            cbMobile.DisplayMember = "Value";
            cbMobile.ValueMember = "Key";

            cbRole.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportUser);
            cbRole.DisplayMember = "Value";
            cbRole.ValueMember = "Key";

            cbKindergarten.DataSource = ExcelHelper.ExcelHelper.GetExcelHeader(FileName, BznsBase.ImportUser);
            cbKindergarten.DisplayMember = "Value";
            cbKindergarten.ValueMember = "Key";

            if (BznsBase.EnableMappingField) {
                cbName.Text = "Name";
                cbEmail.Text = "Email";
                cbSex.Text = "Sex";
                cbMobile.Text = "Mobile";
                cbRole.Text = "Role";
                cbKindergarten.Text = "Kindergarten";
            }

        }

        private void LoadUsers(string FileName) {
            Users = ExcelHelper.ExcelHelper.Import(FileName, BznsBase.ImportUser);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var Result = false;
            var Message = "Oh, import failed please review error log.";
            var NamePos = Convert.ToInt32(cbName.SelectedValue);
            var EmailPos = Convert.ToInt32(cbEmail.SelectedValue);
            var SexPos = Convert.ToInt32(cbSex.SelectedValue);
            var MobilePos = Convert.ToInt32(cbMobile.SelectedValue);
            var RolePos = Convert.ToInt32(cbRole.SelectedValue);
            var KindergartenPos = Convert.ToInt32(cbKindergarten.SelectedValue);

            try
            {

                Result = GetUsersInstance().Insert(NamePos,EmailPos,SexPos,MobilePos,RolePos,KindergartenPos,Users,IsTry2Import);

                if (Result)
                {
                    Message = "Import Users Successfully!";
                }
                else
                {
                    SysLog.Insert(new SysLogInfo("Import Users Fail.", SysLogType.ERROR, "Import Users FRM"));
                }

            }
            catch (Exception ex)
            {
                SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, "Import Users FRM"));
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
    }
}
