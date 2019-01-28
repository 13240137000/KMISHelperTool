using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KMISHelper.DBHelper;
using KMISHelper.DBScript;
using KMISHelper.HelpGlobal;
using MySql.Data.MySqlClient;

namespace KMISHelper.Business
{
    class BznsUsers
    {
        public readonly string ModuleName = "Users";

        public bool Insert(int NamePos, int EmailPos, int SexPos,int MobilePos, int RolePos,int KindergartenPos, DataTable Users, bool IsTry2Import) {
            var result = false;
            var Sql = string.Empty;
            var CreateDate = InitObject.GetSysDate();

            using (MySqlConnection conn = new MySqlConnection(BznsBase.GetConnectionString))
            {

                MySqlTransaction trans = null;
                conn.Open();
                trans = conn.BeginTransaction();

                try
                {

                    foreach (DataRow dr in Users.Rows)
                    {

                        var UserId = InitObject.GetUUID();

                        var Name = NamePos == -1 ? string.Empty : dr[NamePos].ToString().Trim();
                        var Email = EmailPos == -1 ? string.Empty : dr[EmailPos].ToString().Trim();
                        var Sex = SexPos == -1 ? 0 : dr[SexPos].ToString().Trim() == "男"? 1:0;
                        var Mobile = MobilePos == -1 ? string.Empty : dr[MobilePos].ToString().Trim();
                        var Role = RolePos == -1 ? string.Empty : dr[RolePos].ToString().Trim();
                        var Kindergarten = KindergartenPos == -1 ? string.Empty : dr[KindergartenPos].ToString().Trim();
                        var RoleInfo = new RoleInfo();
                        var KindergartenInfo = new KindergartenInfo();
                        var ret = 0;

                        if (!string.IsNullOrEmpty(Role.Trim())) {
                            RoleInfo = InitObject.GetRoleInfoByTitle(Role.Trim());
                        }

                        if (!string.IsNullOrEmpty(Kindergarten.Trim())) {
                            KindergartenInfo = InitObject.GetKindergartenInfoByName(Kindergarten.Trim());
                        }


                        try
                        {

                            if (!string.IsNullOrEmpty(Email.Trim()) && !string.IsNullOrEmpty(Name.Trim()) && !string.IsNullOrEmpty(RoleInfo.RoleId) && !string.IsNullOrEmpty(KindergartenInfo.Id)) {

                                // Insert into upms user

                                Sql = string.Format(InitObject.GetScriptServiceInstance().UserInsert, UserId, Name, BznsBase.UserPassword, BznsBase.UserPasswordSalt, Name, Mobile, Email, KindergartenInfo.Id, BznsBase.BrandID, Sex);
                                ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                // Insert into upms role

                                var UserRoleId = InitObject.GetUUID();

                                if (ret > 0) {
                                    ret = 0;
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().RoleInsert, UserRoleId, UserId, RoleInfo.RoleId);
                                    ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                }

                                // Insert into user kindergarten

                                if (ret > 0) {
                                    ret = 0;
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().UserKindergartenInsert, UserId, KindergartenInfo.Id);
                                    ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                }

                            }

                            
                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat("Import Users LogID:", dr["LogID"].ToString(), " Error Message:", ex.Message), SysLogType.ERROR, ModuleName));
                        }
                        finally
                        {

                        }

                    }

                    if (IsTry2Import)
                    {
                        trans.Rollback();
                    }
                    else
                    {
                        trans.Commit();
                    }

                    result = true;

                }
                catch (Exception ex)
                {

                    SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, string.Concat(ModuleName)));
                    trans.Rollback();
                }
                finally
                {
                    trans = null;
                }
            }


            return result;
        }

    }
}
