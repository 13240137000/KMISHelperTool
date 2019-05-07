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
using System.Configuration;
using System.IO;

namespace KMISHelper.Business
{
    class BznsFinance
    {

        public readonly string ModuleName = "Finance";

        public void WriteLog(string StudentNo)
        {

            try
            {
                var FilePath = ConfigurationManager.AppSettings["WriteLogPath"];

                StreamWriter file = new System.IO.StreamWriter(FilePath, false);
                file.WriteLine(StudentNo);
                file.Close();
                file.Dispose();
            }
            catch (Exception e)
            {


            }

        }

        public bool BillInsert(DataTable Students, int StudentNoPos, int YearTitlePos, int MonthPos, int BillStartDatePos, int PaymentMethodPos, int TuitionFeeMoneyPos, int DiscountMoneyPos, int DiscountNamePos, int StartDatePos, int PerTuitionFeeMoneyPos, int mfMethodPos, int mfMoneyPos, int mfStartDatePos, int scMethodPos, int scMoneyPos, int scMonthPos, int scStartDatePos, List<OnceInfo> ofs, bool IsTry2Import)
        {

            var Result = false;
            var KindergartenId = BznsBase.KindergartenId;
            var UserID = BznsBase.UserID;
            var Sql = string.Empty;
            var CreateDate = InitObject.GetSysDate();
            var DescAlias = string.Empty;
            var DescMonthlyPrice = new decimal(0);

            using (MySqlConnection conn = new MySqlConnection(BznsBase.GetConnectionString))
            {

                MySqlTransaction trans = null;
                conn.Open();
                trans = conn.BeginTransaction();

                try
                {

                    foreach (DataRow dr in Students.Rows)
                    {

                        var DescBuilder = new StringBuilder().Clear();
                        var StudentNo = StudentNoPos == -1 ? string.Empty : dr[StudentNoPos].ToString();
                        WriteLog(StudentNo);
                        var ClassId = string.Empty;
                        var StudentId = string.Empty;
                        var myClassInfo = new ClassInfo();
                        var dt = new DataTable();
                        var StudentPaymentId = InitObject.GetUUID();
                        var PaymentResult = 0;
                        var BillResult = 0;
                        var BillID = string.Empty;
                        var BillMonth = DateTime.Now.ToString("yyyy-MM");
                        List<string> ofm = new List<string>();
                        var SPPInfo = new StudentPaymentPlanInfo();
                        var objAccountInfo = new AccountInfo();
                        


                        // Get StudentID and ClassID 

                        Sql = string.Format(InitObject.GetScriptServiceInstance().GetClassIDByStudentID, StudentNo);
                        dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                        if (dt.Rows.Count == 1)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                ClassId = row["class_id"].ToString();
                                StudentId = row["stu_id"].ToString();
                            }
                        }

                        if (string.IsNullOrWhiteSpace(StudentId)) {
                            continue;
                        }

                        // if app fin bill then get info and set id.

                        if (ConfigurationManager.AppSettings["IsAppend"].ToLower() == "yes")
                        {
                            SPPInfo = GetSPPByID(StudentId);
                            objAccountInfo = GetAccountInfo(StudentId);

                            if (string.IsNullOrWhiteSpace(SPPInfo.Intro) && !objAccountInfo.HasPrepaidAccount && !objAccountInfo.HasTuitionAccount && !objAccountInfo.HasMealsAccount &&!objAccountInfo.HasSchoolBusAccount && !objAccountInfo.HasOnetimeAccount)
                            {
                                BznsBase.IsAppend = false;
                            }
                            else
                            {
                                BznsBase.IsAppend = true;
                                StudentPaymentId = SPPInfo.ID;
                            }

                        }


                        // Get Class Info

                        Sql = string.Format(InitObject.GetScriptServiceInstance().GetClassListById, ClassId);
                        dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                        if (dt.Rows.Count == 1)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                myClassInfo.ClassId = row["class_id"].ToString();
                                myClassInfo.ClassName = row["class_nm"].ToString();
                                myClassInfo.CalendarId = row["school_calendar_id"].ToString();
                                myClassInfo.ClassType = row["class_type"].ToString();
                                myClassInfo.YearId = row["acad_year_id"].ToString();
                            }
                        }

                        // Get Year Info

                        var YearTitle = YearTitlePos == -1 ? string.Empty : dr[YearTitlePos].ToString();
                        var myYearInfo = new YearInfo();
                        myYearInfo = InitObject.GetYearByTitle(YearTitle);

                        // Get Monthly Info

                        var BillStartDate = Convert.ToDateTime(dr[BillStartDatePos]);
                        var MonthDict = new Dictionary<string, MonthInfo>();
                        MonthDict = InitObject.SplitMonth(BillStartDate, Convert.ToInt32(dr[MonthPos]), myClassInfo.CalendarId, myYearInfo.YearID);




                        // this method need add condition 


                        // Insert Into Payment Plan

