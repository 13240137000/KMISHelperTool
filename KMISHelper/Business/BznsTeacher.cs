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
    class BznsTeacher
    {

        public readonly string ModuleName = "Teacher";

        public bool InsertTeacher(bool IsTry2Import, DataTable Teachers, int NamePos, int LastNamePos, int FirstNamePos, int SexPos, int NationalityPos, int NationPos, int IDCardTypePos, int IDCardNoPos, int BirthdayPos, int MobileNoPos, int EmailAddressPos, int IsPreEduPos, int ForeignNationalityPos, int TeacherLevelPos, int EduQualificationPos, int ProfessionPos, int UniversityPos,int ClassIdPos,int KindergartenIdPos,int AccountPos) {

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

                    foreach (DataRow dr in Teachers.Rows)
                    {

                        var TeacherID = InitObject.GetUUID();

                        var TeacherName = NamePos == -1 ? string.Empty : dr[NamePos].ToString().Trim();
                        var LastName = LastNamePos == -1 ? string.Empty : dr[LastNamePos].ToString().Trim();
                        var FirstName = FirstNamePos == -1 ? string.Empty : dr[FirstNamePos].ToString().Trim();
                        var Sex = SexPos == -1 ? string.Empty : InitObject.GetSexByName(dr[SexPos].ToString().Trim());
                        var Nationality = NationalityPos == -1 ? string.Empty : dr[NationalityPos].ToString().Trim();
                        var Nation = NationPos == -1 ? string.Empty : InitObject.GetNationCodeByName(dr[NationPos].ToString().Trim());
                        var IDCardType = IDCardTypePos == -1 ? string.Empty : InitObject.GetIdType(dr[IDCardTypePos].ToString().Trim());
                        var IDCardNo = IDCardNoPos == -1 ? string.Empty : dr[IDCardNoPos].ToString().Trim();
                        var Birthday = BirthdayPos == -1 ? string.Empty : dr[BirthdayPos].ToString().Trim();
                        var MobileNo = MobileNoPos == -1 ? string.Empty : dr[MobileNoPos].ToString().Trim();
                        var EmailAddress = EmailAddressPos == -1 ? string.Empty : dr[EmailAddressPos].ToString().Trim();
                        var IsPreEdu = IsPreEduPos == -1 ? string.Empty : dr[IsPreEduPos].ToString().Trim();
                        var ForeignNationality = ForeignNationalityPos == -1 ? string.Empty : dr[ForeignNationalityPos].ToString().Trim();
                        var TeacherLevel = TeacherLevelPos == -1 ? string.Empty : InitObject.GetTeacherLevel(dr[TeacherLevelPos].ToString().Trim());
                        var EduQualification = EduQualificationPos == -1 ? string.Empty : InitObject.GetEducationCodeByName(dr[EduQualificationPos].ToString().Trim());
                        var Profession = ProfessionPos == -1 ? string.Empty : dr[ProfessionPos].ToString().Trim();
                        var University = UniversityPos == -1 ? string.Empty : dr[UniversityPos].ToString().Trim();
                        var iSex = 0;
                        var ClassId = ClassIdPos == -1 ? string.Empty : dr[ClassIdPos].ToString().Trim();
                        var KindergartenId = KindergartenIdPos == -1 ? string.Empty : dr[KindergartenIdPos].ToString().Trim();
                        var Account = AccountPos == -1 ? string.Empty : dr[AccountPos].ToString().Trim();


                        try
                        {

                            if (string.IsNullOrEmpty(KindergartenId)) {
                                KindergartenId = BznsBase.KindergartenId;
                            }

                            Sql = string.Format(InitObject.GetScriptServiceInstance().TeacherInsert, TeacherID, KindergartenId, TeacherName,LastName,FirstName,Sex,Nationality,Nation,IDCardType,IDCardNo,Birthday,Account,EmailAddress,IsPreEdu,ForeignNationality,TeacherLevel,EduQualification,Profession,University,BznsBase.UserID, CreateDate);
                            var ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                            if (!string.IsNullOrEmpty(ClassId) && ret > 0) {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().TeacherClassInsert, ClassId,TeacherID, "STAFFTYPE02",'N');
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }

                            if (ret > 0)
                            {

                                if (Sex == "M") {
                                    iSex = 1;
                                }

                                Sql = string.Format(InitObject.GetScriptServiceInstance().UserInsert, TeacherID, Account, BznsBase.UserPassword, BznsBase.UserPasswordSalt, TeacherName, MobileNo, EmailAddress, KindergartenId, BznsBase.BrandID,iSex);

                                if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) > 0){
                                    
                                    // 8daa29e88498480aad68683c361a451e 保育员
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().RoleInsert, InitObject.GetUUID(),TeacherID, "ed40b872ec1a48dea57819e58fbff1a7");
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().UserKindergartenInsert, TeacherID, KindergartenId);
                                    ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat("Import Teacher LogID:", dr["LogID"].ToString(), " Error Message:", ex.Message), SysLogType.ERROR, ModuleName));
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

                    SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, string.Concat(ModuleName, " - ", "Import Teacher Bzns")));
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
