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
    class BznsStudent
    {
        public static string ModuleName = "Student";

        public bool Insert(DataTable Students, string AcadYear, int StudentNamePos, int CardTypePos, int IdCardPos, int LastNamePos, int FirstNamePos, int NationalityPos, int SexPos, int BirthDayPos, int StudentNoPos, int StatusPos, int PlanTimePos, int NationPos, int GuardianNamePos, int GuardianMobileNoPos, int GuardianRelationshipPos, int GuardianEducationPos, int GuardianJobPos, int GuardianCompanyPos, int ParentNamePos, int MobileNoPos, int RelationshipPos, int EducationPos, int JobPos, int CompanyPos, int ClassNamePos, int ParentName3Pos, int MobileNo3Pos, int Relationship3Pos, int Education3Pos, int Job3Pos, int Company3Pos, bool IsTry2Import)
        {

            var Result = false;
            var KindergartenId = BznsBase.KindergartenId;
            var UserID = BznsBase.UserID;
            var Sql = string.Empty;
            var CreateDate = InitObject.GetSysDate();

            using (MySqlConnection conn = new MySqlConnection(BznsBase.GetConnectionString))
            {

                MySqlTransaction trans = null;
                conn.Open();
                trans = conn.BeginTransaction();

                try
                {

                    foreach (DataRow dr in Students.Rows)
                    {

                        var StudentID = InitObject.GetUUID();
                        var StudentName = StudentNamePos == -1 ? string.Empty : dr[StudentNamePos].ToString();
                        var CardType = CardTypePos == -1 ? string.Empty : InitObject.GetIdType(dr[CardTypePos].ToString());
                        var IdCard = IdCardPos == -1 ? string.Empty : dr[IdCardPos].ToString();
                        var LastName = LastNamePos == -1 ? string.Empty : dr[LastNamePos].ToString();
                        var FirstName = FirstNamePos == -1 ? string.Empty : dr[FirstNamePos].ToString();
                        var Nationality = NationalityPos == -1 ? string.Empty : dr[NationalityPos].ToString();
                        var Sex = SexPos == -1 ? string.Empty : InitObject.GetSexByName(dr[SexPos].ToString());
                        var BirthDay = BirthDayPos == -1 ? string.Empty : dr[BirthDayPos].ToString().Replace("/", "-");
                        var StudentNo = StudentNoPos == -1 ? string.Empty : dr[StudentNoPos].ToString();
                        var Status = StatusPos == -1 ? string.Empty : InitObject.GetStatusByName(dr[StatusPos].ToString());
                        var PlanTime = PlanTimePos == -1 ? string.Empty : Convert.ToDateTime(dr[PlanTimePos]).ToString("yyyy-MM-dd");
                        var Nation = NationPos == -1 ? string.Empty : InitObject.GetNationCodeByName(dr[NationPos].ToString());
                        var ClassName = ClassNamePos == -1 ? string.Empty : dr[ClassNamePos].ToString();

                        if (string.IsNullOrWhiteSpace(ClassName))
                        {
                            Status = "STUSTATUS01";
                        }
                        else
                        {
                            Status = "STUSTATUS09";
                        }

                        // insert student info

                        try
                        {

                            if (!string.IsNullOrEmpty(StudentName.Trim()))
                            {

                                Sql = string.Format(InitObject.GetScriptServiceInstance().StudentInsert, AcadYear, KindergartenId, StudentID, StudentName, CardType, IdCard, LastName, FirstName, PlanTime, Nationality, Nation, Sex, BirthDay, CreateDate, Status, StudentNo, UserID);
                                var ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                // create one account for student

                                if (ret > 0)
                                {
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceInsert, InitObject.GetUUID(), StudentID, UserID, CreateDate);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                }

                            }


                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat("Import Student LogID:", dr["LogID"].ToString(), " Error Message:", ex.Message), SysLogType.ERROR, ModuleName));
                        }
                        finally
                        {

                        }

                        // insert parent info

                        var ParentID = InitObject.GetUUID();
                        var GuardianName = GuardianNamePos == -1 ? string.Empty : dr[GuardianNamePos].ToString();
                        var GuardianMobileNo = GuardianMobileNoPos == -1 ? string.Empty : dr[GuardianMobileNoPos].ToString();
                        var GuardianRelationship = GuardianRelationshipPos == -1 ? string.Empty : InitObject.GetRelationshipCodeByName(dr[GuardianRelationshipPos].ToString());
                        var GuardianEducation = GuardianEducationPos == -1 ? string.Empty : dr[GuardianEducationPos].ToString();
                        var GuardianJob = GuardianJobPos == -1 ? string.Empty : dr[GuardianJobPos].ToString();
                        var GuardianCompany = GuardianCompanyPos == -1 ? string.Empty : dr[GuardianCompanyPos].ToString();

                        try
                        {
                            if (!string.IsNullOrEmpty(GuardianName))
                            {

                                Sql = string.Format(InitObject.GetScriptServiceInstance().ParentInsert, ParentID, StudentID, GuardianName, GuardianMobileNo, GuardianRelationship, "Y", UserID, GuardianEducation, GuardianJob, GuardianCompany);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                Sql = string.Format(InitObject.GetScriptServiceInstance().ParentRefInsert, StudentID, ParentID, "Y");
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                Sql = string.Format(InitObject.GetScriptServiceInstance().PrivacyInsert, StudentID, ParentID, "Y");
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                            }
                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat("Import Student Guardion LogID:", dr["LogID"].ToString(), " Error Message:", ex.Message), SysLogType.ERROR, ModuleName));
                        }
                        finally
                        {

                        }

                        ParentID = InitObject.GetUUID();
                        var ParentName = ParentNamePos == -1 ? string.Empty : dr[ParentNamePos].ToString();
                        var MobileNo = MobileNoPos == -1 ? string.Empty : dr[MobileNoPos].ToString();
                        var Relationship = RelationshipPos == -1 ? string.Empty : InitObject.GetRelationshipCodeByName(dr[RelationshipPos].ToString());
                        var Education = EducationPos == -1 ? string.Empty : dr[EducationPos].ToString();
                        var Job = JobPos == -1 ? string.Empty : dr[JobPos].ToString();
                        var Company = CompanyPos == -1 ? string.Empty : dr[CompanyPos].ToString();

                        try
                        {
                            if (!string.IsNullOrEmpty(ParentName))
                            {

                                Sql = string.Format(InitObject.GetScriptServiceInstance().ParentInsert, ParentID, StudentID, ParentName, MobileNo, Relationship, "N", UserID, Education, Job, Company);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                Sql = string.Format(InitObject.GetScriptServiceInstance().ParentRefInsert, StudentID, ParentID, "N");
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                Sql = string.Format(InitObject.GetScriptServiceInstance().PrivacyInsert, StudentID, ParentID, "N");
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                            }
                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat("Import Student Parent LogID:", dr["LogID"].ToString(), " Error Message:", ex.Message), SysLogType.ERROR, ModuleName));
                        }
                        finally
                        {

                        }

                        // parent 3

                        ParentID = InitObject.GetUUID();
                        var ParentName3 = ParentName3Pos == -1 ? string.Empty : dr[ParentName3Pos].ToString();
                        var MobileNo3 = MobileNo3Pos == -1 ? string.Empty : dr[MobileNo3Pos].ToString();
                        var Relationship3 = Relationship3Pos == -1 ? string.Empty : InitObject.GetRelationshipCodeByName(dr[Relationship3Pos].ToString());
                        var Education3 = Education3Pos == -1 ? string.Empty : dr[Education3Pos].ToString();
                        var Job3 = Job3Pos == -1 ? string.Empty : dr[Job3Pos].ToString();
                        var Company3 = Company3Pos == -1 ? string.Empty : dr[Company3Pos].ToString();

                        try
                        {
                            if (!string.IsNullOrEmpty(ParentName3))
                            {

                                Sql = string.Format(InitObject.GetScriptServiceInstance().ParentInsert, ParentID, StudentID, ParentName3, MobileNo3, Relationship3, "N", UserID, Education3, Job3, Company3);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                Sql = string.Format(InitObject.GetScriptServiceInstance().ParentRefInsert, StudentID, ParentID, "N");
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                Sql = string.Format(InitObject.GetScriptServiceInstance().PrivacyInsert, StudentID, ParentID, "N");
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                            }
                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat("Import Student Parent LogID:", dr["LogID"].ToString(), " Error Message:", ex.Message), SysLogType.ERROR, ModuleName));
                        }
                        finally
                        {

                        }


                        var StudentClassType = "STUCLASSTYPE09";
                        var StudentClassStatus = "STUCLASSSTATUS03";

                        if (!string.IsNullOrWhiteSpace(ClassName))
                        {
                            var ClassId = string.Empty;
                            Sql = string.Format(InitObject.GetScriptServiceInstance().GetClassIdByYKN, AcadYear, KindergartenId, ClassName);
                            var dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                            if (dt.Rows.Count == 1)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    ClassId = row["class_id"].ToString();
                                }
                            }

                            try
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().StudentIntoClass, ClassId, StudentID, StudentClassType, UserID, StudentClassStatus, CreateDate);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }
                            catch (Exception ex)
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat("Import Student Join Class LogID:", dr["LogID"].ToString(), " Error Message:", ex.Message), SysLogType.ERROR, ModuleName));
                            }
                            finally
                            {

                            }

                        }

                        Result = true;
                    }

                    if (IsTry2Import)
                    {
                        trans.Rollback();
                    }
                    else
                    {
                        trans.Commit();
                    }

                    Result = true;

                }
                catch (Exception ex)
                {
                    Result = false;
                    SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, string.Concat(ModuleName, " - ", "Import Student Bzns")));
                    trans.Rollback();
                }
                finally
                {
                    trans = null;
                }
            }

            return Result;

        }

        public bool UpdateRelationship(DataTable Students)
        {
            var Result = false;
            var Sql = string.Empty;
            var UpdateDate = InitObject.GetSysDate();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(BznsBase.GetConnectionString))
                {

                    MySqlTransaction trans = null;
                    conn.Open();
                    trans = conn.BeginTransaction();

                    try
                    {

                        //foreach (DataRow dr in Students.Rows)
                        //{
                        //    var StudentNo = dr[2].ToString();
                        //    var p1 = dr[7].ToString();
                        //    var r1 = dr[8].ToString();
                        //    var p2 = dr[11].ToString();
                        //    var r2 = dr[12].ToString();
                        //    var StudentInfo = new StudentInfo();
                        //    var dt = new DataTable();

                        //    Sql = string.Format(InitObject.GetScriptServiceInstance().GetClassIDByStudentID, StudentNo);
                        //    dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                        //    if (dt.Rows.Count > 0)
                        //    {
                        //        StudentInfo.ClassId = dt.Rows[0]["class_id"].ToString();
                        //        StudentInfo.StudentId = dt.Rows[0]["stu_id"].ToString();
                        //    }

                        //    if (!string.IsNullOrEmpty(r1) && !string.IsNullOrEmpty(p1)) {
                        //        Sql = string.Format(InitObject.GetScriptServiceInstance().UpdateRelationship, InitObject.GetRelationshipCodeByName(r1), StudentInfo.StudentId,p1);
                        //        DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                        //    }

                        //    if (!string.IsNullOrEmpty(r2) && !string.IsNullOrEmpty(p2))
                        //    {
                        //        Sql = string.Format(InitObject.GetScriptServiceInstance().UpdateRelationship, InitObject.GetRelationshipCodeByName(r2), StudentInfo.StudentId,p2);
                        //        DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                        //    }

                        //}

                        trans.Commit();

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                    finally
                    {
                        trans = null;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
            return Result;
        }




    }
}