                        if (!BznsBase.IsAppend)
                        {

                            if (!string.IsNullOrEmpty(StudentId))
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPayment, StudentPaymentId, StudentId, ClassId, KindergartenId, myClassInfo.ClassType, "Y", UserID, CreateDate);
                                PaymentResult = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }
                            else
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, "not found the student no."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Student Payment")));
                            }

                        }
                        else
                        {
                            PaymentResult = 1;
                        }

                        // Insert into fin bill
                    

                        if (PaymentResult > 0 || BznsBase.IsAppend)
                        {

                            BillID = InitObject.GetSysDate(true, true);
                            var BillStatus = "PAYMENTSTATE01";
                            var BillDateStart = MonthDict["summary"].StartDate.ToString("yyyy-MM-dd");
                            var BillDateEnd = MonthDict["summary"].EndDate.ToString("yyyy-MM-dd");

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBill, BillID, StudentId, myYearInfo.YearID, StudentPaymentId, myClassInfo.ClassId, BillStatus, BillMonth, "N", "Y", CreateDate, BillDateStart, BillDateEnd);
                            BillResult = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, "not found the student no, start date or month."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Plan")));
                        }


                        // Insert Into PaymentPlanRef - Tuition Fee 

                        var BatchNo = InitObject.GetSysDate(true);
                        var TuitionFeeMoney = TuitionFeeMoneyPos == -1 ? string.Empty : dr[TuitionFeeMoneyPos].ToString();
                        var DiscountMoney = DiscountMoneyPos == -1 ? string.Empty : dr[DiscountMoneyPos].ToString();
                        var PaymentMethod = PaymentMethodPos == -1 ? string.Empty : InitObject.GetPaymentModeByName(dr[PaymentMethodPos].ToString());

                        var DiscountName = DiscountNamePos == -1 ? string.Empty : dr[DiscountNamePos].ToString();
                        var StartDate = StartDatePos == -1 ? string.Empty : Convert.ToDateTime(dr[StartDatePos]).ToString("yyyy-MM-dd");
                        var TuitionFeeSubject = "PAYMENTSUBJECT01";
                        var RateId = string.Empty;
                        var DiscountID = string.Empty;
                        var DiscountType = string.Empty;
                        var DetailID = InitObject.GetUUID();

                        var DiscountedMoney = new decimal(0);

                        // Get RateId
                        if (!string.IsNullOrEmpty(TuitionFeeMoney) && Convert.ToDecimal(TuitionFeeMoney) > 0)
                        {
                            Sql = string.Format(InitObject.GetScriptServiceInstance().GetRateIdByKMS, Convert.ToDecimal(TuitionFeeMoney), TuitionFeeSubject, KindergartenId, myYearInfo.YearID);
                            dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                            if (dt.Rows.Count >= 1)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    RateId = row["rates_id"].ToString();
                                    DescAlias = row["rates_alias"].ToString();
                                    DescMonthlyPrice = Convert.ToDecimal(row["rates_monthly"]);
                                }
                            }
                        }

                        // Get DiscountID and DiscountType

                        if (!string.IsNullOrEmpty(DiscountMoney) && !string.IsNullOrEmpty(TuitionFeeMoney) && Convert.ToDecimal(DiscountMoney) > 0 && Convert.ToDecimal(TuitionFeeMoney) > 0)
                        {

                            if (!string.IsNullOrEmpty(DiscountName.Trim()))
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetDiscountIdByKMSN, TuitionFeeSubject, KindergartenId, DiscountName);
                            }
                            else
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetDiscountIdByKMS, TuitionFeeSubject, KindergartenId, (Convert.ToDecimal(TuitionFeeMoney) - Convert.ToDecimal(DiscountMoney)));
                            }

                            dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    DiscountID = row["discount_id"].ToString();
                                    DiscountType = row["discount_type"].ToString();
                                }
                            }
                            DiscountedMoney = Convert.ToDecimal(TuitionFeeMoney) - Convert.ToDecimal(DiscountMoney);
                        }

                        // Insert into Tuition

                        if (!string.IsNullOrEmpty(TuitionFeeMoney) && Convert.ToDecimal(TuitionFeeMoney) > 0 && !string.IsNullOrEmpty(RateId) && PaymentResult > 0 && BillResult > 0)
                        {


                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, DetailID, RateId, BatchNo, StudentPaymentId, PaymentMethod, TuitionFeeSubject, Convert.ToDecimal(TuitionFeeMoney), DiscountID, DiscountType, DiscountMoney, 0, 1, UserID, CreateDate, StartDate, DiscountedMoney);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            DescBuilder.Append(string.Format("【学费: {0}/月 ，缴费方式:{1}，标准:{2} 】", DescMonthlyPrice, dr[PaymentMethodPos].ToString(), DescAlias));

                            // Insert into bill ref
                            var BillRefId = InitObject.GetUUID();
                            var BillNo = InitObject.GetBillNo(StartDate, TuitionFeeSubject);
                            var BillStatus = "PAYMENTSTATE01";

                            var tbStartDate = Convert.ToDateTime(dr[StartDatePos]);
                            var tbMonthDict = new Dictionary<string, MonthInfo>();
                            MonthDict = InitObject.SplitMonth(tbStartDate, Convert.ToInt32(dr[MonthPos]), myClassInfo.CalendarId, myYearInfo.YearID);

                            var tbsd = MonthDict["summary"].StartDate.ToString("yyyy-MM-dd");
                            var tbed = MonthDict["summary"].EndDate.ToString("yyyy-MM-dd");
                            var tbMoney = new decimal(0);

                            if (DiscountMoneyPos != -1)
                            {
                                tbMoney = Convert.ToDecimal(DiscountMoney) * Convert.ToInt32(dr[MonthPos]);
                            }
                            else
                            {
                                tbMoney = Convert.ToDecimal(TuitionFeeMoney) * Convert.ToInt32(dr[MonthPos]);
                            }

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRef, BillRefId, BillID, TuitionFeeSubject, BillNo, DetailID, BillMonth, BillStatus, CreateDate, tbsd, tbed, tbMoney);
                            if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Ref Tuition Fee")));
                            }

                            // Insert into bill plan.

                            var IsFirstMonth = true;
                            foreach (KeyValuePair<string, MonthInfo> kv in MonthDict)
                            {

                                if (!kv.Key.Equals("summary"))
                                {

                                    var key = kv.Key;
                                    var val = kv.Value;
                                    var BillPlanID = InitObject.GetUUID();
                                    var bpDiscountMoney = new decimal(0);
                                    var FirstMonth = "N";
                                    if (!string.IsNullOrEmpty(DiscountMoney))
                                    {
                                        bpDiscountMoney = Convert.ToDecimal(DiscountMoney);
                                    }


                                    if (IsFirstMonth)
                                    {
                                        FirstMonth = "Y";
                                    }


                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, TuitionFeeSubject, DetailID, 1, FirstMonth, "Y", "N", CreateDate, val.StudyDays, Convert.ToDecimal(TuitionFeeMoney), DiscountMoney, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));

                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Plan Tuition Fee")));
                                    }
                                    IsFirstMonth = false;
                                }

                            }



                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set the tuition or fee <= 0 or not find rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Tuition Fee")));
                        }

                        // Insert Log
                        try
                        {
                            if (!string.IsNullOrEmpty(DiscountMoney) && Convert.ToDecimal(DiscountMoney) > 0)
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertDiscountApprovedLog, InitObject.GetUUID(), StudentPaymentId, "折扣审核", UserID, CreateDate, "审核通过", CreateDate);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }
                        }
                        catch (Exception)
                        {

                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " create discount approve log failed."), SysLogType.ERROR, string.Concat(ModuleName, " - ", "Discount Log")));
                        }



                        // Insert Per Tuition Fee

                        var PerTuitionFeeMoney = PerTuitionFeeMoneyPos == -1 ? string.Empty : dr[PerTuitionFeeMoneyPos].ToString();
                        var PerRateID = string.Empty;
                        var PerDescAlias = string.Empty;
                        var PerDescMonthlyPrice = new decimal(0);
                        var PerTuitionFeeSubject = "PAYMENTSUBJECT05";
                        var PerDetailID = InitObject.GetUUID();

                        if (!string.IsNullOrEmpty(PerTuitionFeeMoney) && Convert.ToDecimal(PerTuitionFeeMoney) > 0 && PaymentResult > 0 && BillResult > 0)
                        {
                            Sql = string.Format(InitObject.GetScriptServiceInstance().GetRateIdByKMS, Convert.ToDecimal(PerTuitionFeeMoney), PerTuitionFeeSubject, KindergartenId, myYearInfo.YearID);
                            dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                            if (dt.Rows.Count == 0)
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetRateIdByKS, PerTuitionFeeSubject, KindergartenId, myYearInfo.YearID);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                            }

                            if (dt.Rows.Count >= 1)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    PerRateID = row["rates_id"].ToString();
                                    PerDescAlias = row["rates_alias"].ToString();
                                    PerDescMonthlyPrice = Convert.ToDecimal(row["rates_monthly"]);
                                }
                            }


                            if (!string.IsNullOrEmpty(PerRateID) && PaymentResult > 0)
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, PerDetailID, PerRateID, BatchNo, StudentPaymentId, "", PerTuitionFeeSubject, Convert.ToDecimal(PerTuitionFeeMoney), "", "", 0, 0, 1, UserID, CreateDate, CreateDate, 0);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                DescBuilder.Append(string.Format("【预缴学费: {0} ，标准:{1} 】", PerDescMonthlyPrice, DescAlias));

                                // Insert into bill ref.
                                var BillRefId = InitObject.GetUUID();
                                var BillNo = InitObject.GetBillNo(CreateDate, PerTuitionFeeSubject);
                                var BillStatus = "PAYMENTSTATE01";


                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRefNoInit, BillRefId, BillID, PerTuitionFeeSubject, BillNo, PerDetailID, BillMonth, BillStatus, CreateDate, Convert.ToDecimal(PerTuitionFeeMoney));
                                if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                {
                                    SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                                }

                                if (!BznsBase.IsAppend)
                                {
                                    // Insert into bill Plan.
                                    var BillPlanID = InitObject.GetUUID();

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillNoLoop, BillPlanID, BillNo, PerTuitionFeeSubject, PerDetailID, 1, "N", "Y", "N", CreateDate, 0, PerTuitionFeeMoney, PerTuitionFeeMoney);

                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                                    }
                                }



                            }
                            else
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, "not find rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                            }



                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set per tuition or fee <= 0."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                        }


                        // Insert Meals Fee

                        var mfMoney = mfMoneyPos == -1 ? string.Empty : dr[mfMoneyPos].ToString();
                        var mfMethod = mfMethodPos == -1 ? string.Empty : InitObject.GetPaymentModeByName(dr[mfMethodPos].ToString());
                        var mfRateID = string.Empty;
                        var mfDescAlias = string.Empty;
                        var mfDescMonthlyPrice = new decimal(0);
                        var mfSubject = "PAYMENTSUBJECT02";
                        var mfDetailID = InitObject.GetUUID();
                        var mfStartDate = mfStartDatePos == -1 ? string.Empty : Convert.ToDateTime(dr[mfStartDatePos]).ToString("yyyy-MM-dd");

                        if (!string.IsNullOrEmpty(mfMoney) && Convert.ToDecimal(mfMoney) > 0)
                        {
                            Sql = string.Format(InitObject.GetScriptServiceInstance().GetRateIdByKMS, Convert.ToDecimal(mfMoney), mfSubject, KindergartenId, myYearInfo.YearID);
                            dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                            if (dt.Rows.Count >= 1)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    mfRateID = row["rates_id"].ToString();
                                    mfDescAlias = row["rates_alias"].ToString();
                                    mfDescMonthlyPrice = Convert.ToDecimal(row["rates_monthly"]);
                                }
                            }

                            if (!string.IsNullOrEmpty(mfRateID) && PaymentResult > 0 && BillResult > 0)
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, mfDetailID, mfRateID, BatchNo, StudentPaymentId, mfMethod, mfSubject, Convert.ToDecimal(mfMoney), "", "", 0, 0, 1, UserID, CreateDate, mfStartDate, 0);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                DescBuilder.Append(string.Format("【餐费: {0}/月 ，缴费方式:{1}，标准:{2} 】", mfDescMonthlyPrice, dr[mfMethodPos].ToString(), mfDescAlias));

                                // Insert into bill ref.
                                var BillRefId = InitObject.GetUUID();
                                var BillNo = InitObject.GetBillNo(mfStartDate, mfSubject);
                                var BillStatus = "PAYMENTSTATE01";

                                var mfbStartDate = Convert.ToDateTime(dr[mfStartDatePos]);
                                var mfbMonthDict = new Dictionary<string, MonthInfo>();
                                MonthDict = InitObject.SplitMonth(mfbStartDate, Convert.ToInt32(dr[MonthPos]), myClassInfo.CalendarId, myYearInfo.YearID);

                                var mfbsd = MonthDict["summary"].StartDate.ToString("yyyy-MM-dd");
                                var mfbed = MonthDict["summary"].EndDate.ToString("yyyy-MM-dd");
                                var mfbMoney = new decimal(0);

                                mfbMoney = Convert.ToDecimal(mfMoney) * Convert.ToInt32(dr[MonthPos]);


                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRef, BillRefId, BillID, mfSubject, BillNo, mfDetailID, BillMonth, BillStatus, CreateDate, mfbsd, mfbed, mfbMoney);
                                if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                {
                                    SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Meals Fee")));
                                }

                                // Insert into bill plan.


                                var IsFirstMonth = true;
                                foreach (KeyValuePair<string, MonthInfo> kv in MonthDict)
                                {

                                    if (!kv.Key.Equals("summary"))
                                    {

                                        var key = kv.Key;
                                        var val = kv.Value;
                                        var BillPlanID = InitObject.GetUUID();
                                        var bpDiscountMoney = new decimal(0);
                                        var FirstMonth = "N";

                                        if (IsFirstMonth)
                                        {
                                            FirstMonth = "Y";
                                        }


                                        Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, mfSubject, mfDetailID, 1, FirstMonth, "Y", "N", CreateDate, val.StudyDays, Convert.ToDecimal(mfMoney), Convert.ToDecimal(mfMoney), val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));

                                        if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                        {
                                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Meals Fee")));
                                        }
                                        IsFirstMonth = false;
                                    }

                                }

                            }
                            else
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, "not find rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Meals Fee")));
                            }



                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set meals fee or fee <= 0."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Meals Fee")));
                        }




                        // Insert School Car Fee

                        var scMoney = scMoneyPos == -1 ? string.Empty : dr[scMoneyPos].ToString();
                        var scMonth = scMonthPos == -1 ? 0 : Convert.ToInt32(dr[scMonthPos]);
                        var scMethod = scMethodPos == -1 ? string.Empty : InitObject.GetPaymentModeByName(dr[scMethodPos].ToString());
                        var scRateID = string.Empty;
                        var scDescAlias = string.Empty;
                        var scDescMonthlyPrice = new decimal(0);
                        var scSubject = "PAYMENTSUBJECT04";
                        var scDetailID = InitObject.GetUUID();
                        var scStartDate = scStartDatePos == -1 ? string.Empty : dr[scStartDatePos].ToString();

                        if (!string.IsNullOrEmpty(scStartDate))
                        {
                            scStartDate = Convert.ToDateTime(scStartDate).ToString("yyyy-MM-dd");
                        }

                        if (!string.IsNullOrEmpty(scMoney) && Convert.ToDecimal(scMoney) > 0)
                        {
                            Sql = string.Format(InitObject.GetScriptServiceInstance().GetRateIdByKMS, Convert.ToDecimal(scMoney), scSubject, KindergartenId, myYearInfo.YearID);
                            dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                            if (dt.Rows.Count >= 1)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    scRateID = row["rates_id"].ToString();
                                    scDescAlias = row["rates_alias"].ToString();
                                    scDescMonthlyPrice = Convert.ToDecimal(row["rates_monthly"]);
                                }
                            }

                            if (!string.IsNullOrEmpty(scRateID) && PaymentResult > 0 && BillResult > 0)
                            {

                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, scDetailID, scRateID, BatchNo, StudentPaymentId, scMethod, scSubject, Convert.ToDecimal(scMoney), "", "", 0, 0, 1, UserID, CreateDate, scStartDate, 0);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                DescBuilder.Append(string.Format("【校车费: {0}/月 ，缴费方式:{1}，标准:{2} 】", scDescMonthlyPrice, dr[scMethodPos].ToString(), scDescAlias));

                                // Insert into bill ref.
                                var BillRefId = InitObject.GetUUID();
                                var BillNo = InitObject.GetBillNo(scStartDate, scSubject);
                                var BillStatus = "PAYMENTSTATE01";

                                var scbStartDate = Convert.ToDateTime(dr[scStartDatePos]);
                                var scbMonthDict = new Dictionary<string, MonthInfo>();
                                MonthDict = InitObject.SplitMonth(scbStartDate, scMonth, myClassInfo.CalendarId, myYearInfo.YearID);

                                var scbsd = MonthDict["summary"].StartDate.ToString("yyyy-MM-dd");
                                var scbed = MonthDict["summary"].EndDate.ToString("yyyy-MM-dd");
                                var scbMoney = new decimal(0);

                                scbMoney = Convert.ToDecimal(scMoney) * scMonth;

                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRef, BillRefId, BillID, scSubject, BillNo, scDetailID, BillMonth, BillStatus, CreateDate, scbsd, scbed, scbMoney);
                                if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                {
                                    SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import School Car Fee")));
                                }

                                // Insert into bill plan.

                                var IsFirstMonth = true;
                                foreach (KeyValuePair<string, MonthInfo> kv in MonthDict)
                                {

                                    if (!kv.Key.Equals("summary"))
                                    {

                                        var key = kv.Key;
                                        var val = kv.Value;
                                        var BillPlanID = InitObject.GetUUID();
                                        var bpDiscountMoney = new decimal(0);
                                        var FirstMonth = "N";

                                        if (IsFirstMonth)
                                        {
                                            FirstMonth = "Y";
                                        }


                                        Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, scSubject, scDetailID, 1, FirstMonth, "Y", "N", CreateDate, val.StudyDays, Convert.ToDecimal(scMoney), Convert.ToDecimal(scMoney), val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));

                                        if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                        {
                                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import School Car Fee")));
                                        }
                                        IsFirstMonth = false;
                                    }

                                }


                            }
                            else
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont find car rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import School Car Fee")));
                            }

                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set school car fee or <= 0."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import School Car Fee")));
                        }



                        // Insert Once Fee
                        foreach (var info in ofs)
                        {


                            var ofMoney = int.Parse(info.Key.ToString()) == -1 ? string.Empty : dr[int.Parse(info.Key.ToString())].ToString();
                            var ofRateID = string.Empty;
                            var ofDescAlias = string.Empty;
                            var ofDescMonthlyPrice = new decimal(0);
                            var ofSubject = "PAYMENTSUBJECT03";
                            var ofDetailID = InitObject.GetUUID();

                            if (!string.IsNullOrEmpty(ofMoney) && Convert.ToDecimal(ofMoney) > 0)
                            {



                                //Sql = string.Format(InitObject.GetScriptServiceInstance().GetRateIdByKMS, Convert.ToDecimal(ofMoney), ofSubject, KindergartenId, myYearInfo.YearID);
                                //dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetRateIdByAliasName, ofSubject, KindergartenId, myYearInfo.YearID, info.Value);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                if (dt.Rows.Count >= 1)
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        ofRateID = row["rates_id"].ToString();
                                        ofDescAlias = row["rates_alias"].ToString();
                                        ofDescMonthlyPrice = Convert.ToDecimal(row["rates_monthly"]);
                                        ofm.Add(ofMoney);
                                    }
                                }

                                if (!string.IsNullOrEmpty(ofRateID) && PaymentResult > 0 && BillResult > 0)
                                {
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, ofDetailID, ofRateID, BatchNo, StudentPaymentId, "", ofSubject, Convert.ToDecimal(ofMoney), "", "", 0, 0, 1, UserID, CreateDate, CreateDate, 0);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                    DescBuilder.Append(string.Format("【{0}: {1} ，标准:{1} 】", ofDescAlias, ofDescMonthlyPrice, ofDescAlias));

                                    // Insert into bill ref.
                                    var BillRefId = InitObject.GetUUID();
                                    var BillNo = InitObject.GetBillNo(CreateDate, ofSubject);
                                    var BillStatus = "PAYMENTSTATE01";

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRefNoInit, BillRefId, BillID, ofSubject, BillNo, ofDetailID, BillMonth, BillStatus, CreateDate, Convert.ToDecimal(ofMoney));
                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                                    }


                                    if (!BznsBase.IsAppend)
                                    {
                                        // Insert into bill plan.
                                        var BillPlanID = InitObject.GetUUID();

                                        Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillNoLoop, BillPlanID, BillNo, ofSubject, ofDetailID, 1, "N", "Y", "N", CreateDate, 0, Convert.ToDecimal(ofMoney), Convert.ToDecimal(ofMoney));

                                        if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                        {
                                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                                        }
                                    }


                                }
                                else
                                {
                                    SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont find once fee rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                                }



                            }
                            else
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set once fee or <= 0."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                            }
                            // End Insert Once Fee

                        }

                        // update payment plan

                        if (!BznsBase.IsAppend)
                        {
                            Sql = string.Format(InitObject.GetScriptServiceInstance().UpdateStudentPayment, DescBuilder.ToString(), StudentPaymentId);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                        }

                        //if (BznsBase.IsAppend) {
                        //    TuitionFeeMoney = string.Empty;
                        //    DiscountMoney = string.Empty;
                        //    mfMoney = string.Empty;
                        //    scMoney = string.Empty;
                        //    scMonth = 0;
                        //}

                        var sum = InitObject.TotalBillMoney(PerTuitionFeeMoney, TuitionFeeMoney, DiscountMoney, mfMoney, scMoney, ofm, Convert.ToInt32(dr[MonthPos]), scMonth);
                        Sql = string.Format(InitObject.GetScriptServiceInstance().UpdateFinBill, sum, 0, BillID);
                        DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

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

                    SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, string.Concat(ModuleName, " - ", "Import Payment Bzns")));
                    trans.Rollback();
                }

                finally
                {
                    trans = null;
                }

            }

            return Result;
        }

        public bool ChargeInsert(DataTable Students, int StudentNoPos, bool IsTry2Import = true)
        {

           var result = false;
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

                        var StudentNo = StudentNoPos == -1 ? string.Empty : dr[StudentNoPos].ToString();
                        var BillInfo = new BillInfo();
                        var StudentBalanceInfo = new StudentBalanceInfo();
                        List<BillRefInfo> BillRefs = new List<BillRefInfo>();
                        var PreTuitionInfo = new PreTuitionInfo();
                        var ClassId = string.Empty;
                        var StudentId = string.Empty;
                        var Sql = string.Empty;
                        var dt = new DataTable();
                        var PreTuitionMoney = new decimal(0);
                        var myAccountInfo = new AccountInfo();

                        try
                        {
                            if (!string.IsNullOrWhiteSpace(StudentNo))
                            {

                                // Get StudentID and ClassID

                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetClassIDByStudentID, StudentNo);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                if (dt.Rows.Count > 0)
                                {
                                    ClassId = dt.Rows[0]["class_id"].ToString();
                                    StudentId = dt.Rows[0]["stu_id"].ToString();
                                }

                                //judge isappend?
                                if (ConfigurationManager.AppSettings["IsAppend"].ToLower() == "yes")
                                {
                                    myAccountInfo = GetAccountInfo(StudentId);

                                    if (!myAccountInfo.HasPrepaidAccount && !myAccountInfo.HasTuitionAccount && !myAccountInfo.HasSchoolBusAccount && !myAccountInfo.HasMealsAccount && !myAccountInfo.HasOnetimeAccount)
                                    {
                                        BznsBase.IsAppend = false;
                                    }
                                    else
                                    {
                                        BznsBase.IsAppend = true;
                                    }
                                }
                                else {
                                    BznsBase.IsAppend = false;
                                }

                                

                                // Get Student Balance Info

                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentBalanceListByStudentId, StudentId);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                if (dt.Rows.Count > 0)
                                {
                                    StudentBalanceInfo.BalanceId = dt.Rows[0]["balance_id"].ToString();
                                    StudentBalanceInfo.StudentId = dt.Rows[0]["student_id"].ToString();
                                    StudentBalanceInfo.Total = Convert.ToDecimal(dt.Rows[0]["balance_total"]);
                                    StudentBalanceInfo.Income = Convert.ToDecimal(dt.Rows[0]["balance_income"]);
                                }


                                // Get Bill Info

                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetBillListByStudentId, StudentId);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                if (dt.Rows.Count > 0)
                                {
                                    BillInfo.BillId = dt.Rows[0]["bill_id"].ToString();
                                    BillInfo.ClassId = dt.Rows[0]["class_id"].ToString();
                                    BillInfo.StudentId = dt.Rows[0]["stu_id"].ToString();
                                    BillInfo.YearId = dt.Rows[0]["acad_year_id"].ToString();
                                    BillInfo.SumMoney = Convert.ToDecimal(dt.Rows[0]["bill_sum_money"]);
                                }

                                // Get Bill Ref Info

                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetBillRefListByBillId, BillInfo.BillId);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                if (dt.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        var BillRefInfo = new BillRefInfo();

                                        BillRefInfo.RefID = row["bill_ref_id"].ToString();
                                        BillRefInfo.Type = row["bill_type"].ToString();
                                        if (!string.IsNullOrEmpty(row["bill_date_start"].ToString())) BillRefInfo.StartDate = Convert.ToDateTime(row["bill_date_start"]);
                                        if (!string.IsNullOrEmpty(row["bill_date_end"].ToString())) BillRefInfo.EndDate = Convert.ToDateTime(row["bill_date_end"]);
                                        BillRefInfo.Status = row["bill_status"].ToString();
                                        BillRefInfo.Money = Convert.ToDecimal(row["bill_money"]);
                                        BillRefInfo.Payment = Convert.ToDecimal(row["bill_payment"]);

                                        if (row["bill_type"].ToString() == "PAYMENTSUBJECT05")
                                        {
                                            PreTuitionMoney = Convert.ToDecimal(row["bill_money"]);
                                        }

                                        BillRefs.Add(BillRefInfo);
                                    }
                                }

                                // Get Pre Info

                                PreTuitionInfo = GetPreTuitionInfo(BillRefs);

                                // Create Student Payment

                                if (!string.IsNullOrEmpty(BillInfo.BillId) && BillRefs.Count > 0)
                                {

                                    var PaymentId = InitObject.GetUUID();
                                    var PaymentWay = InitObject.GetPaymentType(BznsBase.PaymentType);
                                    var PaymentNo = InitObject.GetPaymentInfo().PaymentNo;
                                    var PaymentStatus = "PAYMENTSTATE02";
                                    var ReceiptNo = InitObject.GetPaymentInfo().ReceiptNo;

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().PaymentInsert, PaymentId, BillInfo.BillId, BillInfo.SumMoney, PaymentWay, CreateDate, PaymentNo, PaymentStatus, BznsBase.UserID, CreateDate, ReceiptNo);
                                    var IsPaymentCreated = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                    if (IsPaymentCreated > 0)
                                    {
                                        // is append need to get IsHasOnceSubject
                                        var IsHasOnceSubject = false;
                                        var OnceSubjectAccount = string.Empty;
                                        var OnceMoney = new decimal(0);

                                        
                                        foreach (BillRefInfo bri in BillRefs)
                                        {

                                            // Create Payment Ref

                                            var PaymentRefId = InitObject.GetUUID();

                                            if (PreTuitionMoney > 0 && bri.Type == "PAYMENTSUBJECT01")
                                            {
                                                Sql = string.Format(InitObject.GetScriptServiceInstance().PaymentRefInsert, PaymentRefId, bri.RefID, PaymentId, bri.Money - PreTuitionMoney, CreateDate);
                                            }
                                            else
                                            {
                                                Sql = string.Format(InitObject.GetScriptServiceInstance().PaymentRefInsert, PaymentRefId, bri.RefID, PaymentId, bri.Money, CreateDate);
                                            }

                                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                            // Create Student Balance Account

                                            var AccountID = InitObject.GetUUID();

                                            if (bri.Type.Trim() != "PAYMENTSUBJECT03")
                                            {
                                                if (bri.Type.Trim() == "PAYMENTSUBJECT05")
                                                {

                                                    // Is Append.

                                                    var val = new decimal(0);

                                                    if (!PreTuitionInfo.IsDeductionPreTuition)
                                                    {
                                                        val = PreTuitionInfo.PreTuitionMoney;
                                                    }

                                                    if (BznsBase.IsAppend)
                                                    {
                                                        if (myAccountInfo.HasPrepaidAccount)
                                                        {
                                                            AccountID = myAccountInfo.PrepaidAccountID;
                                                            Sql = string.Format(InitObject.GetScriptServiceInstance().SBAUpdate, val, val, BznsBase.UserID, CreateDate, myAccountInfo.PrepaidAccountID);
                                                        }
                                                        else
                                                        {
                                                            Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, val, bri.Type, BznsBase.UserID, CreateDate);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, val, bri.Type, BznsBase.UserID, CreateDate);
                                                    }


                                                }
                                                else
                                                {
                                                    // subject is 1,2,4
                                                    if (BznsBase.IsAppend)
                                                    {
                                                        switch (bri.Type.Trim()) {
                                                            case "PAYMENTSUBJECT01":
                                                                if (myAccountInfo.HasTuitionAccount)
                                                                {
                                                                    AccountID = myAccountInfo.TuitionAccountID;
                                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().SBAUpdate, bri.Money, 0, BznsBase.UserID, CreateDate, AccountID);
                                                                }
                                                                else {
                                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, bri.Money, bri.Type, BznsBase.UserID, CreateDate);
                                                                }
                                                                break;
                                                            case "PAYMENTSUBJECT02":
                                                                if (myAccountInfo.HasMealsAccount)
                                                                {
                                                                    AccountID = myAccountInfo.MealsAccountID;
                                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().SBAUpdate, bri.Money, 0, BznsBase.UserID, CreateDate, AccountID);
                                                                }
                                                                else {
                                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, bri.Money, bri.Type, BznsBase.UserID, CreateDate);
                                                                }
                                                                break;
                                                            case "PAYMENTSUBJECT04":
                                                                if (myAccountInfo.HasSchoolBusAccount)
                                                                {
                                                                    AccountID = myAccountInfo.SchoolBusAccountID;
                                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().SBAUpdate, bri.Money, 0, BznsBase.UserID, CreateDate, AccountID);
                                                                }
                                                                else {
                                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, bri.Money, bri.Type, BznsBase.UserID, CreateDate);
                                                                }
                                                                break;
                                                        }
                                                        
                                                    }
                                                    else {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, bri.Money, bri.Type, BznsBase.UserID, CreateDate);
                                                    }

                                                }
                                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                            }
                                            else
                                            {
                                                // is append if have one time subject update else insert.

                                                if ((BznsBase.IsAppend && myAccountInfo.HasOnetimeAccount) || IsHasOnceSubject)
                                                {
                                                    AccountID = myAccountInfo.OnetimeAccountID;
                                                    OnceSubjectAccount = myAccountInfo.OnetimeAccountID.ToString();
                                                    //OnceMoney = OnceMoney + bri.Money;
                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountUpdate, bri.Money, OnceSubjectAccount);
                                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                                }
                                                else {
                                                    IsHasOnceSubject = true;
                                                    myAccountInfo.HasOnetimeAccount = true;
                                                    myAccountInfo.OnetimeAccountID = AccountID;
                                                    OnceMoney = bri.Money;
                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, bri.Money, bri.Type, BznsBase.UserID, CreateDate);
                                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                                }

                                            }

                                            // Create subject account

                                            var SubjectAccountId = InitObject.GetUUID();


                                            // Create subject log.

                                            Sql = string.Format(InitObject.GetScriptServiceInstance().SubjectLogInsert, StudentId, bri.Type, SubjectAccountId, "REALGROWTH", "导入初始化金额", bri.Money, CreateDate, bri.RefID, PaymentRefId);
                                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                            switch (bri.Type.ToUpper().Trim())
                                            {
                                                case "PAYMENTSUBJECT01":
                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().TuitionAccountInsert, SubjectAccountId, AccountID, StudentId, BillInfo.YearId, bri.Money, 'Y', bri.Money, bri.RefID, BznsBase.UserID, CreateDate);
                                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                                    break;
                                                case "PAYMENTSUBJECT02":
                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().MealsAccountInsert, SubjectAccountId, AccountID, StudentId, BillInfo.YearId, bri.Money, 'Y', bri.Money, bri.RefID, BznsBase.UserID, CreateDate);
                                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                                    break;
                                                case "PAYMENTSUBJECT03":
                                                    // is append
                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().ItemAccountInsert, SubjectAccountId, AccountID, StudentId, BillInfo.YearId, bri.Money, 'Y', bri.Money, bri.RefID, BznsBase.UserID, CreateDate);
                                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                                    break;
                                                case "PAYMENTSUBJECT04":
                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().SchoolBusAccountInsert, SubjectAccountId, AccountID, StudentId, BillInfo.YearId, bri.Money, 'Y', bri.Money, bri.RefID, BznsBase.UserID, CreateDate);
                                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                                    break;
                                                case "PAYMENTSUBJECT05":
                                                    // is append
                                                    //BillInfo.SumMoney = BillInfo.SumMoney - bri.Money;
                                                    var val = new decimal(0);

                                                    if (!PreTuitionInfo.IsDeductionPreTuition)
                                                    {
                                                        val = PreTuitionInfo.PreTuitionMoney;
                                                    }

                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().PrepaidTuitionAccountInsert, SubjectAccountId, AccountID, StudentId, BillInfo.YearId, val, 'Y', bri.Money, bri.RefID, BznsBase.UserID, CreateDate);
                                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                                    if (PreTuitionInfo.IsLog)
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().SubjectLogInsert, StudentId, bri.Type, SubjectAccountId, "ASTHENIA", "导入初始化金额-预缴学费抵扣学费", (bri.Money * -1), CreateDate, bri.RefID, PaymentRefId);
                                                        DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                                    }
                                                    break;
                                            }

                                        }

                                    }

                                }


                                // Update balance money

                                Sql = string.Format(InitObject.GetScriptServiceInstance().BalanceUpdate, PreTuitionInfo.PreTuitionMoney, BillInfo.SumMoney, StudentId);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                // Update bill status

                                Sql = string.Format(InitObject.GetScriptServiceInstance().BillUpdate, BillInfo.BillId);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                // Update bill ref status

                                Sql = string.Format(InitObject.GetScriptServiceInstance().BillRefUpdate, BillInfo.BillId);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                // Create student balance account log

                                Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceLogInsert, "TRADETYPE01", StudentId, BillInfo.SumMoney, CreateDate, InitObject.GetPaymentType(BznsBase.PaymentType), "N", StudentBalanceInfo.BalanceId, BillInfo.BillId, "导入初始化金额");
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                if (PreTuitionInfo.IsLog)
                                {
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceLogInsert, "TRADETYPE02", StudentId, PreTuitionInfo.PreTuitionMoney, CreateDate, InitObject.GetPaymentType(BznsBase.PaymentType), "N", StudentBalanceInfo.BalanceId, BillInfo.BillId, "预缴学费抵扣账单");
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                }

                                // Update fill plan status for payment done.

                                try
                                {
                                    if (!BznsBase.IsAppend)
                                    {
                                        BillPlanPaymentStatusUpdate(StudentId, trans);
                                    }
                                }
                                catch (Exception)
                                {

                                    
                                }


                            }
                        }
                        catch (Exception e)
                        {
                            trans.Rollback();
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " create charge error.", e.Message.ToString()), SysLogType.ERROR, string.Concat(ModuleName)));
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
                catch (Exception e)
                {
                    trans.Rollback();
                }
                finally
                {

                }

            }

            return result;

        }

        private PreTuitionInfo GetPreTuitionInfo(List<BillRefInfo> BillRefs)
        {
            var Info = new PreTuitionInfo();
            var IsIncludeTuitionInfo = false;
            var i = 0;
            var HasPreTuition = false;
            var HasTuition = false;
            try
            {
                foreach (BillRefInfo bri in BillRefs)
                {
                    if (bri.Type == "PAYMENTSUBJECT01")
                    {
                        IsIncludeTuitionInfo = true;
                    }
                }
                if (!IsIncludeTuitionInfo)
                {
                    foreach (BillRefInfo bri in BillRefs)
                    {
                        if (bri.Type == "PAYMENTSUBJECT05")
                        {
                            Info.IsDeductionPreTuition = false;
                            Info.PreTuitionMoney = bri.Money;
                            Info.Index = i;
                        }
                        i++;
                    }
                }

                foreach (BillRefInfo b in BillRefs)
                {
                    if (b.Type == "PAYMENTSUBJECT05")
                    {
                        HasPreTuition = true;
                    }
                    if (b.Type == "PAYMENTSUBJECT01")
                    {
                        HasTuition = true;
                    }
                }

                if (HasPreTuition && HasTuition)
                {
                    Info.IsLog = true;
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
            return Info;
        }

        private void BillPlanPaymentStatusUpdate(string StudentId, MySqlTransaction trans)
        {

            var Sql = string.Empty;
            var dt = new DataTable();

            try
            {
                Sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentPaymentPlanListByID, StudentId);
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    var StudentPaymentID = dt.Rows[0]["student_payment_id"].ToString();
                    Sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentPaymentPlanRefListByStudentID, StudentPaymentID);
                    dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        string sSql = string.Format(InitObject.GetScriptServiceInstance().BillPlanUpdate, row["id"].ToString());
                        DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, sSql, null);
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

        }

        public bool InsertForShanghai(bool IsTry2Import, DataTable Students, int StudentNoPos, int YearPos, int StartDatePos, int PTMoneyPos, int PTRateIDPos, int TMoneyPos, int TRateIDPos, int TStartDatePos, List<MoneyInfo> tmi, int MMoneyPos, int MRateIDPos, int MStartDatePos, List<MoneyInfo> mmi, int SBMoneyPos, int SBRateIDPos, int SBStartDatePos, List<MoneyInfo> sbmi, List<OnceInfo> omi, int TMonthPos, int MMonthPos, int SBMonthPos)
        {

            var result = false;
            var CreateDate = InitObject.GetSysDate();
            var Sql = string.Empty;
            var DescAlias = string.Empty;
            var DescMonthlyPrice = new decimal(0);
            var gtm = new decimal(0);
            var gmm = new decimal(0);
            var gsbm = new decimal(0);


            using (MySqlConnection conn = new MySqlConnection(BznsBase.GetConnectionString))
            {
                MySqlTransaction trans = null;
                conn.Open();
                trans = conn.BeginTransaction();

                try
                {

                    foreach (DataRow dr in Students.Rows)
                    {

                        var DescBuilder = new StringBuilder().Clear();
                        var StudentNo = StudentNoPos == -1 ? string.Empty : dr[StudentNoPos].ToString();
                        WriteLog(StudentNo);
                        var ClassId = string.Empty;
                        var StudentId = string.Empty;
                        var dt = new DataTable();
                        var myClassInfo = new ClassInfo();
                        var StudentPaymentId = InitObject.GetUUID();
                        var PaymentResult = 0;
                        var BillResult = 0;
                        var BillID = string.Empty;
                        var BillMonth = DateTime.Now.ToString("yyyy-MM");
                        var YearID = dr[YearPos].ToString();
                        List<string> ofm = new List<string>();

                        // Get Student Info

                        Sql = string.Format(InitObject.GetScriptServiceInstance().GetClassIDByStudentID, StudentNo);
                        dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            ClassId = dt.Rows[0]["class_id"].ToString();
                            StudentId = dt.Rows[0]["stu_id"].ToString();
                        }

                        // Get Class Info

                        Sql = string.Format(InitObject.GetScriptServiceInstance().GetClassListById, ClassId);
                        dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                        if (dt.Rows.Count == 1)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                myClassInfo.ClassId = row["class_id"].ToString();
                                myClassInfo.ClassName = row["class_nm"].ToString();
                                myClassInfo.CalendarId = row["school_calendar_id"].ToString();
                                myClassInfo.ClassType = row["class_type"].ToString();
                                myClassInfo.YearId = row["acad_year_id"].ToString();
                            }
                        }

                        // Get Monthly Info

                        var BillStartDate = Convert.ToDateTime(dr[StartDatePos]);
                        var MonthDict = new Dictionary<string, MonthInfo>();
                        MonthDict = InitObject.SplitMonth(BillStartDate, Convert.ToInt32(dr[TMonthPos]), myClassInfo.CalendarId, YearID);

                        // Insert Into Student Payment Plan

                        if (!string.IsNullOrEmpty(StudentId))
                        {
                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPayment, StudentPaymentId, StudentId, ClassId, BznsBase.KindergartenId, myClassInfo.ClassType, "Y", BznsBase.UserID, CreateDate);
                            PaymentResult = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, "not found the student no."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Student Payment")));
                        }

                        // Insert into fin bill

                        if (PaymentResult > 0)
                        {

                            BillID = InitObject.GetSysDate(true, true);
                            var BillStatus = "PAYMENTSTATE01";
                            var BillDateStart = MonthDict["summary"].StartDate.ToString("yyyy-MM-dd");
                            var BillDateEnd = MonthDict["summary"].EndDate.ToString("yyyy-MM-dd");

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBill, BillID, StudentId, YearID, StudentPaymentId, myClassInfo.ClassId, BillStatus, BillMonth, "N", "Y", CreateDate, BillDateStart, BillDateEnd);
                            BillResult = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, "not found the student no, start date or month."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Plan")));
                        }


                        #region per tuition

                        // Insert Per Tuition Fee

                        var BatchNo = InitObject.GetSysDate(true);
                        var PerTuitionFeeMoney = PTMoneyPos == -1 ? string.Empty : dr[PTMoneyPos].ToString();
                        var PerRateID = PTRateIDPos == -1 ? string.Empty : dr[PTRateIDPos].ToString();
                        var PerDescAlias = "标准预缴学费";
                        var PerDescMonthlyPrice = new decimal(0);
                        var PerTuitionFeeSubject = "PAYMENTSUBJECT05";
                        var PerDetailID = InitObject.GetUUID();

                        if (!string.IsNullOrEmpty(PerTuitionFeeMoney) && Convert.ToDecimal(PerTuitionFeeMoney) > 0 && PaymentResult > 0 && BillResult > 0)
                        {

                            PerDescMonthlyPrice = PTMoneyPos == -1 ? 0 : Convert.ToDecimal(dr[PTMoneyPos]);


                            if (!string.IsNullOrEmpty(PerRateID) && PaymentResult > 0)
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, PerDetailID, PerRateID, BatchNo, StudentPaymentId, "", PerTuitionFeeSubject, Convert.ToDecimal(PerTuitionFeeMoney), "", "", 0, 0, 1, BznsBase.UserID, CreateDate, CreateDate, 0);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                DescBuilder.Append(string.Format("【预缴学费: {0} ，标准:{1} 】", PerDescMonthlyPrice, PerDescAlias));

                                // Insert into bill ref.
                                var BillRefId = InitObject.GetUUID();
                                var BillNo = InitObject.GetBillNo(CreateDate, PerTuitionFeeSubject);
                                var BillStatus = "PAYMENTSTATE01";


                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRefNoInit, BillRefId, BillID, PerTuitionFeeSubject, BillNo, PerDetailID, BillMonth, BillStatus, CreateDate, Convert.ToDecimal(PerTuitionFeeMoney));
                                if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                {
                                    SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                                }

                                // Insert into bill Plan.
                                var BillPlanID = InitObject.GetUUID();

                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillNoLoop, BillPlanID, BillNo, PerTuitionFeeSubject, PerDetailID, 1, "N", "Y", "N", CreateDate, 0, PerTuitionFeeMoney, PerTuitionFeeMoney);

                                if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                {
                                    SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                                }

                            }
                            else
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, "not find rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                            }



                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set per tuition or fee <= 0."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                        }

                        #endregion

                        #region tuition

                        // Insert Into PaymentPlanRef - Tuition Fee 

                        var PaymentMethod = "PAYMENTMODE04";
                        var TuitionFeeMoney = TMoneyPos == -1 ? string.Empty : dr[TMoneyPos].ToString();
                        var StartDate = "";

                        try
                        {
                            StartDate = StartDatePos == -1 ? string.Empty : Convert.ToDateTime(dr[TStartDatePos]).ToString("yyyy-MM-dd");
                        }
                        catch (Exception)
                        {

                        }


                        var TuitionFeeSubject = "PAYMENTSUBJECT01";
                        var RateId = TRateIDPos == -1 ? string.Empty : dr[TRateIDPos].ToString(); ;
                        var DetailID = InitObject.GetUUID();
                        var BillMoney = new decimal(0);

                        // Get RateId
                        if (!string.IsNullOrEmpty(TuitionFeeMoney) && Convert.ToDecimal(TuitionFeeMoney) > 0)
                        {

                            DescAlias = "标准学费";
                            DescMonthlyPrice = Convert.ToDecimal(TuitionFeeMoney);
                        }


                        // Insert into Tuition

                        if (!string.IsNullOrEmpty(TuitionFeeMoney) && Convert.ToDecimal(TuitionFeeMoney) > 0 && !string.IsNullOrEmpty(RateId) && PaymentResult > 0 && BillResult > 0)
                        {

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, DetailID, RateId, BatchNo, StudentPaymentId, PaymentMethod, TuitionFeeSubject, Convert.ToDecimal(TuitionFeeMoney), "", "", 0, 0, 1, BznsBase.UserID, CreateDate, StartDate, 0);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            DescBuilder.Append(string.Format("【学费: {0}/月 ，缴费方式:{1}，标准:{2} 】", DescMonthlyPrice, "月", DescAlias));

                            foreach (MoneyInfo mi in tmi)
                            {
                                if (!string.IsNullOrEmpty(dr[mi.MoneyPos].ToString()))
                                {
                                    BillMoney += Convert.ToDecimal(dr[mi.MoneyPos]);
                                }
                            }


                            // Insert into bill ref
                            var BillRefId = InitObject.GetUUID();
                            var BillNo = InitObject.GetBillNo(StartDate, TuitionFeeSubject);
                            var BillStatus = "PAYMENTSTATE01";

                            var tbStartDate = Convert.ToDateTime(dr[StartDatePos]);
                            var tbMonthDict = new Dictionary<string, MonthInfo>();
                            MonthDict = InitObject.SplitMonth(Convert.ToDateTime(dr[TStartDatePos]), Convert.ToInt32(dr[TMonthPos]), myClassInfo.CalendarId, YearID);

                            var tbsd = MonthDict["summary"].StartDate.ToString("yyyy-MM-dd");
                            var tbed = MonthDict["summary"].EndDate.ToString("yyyy-MM-dd");

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRef, BillRefId, BillID, TuitionFeeSubject, BillNo, DetailID, BillMonth, BillStatus, CreateDate, tbsd, tbed, BillMoney);
                            if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Ref Tuition Fee")));
                            }

                            // Insert into bill plan.


                            var i = 0;
                            var IsFirstMonth = true;
                            foreach (KeyValuePair<string, MonthInfo> kv in MonthDict)
                            {

                                if (!kv.Key.Equals("summary"))
                                {
                                    var key = kv.Key;
                                    var val = kv.Value;
                                    var BillPlanID = InitObject.GetUUID();
                                    var FirstMonth = "N";
                                    var Money = dr[tmi[i].MoneyPos];

                                    if (IsFirstMonth)
                                    {
                                        FirstMonth = "Y";
                                    }


                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, TuitionFeeSubject, DetailID, 1, FirstMonth, "Y", "N", CreateDate, val.StudyDays, Convert.ToDecimal(Money), 0, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));

                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Plan Tuition Fee")));
                                    }
                                    IsFirstMonth = false;
                                    i++;
                                }

                            }



                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set the tuition or fee <= 0 or not find rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Tuition Fee")));
                        }

                        #endregion

                        #region meals

                        // Insert Into PaymentPlanRef - Meals 

                        var MealsMoney = MMoneyPos == -1 ? string.Empty : dr[MMoneyPos].ToString();
                        var mStartDate = "";
                        try
                        {
                            mStartDate = MStartDatePos == -1 ? string.Empty : Convert.ToDateTime(dr[MStartDatePos]).ToString("yyyy-MM-dd");
                        }
                        catch (Exception)
                        {

                        }


                        var MealsSubject = "PAYMENTSUBJECT02";
                        var mRateId = MRateIDPos == -1 ? string.Empty : dr[MRateIDPos].ToString();
                        var mDetailID = InitObject.GetUUID();
                        var mBillMoney = new decimal(0);

                        // Get RateId
                        if (!string.IsNullOrEmpty(MealsMoney) && Convert.ToDecimal(MealsMoney) > 0)
                        {

                            DescAlias = "标准餐费";
                            DescMonthlyPrice = Convert.ToDecimal(MealsMoney);
                        }



                        if (!string.IsNullOrEmpty(MealsMoney) && Convert.ToDecimal(MealsMoney) > 0 && !string.IsNullOrEmpty(mRateId) && PaymentResult > 0 && BillResult > 0)
                        {

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, mDetailID, mRateId, BatchNo, StudentPaymentId, PaymentMethod, MealsSubject, Convert.ToDecimal(MealsMoney), "", "", 0, 0, 1, BznsBase.UserID, CreateDate, mStartDate, 0);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            DescBuilder.Append(string.Format("【餐费: {0}/月 ，缴费方式:{1}，标准:{2} 】", DescMonthlyPrice, "月", DescAlias));

                            foreach (MoneyInfo mi in mmi)
                            {
                                if (!string.IsNullOrEmpty(dr[mi.MoneyPos].ToString())) mBillMoney += Convert.ToDecimal(dr[mi.MoneyPos]);
                            }

                            // Insert into bill ref
                            var BillRefId = InitObject.GetUUID();
                            var BillNo = InitObject.GetBillNo(mStartDate, MealsSubject);
                            var BillStatus = "PAYMENTSTATE01";

                            var tbStartDate = Convert.ToDateTime(dr[MStartDatePos]);
                            var tbMonthDict = new Dictionary<string, MonthInfo>();
                            MonthDict = InitObject.SplitMonth(Convert.ToDateTime(dr[MStartDatePos]), Convert.ToInt32(dr[MMonthPos]), myClassInfo.CalendarId, YearID);

                            var tbsd = MonthDict["summary"].StartDate.ToString("yyyy-MM-dd");
                            var tbed = MonthDict["summary"].EndDate.ToString("yyyy-MM-dd");

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRef, BillRefId, BillID, MealsSubject, BillNo, mDetailID, BillMonth, BillStatus, CreateDate, tbsd, tbed, mBillMoney);
                            if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Ref Meals Fee")));
                            }

                            // Insert into bill plan.


                            var i = 0;
                            var IsFirstMonth = true;
                            foreach (KeyValuePair<string, MonthInfo> kv in MonthDict)
                            {

                                if (!kv.Key.Equals("summary"))
                                {
                                    var key = kv.Key;
                                    var val = kv.Value;
                                    var BillPlanID = InitObject.GetUUID();
                                    var FirstMonth = "N";
                                    var Money = dr[mmi[i].MoneyPos];

                                    if (IsFirstMonth)
                                    {
                                        FirstMonth = "Y";
                                    }


                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, MealsSubject, mDetailID, 1, FirstMonth, "Y", "N", CreateDate, val.StudyDays, Convert.ToDecimal(Money), 0, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));

                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Plan Meals Fee")));
                                    }
                                    IsFirstMonth = false;
                                    i++;
                                }
                            }

                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set the tuition or fee <= 0 or not find rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Meals Fee")));
                        }

                        #endregion

                        #region school bus

                        // Insert Into PaymentPlanRef - Meals 

                        var sbMoney = SBMoneyPos == -1 ? string.Empty : dr[SBMoneyPos].ToString();
                        var sbStartDate = "";

                        try
                        {
                            sbStartDate = SBStartDatePos == -1 ? string.Empty : Convert.ToDateTime(dr[SBStartDatePos]).ToString("yyyy-MM-dd");
                        }
                        catch (Exception)
                        {
                        }


                        var sbSubject = "PAYMENTSUBJECT04";
                        var sbRateId = SBRateIDPos == -1 ? string.Empty : dr[SBRateIDPos].ToString();
                        var sbDetailID = InitObject.GetUUID();
                        var sbBillMoney = new decimal(0);

                        // Get RateId
                        if (!string.IsNullOrEmpty(sbMoney) && Convert.ToDecimal(sbMoney) > 0)
                        {

                            DescAlias = "标准校车费";
                            DescMonthlyPrice = Convert.ToDecimal(sbMoney);
                        }



                        if (!string.IsNullOrEmpty(sbMoney) && Convert.ToDecimal(sbMoney) > 0 && !string.IsNullOrEmpty(sbRateId) && PaymentResult > 0 && BillResult > 0)
                        {

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, sbDetailID, sbRateId, BatchNo, StudentPaymentId, PaymentMethod, sbSubject, Convert.ToDecimal(sbMoney), "", "", 0, 0, 1, BznsBase.UserID, CreateDate, sbStartDate, 0);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            DescBuilder.Append(string.Format("【校车费: {0}/月 ，缴费方式:{1}，标准:{2} 】", DescMonthlyPrice, "月", DescAlias));

                            foreach (MoneyInfo mi in sbmi)
                            {
                                if (!string.IsNullOrEmpty(dr[mi.MoneyPos].ToString())) sbBillMoney += Convert.ToDecimal(dr[mi.MoneyPos]);
                            }

                            // Insert into bill ref
                            var BillRefId = InitObject.GetUUID();
                            var BillNo = InitObject.GetBillNo(sbStartDate, sbSubject);
                            var BillStatus = "PAYMENTSTATE01";

                            var tbStartDate = Convert.ToDateTime(dr[SBStartDatePos]);
                            var tbMonthDict = new Dictionary<string, MonthInfo>();
                            MonthDict = InitObject.SplitMonth(Convert.ToDateTime(dr[SBStartDatePos]), Convert.ToInt32(dr[SBMonthPos]), myClassInfo.CalendarId, YearID);

                            var tbsd = MonthDict["summary"].StartDate.ToString("yyyy-MM-dd");
                            var tbed = MonthDict["summary"].EndDate.ToString("yyyy-MM-dd");

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRef, BillRefId, BillID, sbSubject, BillNo, sbDetailID, BillMonth, BillStatus, CreateDate, tbsd, tbed, sbBillMoney);
                            if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Ref School bus Fee")));
                            }

                            // Insert into bill plan.


                            var i = 0;
                            var IsFirstMonth = true;
                            foreach (KeyValuePair<string, MonthInfo> kv in MonthDict)
                            {

                                if (!kv.Key.Equals("summary"))
                                {
                                    var key = kv.Key;
                                    var val = kv.Value;
                                    var BillPlanID = InitObject.GetUUID();
                                    var FirstMonth = "N";
                                    var Money = dr[sbmi[i].MoneyPos];

                                    if (IsFirstMonth)
                                    {
                                        FirstMonth = "Y";
                                    }


                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, sbSubject, sbDetailID, 1, FirstMonth, "Y", "N", CreateDate, val.StudyDays, Convert.ToDecimal(Money), 0, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));

                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Bill Plan School bus Fee")));
                                    }
                                    IsFirstMonth = false;
                                    i++;
                                }
                            }

                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set the tuition or fee <= 0 or not find rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import School Bus")));
                        }

                        #endregion

                        #region other

                        // Insert Once Fee
                        foreach (var info in omi)
                        {


                            var ofMoney = int.Parse(info.Key.ToString()) == -1 ? string.Empty : dr[int.Parse(info.Key.ToString())].ToString();
                            var ofRateID = string.Empty;
                            var ofDescAlias = string.Empty;
                            var ofDescMonthlyPrice = new decimal(0);
                            var ofSubject = "PAYMENTSUBJECT03";
                            var ofDetailID = InitObject.GetUUID();

                            if (!string.IsNullOrEmpty(ofMoney) && Convert.ToDecimal(ofMoney) > 0)
                            {



                                //Sql = string.Format(InitObject.GetScriptServiceInstance().GetRateIdByKMS, Convert.ToDecimal(ofMoney), ofSubject, KindergartenId, myYearInfo.YearID);
                                //dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetRateIdByAliasName, ofSubject, BznsBase.KindergartenId, YearID, info.Value);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                if (dt.Rows.Count >= 1)
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        ofRateID = row["rates_id"].ToString();
                                        ofDescAlias = row["rates_alias"].ToString();
                                        ofDescMonthlyPrice = Convert.ToDecimal(row["rates_monthly"]);
                                        ofm.Add(ofMoney);
                                    }
                                }

                                if (!string.IsNullOrEmpty(ofRateID) && PaymentResult > 0 && BillResult > 0)
                                {
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, ofDetailID, ofRateID, BatchNo, StudentPaymentId, "", ofSubject, Convert.ToDecimal(ofMoney), "", "", 0, 0, 1, BznsBase.UserID, CreateDate, CreateDate, 0);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                    DescBuilder.Append(string.Format("【{0}: {1} ，标准:{1} 】", ofDescAlias, ofDescMonthlyPrice, ofDescAlias));

                                    // Insert into bill ref.
                                    var BillRefId = InitObject.GetUUID();
                                    var BillNo = InitObject.GetBillNo(CreateDate, ofSubject);
                                    var BillStatus = "PAYMENTSTATE01";

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRefNoInit, BillRefId, BillID, ofSubject, BillNo, ofDetailID, BillMonth, BillStatus, CreateDate, Convert.ToDecimal(ofMoney));
                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                                    }

                                    // Insert into bill plan.
                                    var BillPlanID = InitObject.GetUUID();

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillNoLoop, BillPlanID, BillNo, ofSubject, ofDetailID, 1, "N", "Y", "N", CreateDate, 0, Convert.ToDecimal(ofMoney), Convert.ToDecimal(ofMoney));

                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill plan insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                                    }

                                }
                                else
                                {
                                    SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont find once fee rate."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                                }



                            }
                            else
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set once fee or <= 0."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                            }
                            // End Insert Once Fee

                        }

                        #endregion

                        // update payment plan
                        Sql = string.Format(InitObject.GetScriptServiceInstance().UpdateStudentPayment, DescBuilder.ToString(), StudentPaymentId);
                        DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                        var sum = TotalMoney(BillMoney, mBillMoney, sbBillMoney, ofm, PerTuitionFeeMoney);
                        Sql = string.Format(InitObject.GetScriptServiceInstance().UpdateFinBill, sum, 0, BillID);
                        DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

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
                catch (Exception e)
                {
                    trans.Rollback();
                }
                finally
                {

                }

            }

            return result;
        }

        private decimal TotalMoney(decimal tm, decimal mm, decimal sbm, List<string> ofMoneys, string ptMoney)
        {
            var sum = new decimal(0);
            try
            {
                if (!string.IsNullOrEmpty(ptMoney))
                {
                    sum += Convert.ToDecimal(ptMoney);
                }

                sum += tm;
                sum += mm;
                sum += sbm;

                foreach (var m in ofMoneys)
                {
                    sum = sum + Convert.ToDecimal(m);
                }

            }
            catch (Exception)
            {

                throw;
            }
            return sum;
        }

        private StudentPaymentPlanInfo GetSPPByID(string sid)
        {
            var result = new StudentPaymentPlanInfo();
            var dt = new DataTable();
            var sql = string.Empty;
            try
            {
                sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentPyamentPlanListByStudentID, sid);
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    result.ID = dt.Rows[0]["student_payment_id"].ToString();
                    result.StudentID = dt.Rows[0]["student_id"].ToString();
                    result.Intro = dt.Rows[0]["student_payment_intro"].ToString();
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
            return result;
        }

        private AccountInfo GetAccountInfo(string sid)
        {
            var result = new AccountInfo();
            var sql = string.Empty;
            var dt = new DataTable();
            try
            {

                sql = string.Format(InitObject.GetScriptServiceInstance().GetBalanceAccountByStudentIDAndType, sid, "PAYMENTSUBJECT01");
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    result.HasTuitionAccount = true;
                    result.TuitionAccountID = dt.Rows[0]["account_id"].ToString();
                }
                else {
                    result.HasTuitionAccount = false;
                    result.TuitionAccountID = string.Empty;
                }

                sql = string.Format(InitObject.GetScriptServiceInstance().GetBalanceAccountByStudentIDAndType, sid, "PAYMENTSUBJECT02");
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    result.HasMealsAccount = true;
                    result.MealsAccountID = dt.Rows[0]["account_id"].ToString();
                }
                else {
                    result.HasMealsAccount = false;
                    result.MealsAccountID = string.Empty;
                }

                sql = string.Format(InitObject.GetScriptServiceInstance().GetBalanceAccountByStudentIDAndType, sid, "PAYMENTSUBJECT03");
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    result.HasOnetimeAccount = true;
                    result.OnetimeAccountID = dt.Rows[0]["account_id"].ToString();
                }
                else {
                    result.HasOnetimeAccount = false;
                    result.OnetimeAccountID = string.Empty;
                }

                sql = string.Format(InitObject.GetScriptServiceInstance().GetBalanceAccountByStudentIDAndType, sid, "PAYMENTSUBJECT04");
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    result.HasSchoolBusAccount = true;
                    result.SchoolBusAccountID = dt.Rows[0]["account_id"].ToString();
                }
                else {
                    result.HasSchoolBusAccount = false;
                    result.SchoolBusAccountID = string.Empty;
                }

                sql = string.Format(InitObject.GetScriptServiceInstance().GetBalanceAccountByStudentIDAndType, sid, "PAYMENTSUBJECT05");
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    result.HasPrepaidAccount = true;
                    result.PrepaidAccountID = dt.Rows[0]["account_id"].ToString();
                }
                else {
                    result.HasPrepaidAccount = false;
                    result.PrepaidAccountID = string.Empty;
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
            return result;

        }

    }


}
