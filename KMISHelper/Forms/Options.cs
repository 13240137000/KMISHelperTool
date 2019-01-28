using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void LoadBrand()
        {
            var Sql = string.Empty;
            var Brand = new DataTable();
            try
            {
                Sql = InitObject.GetScriptServiceInstance().GetBrandList;
                Brand = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                cbBrand.DataSource = Brand;
                cbBrand.DisplayMember = "brand_name";
                cbBrand.ValueMember = "brand_id";
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Brand = null;
            }
        }

        private void LoadKindergarten()
        {
            var Sql = string.Empty;
            var Kindergarten = new DataTable();
            try
            {
                Sql = InitObject.GetScriptServiceInstance().GetKindergartenList;
                Kindergarten = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                cbKindergarten.DataSource = Kindergarten;
                cbKindergarten.DisplayMember = "kindergarten_nm";
                cbKindergarten.ValueMember = "kindergarten_id";
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Kindergarten = null;
            }
        }

        private void LoadBrandUser()
        {
            var Sql = string.Empty;
            var BrandUser = new DataTable();
            try
            {
                Sql = InitObject.GetScriptServiceInstance().GetBrandUserList;
                BrandUser = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                cbBrandUser.DataSource = BrandUser;
                cbBrandUser.DisplayMember = "realname";
                cbBrandUser.ValueMember = "user_id";
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                BrandUser = null;
            }
        }

        private void LoadKindergartenUser()
        {
            var Sql = string.Empty;
            var KindergartenUser = new DataTable();
            try
            {
                Sql = InitObject.GetScriptServiceInstance().GetKindergartenUserList;
                KindergartenUser = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                cbKindergartenUser.DataSource = KindergartenUser;
                cbKindergartenUser.DisplayMember = "realname";
                cbKindergartenUser.ValueMember = "user_id";
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                KindergartenUser = null;
            }
        }

        private void Options_Load(object sender, EventArgs e)
        {
            LoadBrand();
            LoadKindergarten();
            LoadBrandUser();
            LoadKindergartenUser();
        }

        private void btnGeneralSave_Click(object sender, EventArgs e)
        {
            var BrandId = string.Empty;
            var KindergartenId = string.Empty;
            var UserID = string.Empty;
            var BrandUserID = string.Empty;

            try
            {
                BrandId = cbBrand.SelectedValue.ToString();
                KindergartenId = cbKindergarten.SelectedValue.ToString();
                BrandUserID = cbBrandUser.SelectedValue.ToString();
                UserID = cbKindergartenUser.SelectedValue.ToString();

                InitObject.SetConfigValue("BrandId", BrandId);
                InitObject.SetConfigValue("KindergartenId", KindergartenId);
                InitObject.SetConfigValue("BrandUserID", BrandUserID);
                InitObject.SetConfigValue("UserID", UserID);

                MessageBox.Show("Congratulations, your change successfully!");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }
        }

        private void btnExportSave_Click(object sender, EventArgs e)
        {

            try
            {

                if (!string.IsNullOrEmpty(txtClassPath.Text.Trim()))
                {
                    InitObject.SetConfigValue("ExportClassFilePath", txtClassPath.Text.Trim());
                }

                if (!string.IsNullOrEmpty(txtErrorLog.Text.Trim()))
                {
                    InitObject.SetConfigValue("ErrorLogPath", txtErrorLog.Text.Trim());
                }

                if (!string.IsNullOrEmpty(txtInfoLog.Text.Trim()))
                {
                    InitObject.SetConfigValue("InfoLogPath", txtInfoLog.Text.Trim());
                }

                if (!string.IsNullOrEmpty(txtWarningLog.Text.Trim()))
                {
                    InitObject.SetConfigValue("WarningLogPath", txtWarningLog.Text.Trim());
                }

                MessageBox.Show("Congratulations, your change successfully!");

            }
            catch (Exception)
            {

                throw;
            }

            

        }
    }

}
