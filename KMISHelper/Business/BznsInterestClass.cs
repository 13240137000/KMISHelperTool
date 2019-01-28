using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMISHelper.DBHelper;
using KMISHelper.DBScript;
using KMISHelper.HelpGlobal;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;

namespace KMISHelper.Business
{
    class BznsInterestClass
    {

        public readonly string ModuleName = "Instrest Class";

        private CategoryInfo GetCategoryInfo(string CategoryName)
        {

            var CourseCategoryInfo = new CategoryInfo();
            var dt = new DataTable();

            try
            {
                var Sql = string.Format(InitObject.GetScriptServiceInstance().GetCourseCategoryByName, BznsBase.KindergartenId, CategoryName);

                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    CourseCategoryInfo.CategoryId = dt.Rows[0]["type_id"].ToString();
                    CourseCategoryInfo.Name = dt.Rows[0]["type_name"].ToString();
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                dt = null;
            }

            return CourseCategoryInfo;
        }

        private CourseInfo GetCourseInfo(string ClassName)
        {
            var Info = new CourseInfo();
            var Sql = string.Empty;
            var dt = new DataTable();
            try
            {
                Sql = string.Format(InitObject.GetScriptServiceInstance().GetInterestListByName, ClassName, BznsBase.KindergartenId);
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                if (dt.Rows.Count > 0)
                {

                    Info.Id = dt.Rows[0]["delay_class_id"].ToString();
                    Info.Name = dt.Rows[0]["class_nm"].ToString();
                    Info.Type = dt.Rows[0]["delay_type"].ToString();
                    Info.Hours = int.Parse(dt.Rows[0]["total_hours"].ToString());
                    Info.StartDate = Convert.ToDateTime(dt.Rows[0]["class_start_date"]);
                    Info.EndDate = Convert.ToDateTime(dt.Rows[0]["class_end_date"]);
                    Info.Price = Convert.ToDecimal(dt.Rows[0]["lesson_price"]);
                    Info.Amount = Convert.ToDecimal(dt.Rows[0]["total_amount"]);
                    Info.Category = dt.Rows[0]["lesson_type"].ToString();
                    Info.Status = dt.Rows[0]["status"].ToString();
                    Info.ChargeType = dt.Rows[0]["charge_type"].ToString();

                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                dt = null;
            }

            return Info;
        }

        private StudentInfo GetStudentInfo(string StudentNo)
        {

            var dt = new DataTable();
            var Sql = string.Empty;
            var Info = new StudentInfo();

            try
            {
                Sql = string.Format(InitObject.GetScriptServiceInstance().GetClassIDByStudentID, StudentNo);
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    Info.ClassId = dt.Rows[0]["class_id"].ToString();
                    Info.StudentId = dt.Rows[0]["stu_id"].ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return Info;

        }

        public bool InterestInsert(DataTable Classes, int ClassNamePos, int TypePos, int StartDatePos, int EndDatePos, int HoursPos,int PricePos,int AmountPos,int CategoryNamePos,int StatusPos, int ChargeTypePos,bool IsTry2Import) {

            var Result = false;
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
                        try
                        {

                            var ClassName = ClassNamePos == -1 ? string.Empty : dr[ClassNamePos].ToString().Trim();
                            var Type = TypePos == -1 ? string.Empty : InitObject.GetDelayClassType(dr[TypePos].ToString().Trim());
                            var StartDate = StartDatePos == -1 ? string.Empty : dr[StartDatePos].ToString().Trim();
                            var EndDate = EndDatePos == -1 ? string.Empty : dr[EndDatePos].ToString().Trim();
                            var Hours = HoursPos == -1 ? 0 : Convert.ToInt32(dr[HoursPos]);
                            var Price = PricePos == -1 ? 0 : Convert.ToDecimal(dr[PricePos]);
                            var Amount = AmountPos == -1 ? 0 : Convert.ToDecimal(dr[AmountPos]);
                            var CategoryName = CategoryNamePos == -1 ? string.Empty : dr[CategoryNamePos].ToString().Trim();
                            var Status = StatusPos == -1 ? string.Empty : InitObject.GetDelayClassStatus(dr[StatusPos].ToString().Trim());
                            var ChargeType = ChargeTypePos == -1 ? string.Empty : InitObject.GetDelayChargeType(dr[ChargeTypePos].ToString().Trim());
                            var CourseCategoryInfo = new CategoryInfo();
                            var CategoryID = InitObject.GetUUID();
                            var ClassId = InitObject.GetUUID();
                            var Sql = string.Empty;


                            // Get category, if not exist create one in category table.

                            if (Type == "EXTRACLASS")
                            {
                                CourseCategoryInfo = GetCategoryInfo(CategoryName);
                                if (string.IsNullOrEmpty(CourseCategoryInfo.CategoryId))
                                {
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().CategoryInsert, CategoryID, CategoryName, BznsBase.KindergartenId, BznsBase.UserID, CreateDate);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                    CourseCategoryInfo.CategoryId = CategoryID;
                                    CourseCategoryInfo.Name = CategoryName;
                                }
                            }

                            // Create one interest class.

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InterestInsert, ClassId, ClassName, Type, StartDate, EndDate, Hours, Price, Amount, (string.IsNullOrEmpty(CourseCategoryInfo.CategoryId)? string.Empty: CourseCategoryInfo.CategoryId), Status, BznsBase.KindergartenId, CreateDate, BznsBase.UserID, ChargeType);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);



                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(dr["LogID"].ToString(), ex.Message.ToString()), SysLogType.ERROR, string.Concat(ModuleName, " - ", "InterestInsert", " - ", "Class")));
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
                    SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, string.Concat(ModuleName, " - ", "InterestInsert", " - ", "Class")));
                    trans.Rollback();
                }

            }


            return Result;

        }

        public bool StudentInsert(DataTable Students, int StudentNoPos, int YearPos, int CourseNamePos, int ClassHoursPos, int MoneyPos,int PaidPos, int StartDatePos, int EndDatePos, bool IsTry2Import) {

            var Result = false;
            var CreateDate = InitObject.GetSysDate();

            using (MySqlConnection conn = new MySqlConnection(BznsBase.GetConnectionString))
            {

                MySqlTransaction trans = null;
                conn.Open();
                trans = conn.BeginTransaction();

                try
                {
                    foreach (DataRow dr in Students.Rows) {

                        try
                        {

                            int ret = 0;
                            var Sql = string.Empty;
                            var StudentNo = StudentNoPos == -1 ? string.Empty : dr[StudentNoPos].ToString().Trim();
                            var CourseName = CourseNamePos == -1 ? string.Empty : dr[CourseNamePos].ToString().Trim();
                            var Year = YearPos == -1 ? string.Empty : dr[YearPos].ToString().Trim();
                            var CourseInfo = GetCourseInfo(CourseName);
                            var StudentInfo = GetStudentInfo(StudentNo);
                            var YearInfo = InitObject.GetYearByTitle(Year);


                            // Add student to class

                            var RecordId = InitObject.GetUUID();

                            if (!string.IsNullOrEmpty(CourseInfo.Id) && !string.IsNullOrEmpty(StudentInfo.StudentId)) {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InterestStudentInsert, RecordId, CourseInfo.Id, StudentInfo.StudentId, 1);
                                ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }

                            // Add finance bill and ref

                            var BillId = InitObject.GetSysDate(true, true);
                            var BillStatus = "PAYMENTSTATE02";
                            var BillMonth = DateTime.Now.ToString("yyyy-MM");
                            var ClassHours = ClassHoursPos == -1 ? 0 : Convert.ToInt32(dr[ClassHoursPos]);
                            var Money = MoneyPos == -1 ? 0 : Convert.ToDecimal(dr[MoneyPos]);
                            var BillType = 1;
                            var BillAlert = "N";
                            var BillEffect = "Y";
                            var Paid = PaidPos == -1 ? "N" : "Y";
                            var PayMoney = Money;

                            var RefId = InitObject.GetUUID();
                            var StartDate = StartDatePos == -1 ? CourseInfo.StartDate.ToString("yyyy-MM-dd") : Convert.ToString(dr[StartDatePos]);
                            var EndDate = EndDatePos == -1 ? CourseInfo.EndDate.ToString("yyyy-MM-dd") : Convert.ToString(dr[EndDatePos]);
                            var RefBillType = "EXTRACLASS";
                            var RefBillNo = InitObject.GetBillNo(StartDate, string.Empty);
                            var RefStatus = "PAYMENTSTATE02";

                            #region Create student finance order info

                            if (ret > 0 && Money > 0)
                            {

                                // insert into bill

                                ret = 0;
                                if (Paid == "N") PayMoney = new decimal(0);
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InterestBillInsert, BillId, StudentInfo.StudentId, YearInfo.YearID, "", BillStatus, BillMonth,Money,PayMoney,BillType, BillAlert, BillEffect, CreateDate);
                                ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                // insert into bill ref

                                if (ret > 0) {
                                    ret = 0;
                                    if (Paid == "N") RefStatus = "PAYMENTSTATE01";
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InterestBillRefInsert, RefId, BillId, RefBillType, RefBillNo, CourseInfo.Id, BillMonth, ClassHours, CourseInfo.Hours, StartDate, EndDate, RefStatus, Money, PayMoney, CreateDate, CreateDate,BznsBase.UserID);
                                    ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                }

                            }

                            #endregion

                            #region Create student pay info

                            var PayId = InitObject.GetUUID();
                            var PayWay = InitObject.GetPaymentType(BznsBase.PaymentType);
                            var PayInfo = InitObject.GetPaymentInfo();
                            var PayStatus = "PAYMENTSTATE02";

                            var PayRefId = InitObject.GetUUID();
                            var PayRefStatus = "0";

                            
                            if (ret > 0) {

                                ret = 0;
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InterestBillPaymentInsert,PayId,BillId,RefBillNo,Money,PayWay,CreateDate,PayInfo.PaymentNo,PayInfo.PaymentNo,PayStatus,BznsBase.UserID,CreateDate);
                                ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                if (ret > 0) {
                                    ret = 0;
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InterestBillPaymentRefInsert, PayRefId, RefId, PayId, Money, PayStatus, CreateDate);
                                    ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                }

                            }

                            #endregion

                            #region Add money to account and balance

                            var StudentBalanceInfo = new StudentBalanceInfo();
                            var AccountId = InitObject.GetUUID();
                            Sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentBalanceListByStudentId, StudentInfo.StudentId);
                            var dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                            if (dt.Rows.Count > 0){
                                StudentBalanceInfo.BalanceId = dt.Rows[0]["balance_id"].ToString();
                                StudentBalanceInfo.StudentId = dt.Rows[0]["student_id"].ToString();
                                StudentBalanceInfo.Total = Convert.ToDecimal(dt.Rows[0]["balance_total"]);
                                StudentBalanceInfo.Income = Convert.ToDecimal(dt.Rows[0]["balance_income"]);
                            }

                            // Add into student balance account.

                            if (ret > 0 && !string.IsNullOrEmpty(StudentBalanceInfo.BalanceId)) {
                                ret = 0;
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InterestStudentBalanceAccountInsert,AccountId,StudentBalanceInfo.BalanceId,StudentInfo.StudentId, Money, "EXTRACLASS",BznsBase.UserID,CreateDate);
                                ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }

                            // Add into delay subject account

                            var DelayId = InitObject.GetUUID();

                            if (ret > 0) {
                                ret = 0;
                                Sql = string.Format(InitObject.GetScriptServiceInstance().DelayInsert, DelayId,AccountId,StudentInfo.StudentId,YearInfo.YearID,Money,"Y",Money, RefId,BznsBase.UserID,CreateDate);
                                ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }

                            // Add into student balance

                            if (ret > 0) {
                                ret = 0;
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InterestBalanceUpdate,Money,Money,StudentInfo.StudentId);
                                ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }

                            // Add into subject log

                            if (ret > 0) {
                                ret = 0;
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InterestSubjectLogInsert,StudentInfo.StudentId, "EXTRACLASS",DelayId, "REALGROWTH","延时服务初始化数据导入",Money,CreateDate,RefId,PayRefId);
                                ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }

                            // Add into balance log

                            if (ret > 0) {
                                ret = 0;
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InterestStudentBalanceLogInsert, "TRADETYPE01",StudentInfo.StudentId,Money,CreateDate, "EXTRACLASS","N",StudentBalanceInfo.BalanceId,BillId, "延时服务初始化数据导入");
                                ret = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }

                            #endregion


                        }
                        catch (Exception ex)
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(dr[StudentNoPos].ToString(), ex.Message.ToString()), SysLogType.ERROR, string.Concat(ModuleName, " - ", "Interest Student Insert", " - ", "Student")));
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
                    SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, string.Concat(ModuleName, " - ", "Interest Student Insert", " - ", "Student")));
                    trans.Rollback();
                }

            }

            return Result;
        }



    }



}
