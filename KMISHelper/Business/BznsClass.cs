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
    public struct CalInfo {
        public string CalID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
    }

    class BznsClass
    {

        public readonly string ModuleName = "Class";

        public DataTable GetClassList() {

            var result = new DataTable();
            var KindergartenId = BznsBase.KindergartenId;
            var sql = string.Empty;

            try
            {
                sql = string.Format(InitObject.GetScriptServiceInstance().GetClassList, KindergartenId);
                result = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
            }
            catch (Exception)
            {

                throw;
            }
            finally {

            }

            return result;
        }

        public DataTable GetClassPlanList()
        {
            var Result = new DataTable();
            var sql = string.Empty;
            try
            {
                sql = string.Format(InitObject.GetScriptServiceInstance().GetClassPlanList,BznsBase.KindergartenId);
                Result = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
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

        public bool InsertPlan(string AcadYear, int PlanTitlePos, int PlanDescriptionPos, DataTable Plans, bool IsTry2Import)
        {

            var Result = false;
            var KindergartenId = BznsBase.KindergartenId;
            var BrandId = BznsBase.BrandID;
            var UserID = BznsBase.UserID;
            var Sql = string.Empty;
            var PlanID = string.Empty;
            var CreateDate = InitObject.GetSysDate();

            using (MySqlConnection conn = new MySqlConnection(BznsBase.GetConnectionString))
            {

                MySqlTransaction trans = null;
                conn.Open();
                trans = conn.BeginTransaction();

                try
                {

                    foreach (DataRow dr in Plans.Rows)
                    {

                        PlanID = InitObject.GetUUID();
                        var PlanTitle = PlanTitlePos == -1 ? string.Empty : dr[PlanTitlePos].ToString();
                        var PlanDescription = PlanDescriptionPos == -1 ? string.Empty : dr[PlanDescriptionPos].ToString();

                        try
                        {

                            Sql = string.Format(InitObject.GetScriptServiceInstance().ClassPlanInsert, PlanID, AcadYear, PlanTitle, PlanDescription, UserID, KindergartenId, BrandId, CreateDate);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                            Sql = string.Format(InitObject.GetScriptServiceInstance().ClassPlanLogInsert, InitObject.GetUUID(), PlanID, "开班计划申请", UserID, CreateDate, "", "", CreateDate);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                            Sql = string.Format(InitObject.GetScriptServiceInstance().ClassPlanLogInsert, InitObject.GetUUID(), PlanID, "开班计划审核", BznsBase.BrandUserID, CreateDate, "同意", "已审核通过", CreateDate);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat("Import Plan LogID:", dr["LogID"].ToString()," Error Message:",ex.Message), SysLogType.ERROR, ModuleName));
                        }
                        finally {

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

                    Result = true;

                }
                catch (Exception ex)
                {

                    SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, string.Concat(ModuleName," - ", "Import Class Plan Bzns")));
                    trans.Rollback();
                }
                finally
                {
                    trans = null;
                }
            }

            return Result;

        }

        public bool InsertClass(string AcadYear, string PlanID, int SchoolCalPos, int ClassTypePos,int ClassNamePos, int CapacityPos, int StartTimePos, int EndTimePos, int ClassAliasPos, int GradeTypePos,int AllowOverbookedPos,DataTable Classes, bool IsTry2Import)
        {

            var Result = false;
            var KindergartenId = BznsBase.KindergartenId;
            var BrandId = BznsBase.BrandID;
            var UserID = BznsBase.UserID;
            var Sql = string.Empty;
            var ClassID = string.Empty;
            var CreateDate = InitObject.GetSysDate();

            using (MySqlConnection conn = new MySqlConnection(BznsBase.GetConnectionString))
            {

                MySqlTransaction trans = null;
                conn.Open();
                trans = conn.BeginTransaction();

                try
                {

                    foreach (DataRow dr in Classes.Rows)
                    {
                        ClassID = InitObject.GetUUID();
                        var SchoolCal = SchoolCalPos == -1 ? string.Empty : dr[SchoolCalPos].ToString();
                        var ClassType = ClassTypePos == -1 ? string.Empty : InitObject.GetClassTypeByName(dr[ClassTypePos].ToString());
                        var ClassName = ClassNamePos == -1 ? string.Empty : dr[ClassNamePos].ToString();
                        var Capacity = CapacityPos == -1 ? 0:int.Parse(dr[CapacityPos].ToString());
                        var StartTime = StartTimePos == -1 ? string.Empty : dr[StartTimePos].ToString();
                        var EndTime = EndTimePos == -1 ? string.Empty : dr[EndTimePos].ToString();
                        var ClassAlias = ClassAliasPos == -1 ? string.Empty : dr[ClassAliasPos].ToString();
                        var GradeType = GradeTypePos == -1 ? string.Empty : InitObject.GetGradeTypeByName(dr[GradeTypePos].ToString());
                        var AllowOverbooked = AllowOverbookedPos == -1 ? string.Empty : dr[AllowOverbookedPos].ToString();
                        var CalInfo = new CalInfo();

                        if (!string.IsNullOrEmpty(SchoolCal)) {
                            CalInfo = GetCalInfoByTitle(SchoolCal);
                        }

                        try
                        {
                            Sql = string.Format(InitObject.GetScriptServiceInstance().ClassTempInsert, AcadYear, PlanID, ClassID, GradeType, ClassType, CalInfo.CalID, ClassName, ClassAlias, Capacity, AllowOverbooked, StartTime, EndTime, UserID, CreateDate, KindergartenId);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                            Sql = string.Format(InitObject.GetScriptServiceInstance().ClassInsert, AcadYear, PlanID, ClassID, GradeType, ClassType, CalInfo.CalID, ClassName, ClassAlias, Capacity, AllowOverbooked, StartTime, EndTime, UserID, CreateDate, KindergartenId);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat("Import Class LogID:", dr["LogID"].ToString(), " Error Message:", ex.Message), SysLogType.ERROR, ModuleName));
                        }
                        finally {

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

                    Result = true;

                }
                catch (Exception ex)
                {

                    SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, string.Concat(ModuleName, " - ", "Import Class Bzns")));
                    trans.Rollback();
                }
                finally
                {
                    trans = null;
                }
            }

            return Result;

        }

        public CalInfo GetCalInfoByTitle(string CalID) {
            var result = new CalInfo();
            var sql = string.Empty;
            try
            {
                sql = string.Format(InitObject.GetScriptServiceInstance().GetCalInfoByTitle, CalID);
                var dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count > 0) {
                    DataRow dr = dt.Rows[0];
                    result.CalID = dr["school_calendar_id"].ToString();
                    result.Title = dr["school_calendar_title"].ToString();
                    result.Type = dr["class_type"].ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
    }
}
