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
using System.IO;

namespace KMISHelper.Business
{
    class BznsFinance
    {

        public readonly string ModuleName = "Finance";

        public bool BillInsert(DataTable Students, int StudentNoPos, int YearTitlePos, int MonthPos, int BillStartDatePos, int PaymentMethodPos, int TuitionFeeMoneyPos, int DiscountMoneyPos, int DiscountNamePos, int StartDatePos, int PerTuitionFeeMoneyPos, int mfMethodPos, int mfMoneyPos, int mfStartDatePos, int scMethodPos, int scMoneyPos, int scMonthPos, int scStartDatePos, List<OnceInfo> ofs, bool IsTry2Import)
        {

            var Result = false;
            var KindergartenId = BznsBase.KindergartenId;
            var UserID = BznsBase.UserID;
            var Sql = string.Empty;
            var CreateDate = InitObject.GetSysDate();
            var DescAlias = string.Empty;
            var DescMonthlyPrice = new decimal(0);
            var sno = string.Empty;
            
            var StudentPaymentPlanInfo = new StudentPaymentPlanInfo();
            var SAInfo = new ExistAccountInfo();

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
                        var IsAppend = false;

                        // Get StudentID and ClassID 

                        Write(StudentNo, " start handle this student no.",KindergartenId);

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

                        if (string.IsNullOrEmpty(StudentId))
                        {
                            WriteForManual(StudentNo, " not found in db.",KindergartenId);
                            continue;
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
                        var BillStartEndDate = BillStartDate.AddMonths(Convert.ToInt32(dr[MonthPos]));

                        var MonthDict = new Dictionary<string, MonthInfo>();
                        MonthDict = InitObject.SplitMonth(BillStartDate, Convert.ToInt32(dr[MonthPos]), myClassInfo.CalendarId, myYearInfo.YearID);


                        // Is exist student payment plan?

                        SAInfo = GetStudentAllAccount(StudentId);
                        StudentPaymentPlanInfo = GetStudentPaymentPlanInfoBySID(StudentId);

                        if (!string.IsNullOrEmpty(StudentPaymentPlanInfo.student_payment_id))
                        {
                            IsAppend = true;
                        }
                        else {
                            IsAppend = false;
                        }

                        // insert into student payment plan.

                        if (!string.IsNullOrEmpty(StudentId))
                        {
                            if (IsAppend)
                            {

                                PaymentResult = 1;
                                StudentPaymentId = StudentPaymentPlanInfo.student_payment_id;

                                // Is throw an error log?

                                var MoneyFromTuition = TuitionFeeMoneyPos == -1 ? string.Empty : dr[TuitionFeeMoneyPos].ToString();
                                var MoneyFromMeals = mfMoneyPos == -1 ? string.Empty : dr[mfMoneyPos].ToString();
                                var MoneyFromSchoolBus = scMoneyPos == -1 ? string.Empty : dr[scMoneyPos].ToString();
                                var RefInfo = new StudentPaymentPlanRefInfo();
                                var IsHaveTuitionPlan = false;
                                var IsHaveMealsPlan = false;
                                var IsHaveSchoolBusPlan = false;


                                if (!string.IsNullOrEmpty(MoneyFromTuition) && Convert.ToDecimal(MoneyFromTuition) > 0)
                                {

                                    RefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT01");

                                    if (!string.IsNullOrEmpty(RefInfo.id))
                                    {
                                        IsHaveTuitionPlan = IsExistBillPlan("PAYMENTSUBJECT01", BillStartDate.ToString("yyyy-MM-dd"), BillStartEndDate.ToString("yyyy-MM-dd"), RefInfo.id);
                                        if (IsHaveTuitionPlan)
                                        {
                                            WriteForManual(StudentNo, " PAYMENTSUBJECT01",KindergartenId);
                                            continue;
                                        }
                                    }

                                }

                                if (!string.IsNullOrEmpty(MoneyFromMeals) && Convert.ToDecimal(MoneyFromMeals) > 0)
                                {

                                    RefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT02");

                                    if (!string.IsNullOrEmpty(RefInfo.id))
                                    {
                                        IsHaveMealsPlan = IsExistBillPlan("PAYMENTSUBJECT02", BillStartDate.ToString("yyyy-MM-dd"), BillStartEndDate.ToString("yyyy-MM-dd"), RefInfo.id);
                                        if (IsHaveMealsPlan)
                                        {
                                            WriteForManual(StudentNo, " PAYMENTSUBJECT02",KindergartenId);
                                            continue;
                                        }
                                    }

                                }

                                if (!string.IsNullOrEmpty(MoneyFromSchoolBus) && Convert.ToDecimal(MoneyFromSchoolBus) > 0)
                                {

                                    RefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT04");

                                    if (!string.IsNullOrEmpty(RefInfo.id))
                                    {
                                        IsHaveSchoolBusPlan = IsExistBillPlan("PAYMENTSUBJECT04", BillStartDate.ToString("yyyy-MM-dd"), BillStartEndDate.ToString("yyyy-MM-dd"), RefInfo.id);
                                        if (IsHaveSchoolBusPlan)
                                        {
                                            WriteForManual(StudentNo, " PAYMENTSUBJECT04",KindergartenId);
                                            continue;
                                        }
                                    }

                                }

                            }
                            else
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPayment, StudentPaymentId, StudentId, ClassId, KindergartenId, myClassInfo.ClassType, "Y", UserID, CreateDate);
                                PaymentResult = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }

                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, "not found the student no."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Student Payment")));
                        }


                        if (PaymentResult > 0)
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


                        //--------------------------------------------------------------------------------------------------------------------------
                        // Insert Into PaymentPlanRef - Tuition Fee 
                        //--------------------------------------------------------------------------------------------------------------------------

                        #region "Insert Tuition"

                        var PaymentMethod = PaymentMethodPos == -1 ? string.Empty : InitObject.GetPaymentModeByName(dr[PaymentMethodPos].ToString());
                        var TuitionFeeMoney = TuitionFeeMoneyPos == -1 ? string.Empty : dr[TuitionFeeMoneyPos].ToString();
                        var DiscountMoney = DiscountMoneyPos == -1 ? string.Empty : dr[DiscountMoneyPos].ToString();
                        var DiscountName = DiscountNamePos == -1 ? string.Empty : dr[DiscountNamePos].ToString();
                        var StartDate = StartDatePos == -1 ? string.Empty : Convert.ToDateTime(dr[StartDatePos]).ToString("yyyy-MM-dd");
                        var TuitionFeeSubject = "PAYMENTSUBJECT01";
                        var RateId = string.Empty;
                        var DiscountID = string.Empty;
                        var DiscountType = string.Empty;
                        var DetailID = InitObject.GetUUID();
                        var BatchNo = InitObject.GetSysDate(true);
                        var DiscountedMoney = new decimal(0);
                        var IsExistTuition = false;
                        var TuitionRefInfo = new StudentPaymentPlanRefInfo();


                        // Is exist ref

                        TuitionRefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT01");

                        if (!string.IsNullOrEmpty(TuitionRefInfo.id))
                        {
                            IsExistTuition = true;
                            DetailID = TuitionRefInfo.id;
                        }

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


                            if (!IsExistTuition && dt.Rows.Count <= 0)
                            {

                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetRandRateIdByKMS, TuitionFeeSubject, KindergartenId, myYearInfo.YearID);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];


                                if (dt.Rows.Count <= 0)
                                {
                                    if (!string.IsNullOrEmpty(DiscountName.Trim()))
                                    {
                                        WriteForManual(StudentNo, string.Format(" not found Tuition rates subject:'{0}',name:'{1}'", TuitionFeeSubject, DiscountName),KindergartenId);
                                    }
                                    else
                                    {
                                        WriteForManual(StudentNo, string.Format(" not found Tuition rates subject:'{0}',money:'{1}'", TuitionFeeSubject, (Convert.ToDecimal(TuitionFeeMoney) - Convert.ToDecimal(DiscountMoney))),KindergartenId);
                                    }
                                }
                                else
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        RateId = row["rates_id"].ToString();
                                        DescAlias = row["rates_alias"].ToString();
                                        DescMonthlyPrice = Convert.ToDecimal(row["rates_monthly"]);
                                    }

                                }

                            }


                            if (!string.IsNullOrEmpty(TuitionRefInfo.id))
                            {
                                var RatesInfo = GetRatesInfoByID(TuitionRefInfo.rates_id);
                                RateId = RatesInfo.rates_id;
                                DescAlias = RatesInfo.rates_alias;
                                DescMonthlyPrice = RatesInfo.rates_monthly;
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

                            if (!string.IsNullOrEmpty(TuitionRefInfo.id))
                            {
                                DiscountID = TuitionRefInfo.pay_discount_id;
                                DiscountType = TuitionRefInfo.payment_discount;
                            }

                            DiscountedMoney = Convert.ToDecimal(TuitionFeeMoney) - Convert.ToDecimal(DiscountMoney);

                        }


                        // Insert into Tuition

                        if (!string.IsNullOrEmpty(TuitionFeeMoney) && Convert.ToDecimal(TuitionFeeMoney) > 0 && !string.IsNullOrEmpty(RateId) && PaymentResult > 0 && BillResult > 0)
                        {


                            if (!IsExistTuition)
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, DetailID, RateId, BatchNo, StudentPaymentId, PaymentMethod, TuitionFeeSubject, Convert.ToDecimal(TuitionFeeMoney), DiscountID, DiscountType, DiscountMoney, 0, 1, UserID, CreateDate, StartDate, DiscountedMoney);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                DescBuilder.Append(string.Format("【学费: {0}/月 ，缴费方式:{1}，标准:{2} 】", DescMonthlyPrice, dr[PaymentMethodPos].ToString(), DescAlias));
                            }


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


                        #endregion


                        //--------------------------------------------------------------------------------------------------------
                        // Insert Per Tuition Fee
                        //--------------------------------------------------------------------------------------------------------


                        #region "Insert Per Tuition"

                        var PerTuitionFeeMoney = PerTuitionFeeMoneyPos == -1 ? string.Empty : dr[PerTuitionFeeMoneyPos].ToString();
                        var PerTuitionFeeSubject = "PAYMENTSUBJECT05";

                        if (!string.IsNullOrEmpty(PerTuitionFeeMoney) && Convert.ToDecimal(PerTuitionFeeMoney) > 0 && PaymentResult > 0 && BillResult > 0)
                        {

                            var BillRefId = InitObject.GetUUID();
                            var BillNo = InitObject.GetBillNo(CreateDate, PerTuitionFeeSubject);
                            var BillStatus = "PAYMENTSTATE01";

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRefNoInit, BillRefId, BillID, PerTuitionFeeSubject, BillNo, string.Empty, BillMonth, BillStatus, CreateDate, Convert.ToDecimal(PerTuitionFeeMoney));

                            if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                            }

                        }
                        else
                        {
                            SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set per tuition or fee <= 0."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Per Tuition Fee")));
                        }


                        #endregion


                        //--------------------------------------------------------------------------------------------------
                        // Insert Meals Fee
                        //--------------------------------------------------------------------------------------------------


                        #region "Meals"

                        var mfMethod = mfMethodPos == -1 ? string.Empty : InitObject.GetPaymentModeByName(dr[mfMethodPos].ToString());
                        var mfMoney = mfMoneyPos == -1 ? string.Empty : dr[mfMoneyPos].ToString();
                        var mfRateID = string.Empty;
                        var mfDescAlias = string.Empty;
                        var mfDescMonthlyPrice = new decimal(0);
                        var mfSubject = "PAYMENTSUBJECT02";
                        var mfDetailID = InitObject.GetUUID();
                        var mfStartDate = mfStartDatePos == -1 ? string.Empty : Convert.ToDateTime(dr[mfStartDatePos]).ToString("yyyy-MM-dd");


                        var IsExistMeals = false;
                        var MealsRefInfo = new StudentPaymentPlanRefInfo();


                        // Is exist ref

                        MealsRefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT02");

                        if (!string.IsNullOrEmpty(MealsRefInfo.id))
                        {
                            IsExistMeals = true;
                            mfDetailID = MealsRefInfo.id;
                        }


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

                            if (!IsExistMeals && dt.Rows.Count <= 0)
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetRandRateIdByKMS, mfSubject, KindergartenId, myYearInfo.YearID);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                if (dt.Rows.Count <= 0)
                                {
                                    WriteForManual(StudentNo, string.Format(" not found Meals rates subject:'{0}',money:'{1}'", mfMoney, mfSubject),KindergartenId);
                                }
                                else
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        mfRateID = row["rates_id"].ToString();
                                        mfDescAlias = row["rates_alias"].ToString();
                                        mfDescMonthlyPrice = Convert.ToDecimal(row["rates_monthly"]);
                                    }
                                }
                                
                            }

                            if (!string.IsNullOrEmpty(MealsRefInfo.id))
                            {
                                var mfRatesInfo = GetRatesInfoByID(MealsRefInfo.rates_id);
                                mfRateID = mfRatesInfo.rates_id;
                                mfDescAlias = mfRatesInfo.rates_alias;
                                mfDescMonthlyPrice = mfRatesInfo.rates_monthly;
                            }

                            if (!string.IsNullOrEmpty(mfRateID) && PaymentResult > 0 && BillResult > 0)
                            {

                                if (!IsExistMeals)
                                {
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, mfDetailID, mfRateID, BatchNo, StudentPaymentId, mfMethod, mfSubject, Convert.ToDecimal(mfMoney), "", "", 0, 0, 1, UserID, CreateDate, mfStartDate, 0);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                    DescBuilder.Append(string.Format("【餐费: {0}/月 ，缴费方式:{1}，标准:{2} 】", mfDescMonthlyPrice, dr[mfMethodPos].ToString(), mfDescAlias));
                                }

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

                        #endregion


                        //-----------------------------------------------------------------------------------------------------
                        // Insert School Car Fee
                        //-----------------------------------------------------------------------------------------------------

                        #region "School Bus"

                        var scMethod = scMethodPos == -1 ? string.Empty : InitObject.GetPaymentModeByName(dr[scMethodPos].ToString());
                        var scMoney = scMoneyPos == -1 ? string.Empty : dr[scMoneyPos].ToString();
                        var scMonth = scMonthPos == -1 ? 0 : Convert.ToInt32(dr[scMonthPos]);
                        var scRateID = string.Empty;
                        var scDescAlias = string.Empty;
                        var scDescMonthlyPrice = new decimal(0);
                        var scSubject = "PAYMENTSUBJECT04";
                        var scDetailID = InitObject.GetUUID();
                        var scStartDate = scStartDatePos == -1 ? string.Empty : dr[scStartDatePos].ToString();
                        var IsExistSchoolBus = false;
                        var SchoolBusRefInfo = new StudentPaymentPlanRefInfo();


                        // Is exist ref

                        SchoolBusRefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT04");

                        if (!string.IsNullOrEmpty(SchoolBusRefInfo.id))
                        {
                            IsExistSchoolBus = true;
                            scDetailID = SchoolBusRefInfo.id;
                        }

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

                            if (!IsExistSchoolBus && dt.Rows.Count <= 0)
                            {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetRandRateIdByKMS, scSubject, KindergartenId, myYearInfo.YearID);
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                if (dt.Rows.Count <= 0)
                                {
                                    WriteForManual(StudentNo, string.Format(" not found school bus rates subject:'{0}',money:'{1}'", scMoney, scSubject),KindergartenId);
                                }
                                else {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        scRateID = row["rates_id"].ToString();
                                        scDescAlias = row["rates_alias"].ToString();
                                        scDescMonthlyPrice = Convert.ToDecimal(row["rates_monthly"]);
                                    }
                                }

                            }
                            

                            if (!string.IsNullOrEmpty(SchoolBusRefInfo.id))
                            {
                                var scRatesInfo = GetRatesInfoByID(SchoolBusRefInfo.rates_id);
                                scRateID = scRatesInfo.rates_id;
                                scDescAlias = scRatesInfo.rates_alias;
                                scDescMonthlyPrice = scRatesInfo.rates_monthly;
                            }

                            if (!string.IsNullOrEmpty(scRateID) && PaymentResult > 0 && BillResult > 0)
                            {


                                if (!IsExistSchoolBus)
                                {
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, scDetailID, scRateID, BatchNo, StudentPaymentId, scMethod, scSubject, Convert.ToDecimal(scMoney), "", "", 0, 0, 1, UserID, CreateDate, scStartDate, 0);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                    DescBuilder.Append(string.Format("【校车费: {0}/月 ，缴费方式:{1}，标准:{2} 】", scDescMonthlyPrice, dr[scMethodPos].ToString(), scDescAlias));
                                }

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


                        #endregion

                        //------------------------------------------------------------------------------------------------------
                        // Insert Once Fee
                        //------------------------------------------------------------------------------------------------------

                        #region "Insert once"

                        foreach (var info in ofs)
                        {

                            var ofMoney = int.Parse(info.Key.ToString()) == -1 ? string.Empty : dr[int.Parse(info.Key.ToString())].ToString();
                            var ofSubject = "PAYMENTSUBJECT03";

                            if (!string.IsNullOrEmpty(ofMoney) && Convert.ToDecimal(ofMoney) > 0)
                            {

                                if (PaymentResult > 0 && BillResult > 0)
                                {

                                    // Insert into bill ref.
                                    
                                    var BillRefId = InitObject.GetUUID();
                                    var BillNo = InitObject.GetBillNo(CreateDate, ofSubject);
                                    var BillStatus = "PAYMENTSTATE01";

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRefNoInit, BillRefId, BillID, ofSubject, BillNo, string.Empty, BillMonth, BillStatus, CreateDate, Convert.ToDecimal(ofMoney));
                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                                    }
                                    else {
                                        ofm.Add(ofMoney);
                                    }


                                }

                            }
                            else
                            {
                                SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " dont set once fee or <= 0."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                            }

                        }

                        #endregion



                        // update payment plan if don't exist student payment plan

                        if (!IsAppend)
                        {

                            Sql = string.Format(InitObject.GetScriptServiceInstance().UpdateStudentPayment, DescBuilder.ToString(), StudentPaymentId);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                        }

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

                    SysLog.Insert(new SysLogInfo(ex.Message.ToString() + sno, SysLogType.ERROR, string.Concat(ModuleName, " - ", "Import Payment Bzns")));
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
            var CreateDate = new DateTime(2019, 3, 30, 0, 0, 0).ToString("yyyy-MM-dd hh:mm:ss"); //InitObject.GetSysDate();
            var sno = string.Empty;

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
                        Write(StudentNo, " start handle this student no payment.", BznsBase.KindergartenId);
                        var BillInfo = new BillInfo();
                        var StudentBalanceInfo = new StudentBalanceInfo();
                        List<BillRefInfo> BillRefs = new List<BillRefInfo>();
                        var PreTuitionInfo = new PreTuitionInfo();
                        var ClassId = string.Empty;
                        var StudentId = string.Empty;
                        var Sql = string.Empty;
                        var dt = new DataTable();
                        var PreTuitionMoney = new decimal(0);
                        var IsHaveTuition = false;
                        var SBAInfo = new StudentBalanceAccountInfo();
                        var SAInfo = new ExistAccountInfo();
                        var IsHavePer = false;

                        try
                        {
                            if (!string.IsNullOrWhiteSpace(StudentNo))
                            {

                                // Get StudentID and ClassID

                                Sql = string.Format(InitObject.GetScriptServiceInstance().GetClassIDByStudentID, StudentNo.Trim());
                                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                                if (dt.Rows.Count > 0)
                                {
                                    ClassId = dt.Rows[0]["class_id"].ToString();
                                    StudentId = dt.Rows[0]["stu_id"].ToString();
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

                                // Get Student Account Exist Info

                                SAInfo = GetStudentAllAccount(StudentId);


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

                                        // -------------------------------------------------------------
                                        // 如果有预缴学费科目，预缴学费赋值给PreTuitionMoney
                                        if (row["bill_type"].ToString() == "PAYMENTSUBJECT05")
                                        {
                                            IsHavePer = true;
                                            PreTuitionMoney = Convert.ToDecimal(row["bill_money"]);
                                        }

                                        if (row["bill_type"].ToString() == "PAYMENTSUBJECT01")
                                        {
                                            IsHaveTuition = true;
                                        }
                                        // -------------------------------------------------------------
                                        BillRefs.Add(BillRefInfo);
                                    }
                                }

                                // Get Pre Info

                                PreTuitionInfo = GetPreTuitionInfo(BillRefs);


                                SBAInfo = GetStudentBalanceAccountInfo(StudentId);


                                if (IsHaveTuition && SBAInfo.AvlAmt > 0)
                                {
                                    PreTuitionInfo.IsLog = true;
                                    PreTuitionInfo.IsDeductionPreTuition = true;
                                    PreTuitionInfo.PreTuitionMoney = PreTuitionInfo.PreTuitionMoney + SBAInfo.AvlAmt;
                                    PreTuitionMoney = PreTuitionMoney + SBAInfo.AvlAmt;
                                }


                                if (!string.IsNullOrEmpty(BillInfo.BillId) && BillRefs.Count > 0)
                                {

                                    var PaymentId = InitObject.GetUUID();
                                    var PaymentWay = InitObject.GetPaymentType(BznsBase.PaymentType);
                                    var PaymentNo = InitObject.GetPaymentInfo().PaymentNo;
                                    var PaymentStatus = "PAYMENTSTATE02";
                                    var ReceiptNo = InitObject.GetPaymentInfo().ReceiptNo;


                                    // Create Student Payment

                                    var pyamentTotalMoney = BillInfo.SumMoney;

                                    if (IsHaveTuition && SBAInfo.AvlAmt > 0)
                                    {
                                        pyamentTotalMoney = BillInfo.SumMoney - SBAInfo.AvlAmt;
                                    }

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().PaymentInsert, PaymentId, BillInfo.BillId, pyamentTotalMoney, PaymentWay, CreateDate, PaymentNo, PaymentStatus, BznsBase.UserID, CreateDate, ReceiptNo);
                                    var IsPaymentCreated = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                    if (IsPaymentCreated > 0)
                                    {

                                        foreach (BillRefInfo bri in BillRefs)
                                        {

                                            // Create Payment Ref

                                            var PaymentRefId = InitObject.GetUUID();

                                            //insert into (tuition-pre) money if type is tuition and pre than 0 

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

                                            switch (bri.Type.Trim())
                                            {

                                                case "PAYMENTSUBJECT01":
                                                    if (SAInfo.IsExistTuitionAccount)
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountUpdate, bri.Money, 0, SAInfo.TuitionAccountId);
                                                    }
                                                    else
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, bri.Money, bri.Type, BznsBase.UserID, CreateDate, 0);
                                                    }
                                                    break;
                                                case "PAYMENTSUBJECT02":
                                                    if (SAInfo.IsExistMealsAccount)
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountUpdate, bri.Money, 0, SAInfo.MealesAccountId);
                                                    }
                                                    else
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, bri.Money, bri.Type, BznsBase.UserID, CreateDate, 0);
                                                    }
                                                    break;
                                                case "PAYMENTSUBJECT03":
                                                    if (SAInfo.IsExistOtherAccount)
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountUpdate, bri.Money, 0, SAInfo.OtherAccountId);
                                                    }
                                                    else
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, bri.Money, bri.Type, BznsBase.UserID, CreateDate, 0);
                                                        SAInfo.IsExistOtherAccount = true;
                                                        SAInfo.OtherAccountId = AccountID;
                                                    }
                                                    break;
                                                case "PAYMENTSUBJECT04":
                                                    if (SAInfo.IsExistSchoolBusAccount)
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountUpdate, bri.Money, 0, SAInfo.SchoolBusAccountId);
                                                    }
                                                    else
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, bri.Money, bri.Type, BznsBase.UserID, CreateDate, 0);
                                                    }
                                                    break;
                                                case "PAYMENTSUBJECT05":

                                                    var val = new decimal(0);

                                                    if (!PreTuitionInfo.IsDeductionPreTuition)
                                                    {
                                                        val = PreTuitionInfo.PreTuitionMoney;
                                                    }

                                                    if (SBAInfo.IsNone)
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountInsert, AccountID, StudentBalanceInfo.BalanceId, StudentId, val, bri.Type, BznsBase.UserID, CreateDate, val);
                                                    }
                                                    else
                                                    {
                                                        Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountUpdate, val, val, SAInfo.PreAccountId);
                                                    }

                                                    break;
                                            }


                                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);


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
                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().ItemAccountInsert, SubjectAccountId, AccountID, StudentId, BillInfo.YearId, bri.Money, 'Y', bri.Money, bri.RefID, BznsBase.UserID, CreateDate);
                                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                                    break;
                                                case "PAYMENTSUBJECT04":
                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().SchoolBusAccountInsert, SubjectAccountId, AccountID, StudentId, BillInfo.YearId, bri.Money, 'Y', bri.Money, bri.RefID, BznsBase.UserID, CreateDate);
                                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                                    break;
                                                case "PAYMENTSUBJECT05":

                                                    var val = new decimal(0);
                                                    var tbdMoney = new decimal(0);

                                                    if (!PreTuitionInfo.IsDeductionPreTuition)
                                                    {
                                                        val = PreTuitionInfo.PreTuitionMoney;
                                                        tbdMoney = bri.Money;
                                                    }

                                                    Sql = string.Format(InitObject.GetScriptServiceInstance().PrepaidTuitionAccountInsert, SubjectAccountId, AccountID, StudentId, BillInfo.YearId, val, 'Y', tbdMoney, bri.RefID, BznsBase.UserID, CreateDate);
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

                                // 本账单中没有预缴学费但有学费，抵消预缴

                                if (!IsHavePer && IsHaveTuition && SBAInfo.AvlAmt > 0)
                                {

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().StudentBalanceAccountUpdate, (SBAInfo.AvlAmt * -1), (SBAInfo.AvlAmt * -1), SAInfo.PreAccountId);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().UpdateStudentPrepaidTuitionAccount, 0, 0, 0, StudentId);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                                }

                                // Update balance money


                                var m = new decimal(0);

                                if (IsHaveTuition && SBAInfo.AvlAmt > 0)
                                {
                                    m = PreTuitionInfo.PreTuitionMoney;
                                }

                                Sql = string.Format(InitObject.GetScriptServiceInstance().BalanceUpdate, (BillInfo.SumMoney - m), (BillInfo.SumMoney - m), StudentId);
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
                                    BillPlanPaymentStatusUpdate(StudentId, trans);
                                }
                                catch (Exception)
                                {

                                    throw;
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
                    SysLog.Insert(new SysLogInfo(string.Concat(sno, " create charge error.", e.Message.ToString()), SysLogType.ERROR, string.Concat(ModuleName)));
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
            var IsIncludeTuition = false;
            var i = 0;
            var HasPreTuition = false;
            var HasTuition = false;
            try
            {

                // if have tuition
                foreach (BillRefInfo bri in BillRefs)
                {
                    if (bri.Type == "PAYMENTSUBJECT01")
                    {
                        IsIncludeTuition = true;
                    }
                }

                // if not have tuition not deduction pre tuition
                if (!IsIncludeTuition)
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

        public bool InsertForShanghai(bool IsTry2Import, DataTable Students, int StudentNoPos, int YearPos, int StartDatePos, int PTMoneyPos, int PTRateIDPos, int TMoneyPos, int TRateIDPos, int TStartDatePos, List<MoneyInfo> tmi, int MMoneyPos, int MRateIDPos, int MStartDatePos, List<MoneyInfo> mmi, int SBMoneyPos, int SBRateIDPos, int SBStartDatePos, List<MoneyInfo> sbmi, List<OnceInfo> omi, int TMonthPos, int MMonthPos, int SBMonthPos,int DiscountIdPos,int DiscountMoneyPos,int DiscountTypePos)
        {

            var result = false;
            var CreateDate = InitObject.GetSysDate();
            var Sql = string.Empty;
            var DescAlias = string.Empty;
            var DescMonthlyPrice = new decimal(0);
            var gtm = new decimal(0);
            var gmm = new decimal(0);
            var gsbm = new decimal(0);
            var StudentPaymentPlanInfo = new StudentPaymentPlanInfo();
            var SAInfo = new ExistAccountInfo();


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

                        var ClassId = string.Empty;
                        var StudentId = string.Empty;
                        var dt = new DataTable();
                        var myClassInfo = new ClassInfo();
                        var StudentPaymentId = InitObject.GetUUID();
                        var BatchNo = InitObject.GetSysDate(true);
                        var PaymentResult = 0;
                        var BillResult = 0;
                        var BillID = string.Empty;
                        var BillMonth = DateTime.Now.ToString("yyyy-MM");
                        var YearID = dr[YearPos].ToString();
                        List<string> ofm = new List<string>();
                        var IsAppend = false;


                        Write(StudentNo, " start handle this student no.", BznsBase.KindergartenId);

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
                        var BillStartEndDate = BillStartDate.AddMonths(Convert.ToInt32(dr[TMonthPos]));
                        var MonthDict = new Dictionary<string, MonthInfo>();
                        MonthDict = InitObject.SplitMonth(BillStartDate, Convert.ToInt32(dr[TMonthPos]), myClassInfo.CalendarId, YearID);


                        // Is exist student payment plan?

                        SAInfo = GetStudentAllAccount(StudentId);
                        StudentPaymentPlanInfo = GetStudentPaymentPlanInfoBySID(StudentId);

                        if (!string.IsNullOrEmpty(StudentPaymentPlanInfo.student_payment_id))
                        {

                            IsAppend = true;
                        }
                        else
                        {
                            IsAppend = false;
                        }

                        // Insert Into Student Payment Plan

                        if (!string.IsNullOrEmpty(StudentId))
                        {

                            if (IsAppend)
                            {

                                PaymentResult = 1;
                                StudentPaymentId = StudentPaymentPlanInfo.student_payment_id;

                                // Is throw an error log?

                                var MoneyFromTuition = TMoneyPos == -1 ? string.Empty : dr[TMoneyPos].ToString();
                                var MoneyFromMeals = MMoneyPos == -1 ? string.Empty : dr[MMoneyPos].ToString();
                                var MoneyFromSchoolBus = SBMoneyPos == -1 ? string.Empty : dr[SBMoneyPos].ToString();
                                var RefInfo = new StudentPaymentPlanRefInfo();
                                var IsHaveTuitionPlan = false;
                                var IsHaveMealsPlan = false;
                                var IsHaveSchoolBusPlan = false;


                                if (!string.IsNullOrEmpty(MoneyFromTuition) && Convert.ToDecimal(MoneyFromTuition) > 0)
                                {

                                    RefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT01");

                                    if (!string.IsNullOrEmpty(RefInfo.id))
                                    {
                                        IsHaveTuitionPlan = IsExistBillPlan("PAYMENTSUBJECT01", BillStartDate.ToString("yyyy-MM-dd"), BillStartEndDate.ToString("yyyy-MM-dd"), RefInfo.id);
                                        if (IsHaveTuitionPlan)
                                        {
                                            WriteForManual(StudentNo, " PAYMENTSUBJECT01", BznsBase.KindergartenId);
                                            continue;
                                        }
                                    }

                                }

                                if (!string.IsNullOrEmpty(MoneyFromMeals) && Convert.ToDecimal(MoneyFromMeals) > 0)
                                {

                                    RefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT02");

                                    if (!string.IsNullOrEmpty(RefInfo.id))
                                    {
                                        IsHaveMealsPlan = IsExistBillPlan("PAYMENTSUBJECT02", BillStartDate.ToString("yyyy-MM-dd"), BillStartEndDate.ToString("yyyy-MM-dd"), RefInfo.id);
                                        if (IsHaveMealsPlan)
                                        {
                                            WriteForManual(StudentNo, " PAYMENTSUBJECT02", BznsBase.KindergartenId);
                                            continue;
                                        }
                                    }

                                }

                                if (!string.IsNullOrEmpty(MoneyFromSchoolBus) && Convert.ToDecimal(MoneyFromSchoolBus) > 0)
                                {

                                    RefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT04");

                                    if (!string.IsNullOrEmpty(RefInfo.id))
                                    {
                                        IsHaveSchoolBusPlan = IsExistBillPlan("PAYMENTSUBJECT04", BillStartDate.ToString("yyyy-MM-dd"), BillStartEndDate.ToString("yyyy-MM-dd"), RefInfo.id);
                                        if (IsHaveSchoolBusPlan)
                                        {
                                            WriteForManual(StudentNo, " PAYMENTSUBJECT04", BznsBase.KindergartenId);
                                            continue;
                                        }
                                    }

                                }

                            }
                            else {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPayment, StudentPaymentId, StudentId, ClassId, BznsBase.KindergartenId, myClassInfo.ClassType, "Y", BznsBase.UserID, CreateDate);
                                PaymentResult = DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                            }

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



                        #region per tuition

                        // Insert Per Tuition Fee

                        var PerTuitionFeeMoney = PTMoneyPos == -1 ? string.Empty : dr[PTMoneyPos].ToString();
                        var PerTuitionFeeSubject = "PAYMENTSUBJECT05";

                        if (!string.IsNullOrEmpty(PerTuitionFeeMoney) && Convert.ToDecimal(PerTuitionFeeMoney) > 0 && PaymentResult > 0 && BillResult > 0)
                        {

                            var BillRefId = InitObject.GetUUID();
                            var BillNo = InitObject.GetBillNo(CreateDate, PerTuitionFeeSubject);
                            var BillStatus = "PAYMENTSTATE01";

                            Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRefNoInit, BillRefId, BillID, PerTuitionFeeSubject, BillNo, string.Empty, BillMonth, BillStatus, CreateDate, Convert.ToDecimal(PerTuitionFeeMoney));
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

                        }
                       

                        #endregion

                        #region tuition

                        // Insert Into PaymentPlanRef - Tuition Fee 

                        var PaymentMethod = "PAYMENTMODE04";
                        var TuitionFeeMoney = TMoneyPos == -1 ? string.Empty : dr[TMoneyPos].ToString();
                        var StartDate = StartDatePos == -1 ? string.Empty : Convert.ToDateTime(dr[TStartDatePos]).ToString("yyyy-MM-dd");
                        var TuitionFeeSubject = "PAYMENTSUBJECT01";
                        var RateId = TRateIDPos == -1 ? string.Empty : dr[TRateIDPos].ToString();
                        var DetailID = InitObject.GetUUID();
                        var BillMoney = new decimal(0);
                        var IsExistTuition = false;
                        var TuitionRefInfo = new StudentPaymentPlanRefInfo();

                        var DiscountId = DiscountIdPos == -1 ? string.Empty : dr[DiscountIdPos].ToString();
                        var DiscountType = DiscountTypePos == -1 ? string.Empty : dr[DiscountTypePos].ToString();
                        var DiscountMoney = new decimal(0);
                        var DiscountVal = new decimal(0);

                        if (!string.IsNullOrEmpty(dr[DiscountMoneyPos].ToString())) {
                            DiscountMoney = Convert.ToDecimal(dr[DiscountMoneyPos]);
                        }

                        // Is exist ref

                        TuitionRefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT01");

                        if (!string.IsNullOrEmpty(TuitionRefInfo.id))
                        {
                            IsExistTuition = true;
                            DetailID = TuitionRefInfo.id;
                        }

                        // Get RateId
                        if (!string.IsNullOrEmpty(TuitionFeeMoney) && Convert.ToDecimal(TuitionFeeMoney) > 0)
                        {
                            DescAlias = "标准学费";
                            DescMonthlyPrice = Convert.ToDecimal(TuitionFeeMoney);
                        }


                        // Insert into Tuition

                        if (!string.IsNullOrEmpty(TuitionFeeMoney) && Convert.ToDecimal(TuitionFeeMoney) > 0 && !string.IsNullOrEmpty(RateId) && PaymentResult > 0 && BillResult > 0)
                        {

                            DiscountVal = Convert.ToDecimal(TuitionFeeMoney) - Convert.ToDecimal(DiscountMoney);

                            if (!string.IsNullOrEmpty(DiscountId))
                            {

                                if (!string.IsNullOrEmpty(TuitionRefInfo.id))
                                {
                                    Sql = string.Format(InitObject.GetScriptServiceInstance().UpdatePaymentRefForShanghai, DiscountId, DiscountType, DiscountVal, DiscountMoney, BznsBase.UserID, CreateDate, TuitionRefInfo.id);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                }

                            }
                            
                            if (!IsExistTuition) {

                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, DetailID, RateId, BatchNo, StudentPaymentId, PaymentMethod, TuitionFeeSubject, Convert.ToDecimal(TuitionFeeMoney), DiscountId, DiscountType, DiscountMoney, DiscountVal, 1, BznsBase.UserID, CreateDate, StartDate, 0);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                DescBuilder.Append(string.Format("【学费: {0}/月 ，缴费方式:{1}，标准:{2} 】", DescMonthlyPrice, "月", DescAlias));

                            }

                            foreach (MoneyInfo mi in tmi)
                            {
                                if (!string.IsNullOrEmpty(dr[mi.MoneyPos].ToString()) && Convert.ToDecimal(dr[mi.MoneyPos]) > 0)
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
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

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
                                    var Money = Convert.ToDecimal(dr[tmi[i].MoneyPos]);

                                    if (IsFirstMonth)
                                    {
                                        FirstMonth = "Y";
                                    }

                                    if (Convert.ToDecimal(Money) < -1)
                                    {
                                        Money = Money * -1;
                                        Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, TuitionFeeSubject, DetailID, 0, FirstMonth, "Y", "N", CreateDate, val.StudyDays, TuitionFeeMoney, Money, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));
                                    }
                                    else {
                                        Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, TuitionFeeSubject, DetailID, 1, FirstMonth, "Y", "N", CreateDate, val.StudyDays, TuitionFeeMoney, Money, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));
                                    }

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
                        var mStartDate = MStartDatePos == -1 ? string.Empty : Convert.ToDateTime(dr[MStartDatePos]).ToString("yyyy-MM-dd");
                        var MealsSubject = "PAYMENTSUBJECT02";
                        var mRateId = MRateIDPos == -1 ? string.Empty : dr[MRateIDPos].ToString();
                        var mDetailID = InitObject.GetUUID();
                        var mBillMoney = new decimal(0);
                        var IsExistMeals = false;
                        var MealsRefInfo = new StudentPaymentPlanRefInfo();

                        // Is exist ref

                        MealsRefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT02");

                        if (!string.IsNullOrEmpty(MealsRefInfo.id))
                        {
                            IsExistMeals = true;
                            mDetailID = MealsRefInfo.id;
                        }


                        // Get RateId
                        if (!string.IsNullOrEmpty(MealsMoney) && Convert.ToDecimal(MealsMoney) > 0)
                        {
                            DescAlias = "标准餐费";
                            DescMonthlyPrice = Convert.ToDecimal(MealsMoney);
                        }


                        if (!string.IsNullOrEmpty(MealsMoney) && Convert.ToDecimal(MealsMoney) > 0 && !string.IsNullOrEmpty(mRateId) && PaymentResult > 0 && BillResult > 0)
                        {

                            if (!IsExistMeals) {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, mDetailID, mRateId, BatchNo, StudentPaymentId, PaymentMethod, MealsSubject, Convert.ToDecimal(MealsMoney), "", "", 0, 0, 1, BznsBase.UserID, CreateDate, mStartDate, 0);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                DescBuilder.Append(string.Format("【餐费: {0}/月 ，缴费方式:{1}，标准:{2} 】", DescMonthlyPrice, "月", DescAlias));
                            }


                            foreach (MoneyInfo mi in mmi)
                            {
                                if (!string.IsNullOrEmpty(dr[mi.MoneyPos].ToString()) && Convert.ToDecimal(dr[mi.MoneyPos]) > 0) {
                                    mBillMoney += Convert.ToDecimal(dr[mi.MoneyPos]);
                                }
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
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);

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
                                    var Money = Convert.ToDecimal(dr[mmi[i].MoneyPos]);

                                    if (IsFirstMonth)
                                    {
                                        FirstMonth = "Y";
                                    }

                                    if (Convert.ToDecimal(Money) < -1)
                                    {
                                        Money = Money * -1;
                                        Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, MealsSubject, mDetailID, 0, FirstMonth, "Y", "N", CreateDate, val.StudyDays, MealsMoney, Money, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));
                                    }
                                    else {
                                        Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, MealsSubject, mDetailID, 1, FirstMonth, "Y", "N", CreateDate, val.StudyDays, MealsMoney, Money, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));
                                    }


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
                        var sbStartDate = string.Empty;

                        if (!string.IsNullOrEmpty(dr[SBStartDatePos].ToString())) {
                            sbStartDate = Convert.ToDateTime(dr[SBStartDatePos]).ToString("yyyy-MM-dd");
                        }
                        
                        var sbSubject = "PAYMENTSUBJECT04";
                        var sbRateId = SBRateIDPos == -1 ? string.Empty : dr[SBRateIDPos].ToString();
                        var sbDetailID = InitObject.GetUUID();
                        var sbBillMoney = new decimal(0);

                        var IsExistSchoolBus = false;
                        var SchoolBusRefInfo = new StudentPaymentPlanRefInfo();


                        // Is exist ref

                        SchoolBusRefInfo = GetStudentPaymentPlanRefInfo(StudentPaymentId, "PAYMENTSUBJECT04");

                        if (!string.IsNullOrEmpty(SchoolBusRefInfo.id))
                        {
                            IsExistSchoolBus = true;
                            sbDetailID = SchoolBusRefInfo.id;
                        }

                        // Get RateId
                        if (!string.IsNullOrEmpty(sbMoney) && Convert.ToDecimal(sbMoney) > 0)
                        {

                            DescAlias = "标准校车费";
                            DescMonthlyPrice = Convert.ToDecimal(sbMoney);
                        }


                        if (!string.IsNullOrEmpty(sbMoney) && Convert.ToDecimal(sbMoney) > 0 && !string.IsNullOrEmpty(sbRateId) && PaymentResult > 0 && BillResult > 0)
                        {

                            if (!IsExistSchoolBus) {
                                Sql = string.Format(InitObject.GetScriptServiceInstance().InsertStudentPaymentRef, sbDetailID, sbRateId, BatchNo, StudentPaymentId, PaymentMethod, sbSubject, Convert.ToDecimal(sbMoney), "", "", 0, 0, 1, BznsBase.UserID, CreateDate, sbStartDate, 0);
                                DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                                DescBuilder.Append(string.Format("【校车费: {0}/月 ，缴费方式:{1}，标准:{2} 】", DescMonthlyPrice, "月", DescAlias));
                            }

                           
                            foreach (MoneyInfo mi in sbmi)
                            {
                                if (!string.IsNullOrEmpty(dr[mi.MoneyPos].ToString()) && Convert.ToDecimal(dr[mi.MoneyPos]) > 0) {
                                    sbBillMoney += Convert.ToDecimal(dr[mi.MoneyPos]);
                                }  
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
                                    var Money = Convert.ToDecimal( dr[sbmi[i].MoneyPos]);

                                    if (IsFirstMonth)
                                    {
                                        FirstMonth = "Y";
                                    }

                                    if (Money < -1)
                                    {
                                        Money = Money * -1;
                                        Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, sbSubject, sbDetailID, 0, FirstMonth, "Y", "N", CreateDate, val.StudyDays, Convert.ToDecimal(sbMoney), Money, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));
                                    }
                                    else {
                                        Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillPlan, BillPlanID, BillNo, sbSubject, sbDetailID, 1, FirstMonth, "Y", "N", CreateDate, val.StudyDays, Convert.ToDecimal(sbMoney), Money, val.StartDate.ToString("yyyy-MM-dd"), val.EndDate.ToString("yyyy-MM-dd"));
                                    }

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
                            var ofSubject = "PAYMENTSUBJECT03";

                            if (!string.IsNullOrEmpty(ofMoney) && Convert.ToDecimal(ofMoney) > 0)
                            {

                                if (PaymentResult > 0 && BillResult > 0)
                                {
                                    
                                    // Insert into bill ref.
                                    var BillRefId = InitObject.GetUUID();
                                    var BillNo = InitObject.GetBillNo(CreateDate, ofSubject);
                                    var BillStatus = "PAYMENTSTATE01";

                                    Sql = string.Format(InitObject.GetScriptServiceInstance().InsertFinBillRefNoInit, BillRefId, BillID, ofSubject, BillNo, string.Empty, BillMonth, BillStatus, CreateDate, Convert.ToDecimal(ofMoney));
                                    if (DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null) <= 0)
                                    {
                                        SysLog.Insert(new SysLogInfo(string.Concat(StudentNo, " bill ref insert error."), SysLogType.WARNING, string.Concat(ModuleName, " - ", "Import Once Fee")));
                                    }
                                    else {
                                        ofm.Add(ofMoney);
                                    }

                                }

                            }

                        }

                        #endregion



                        // update payment plan if not is append

                        if (!IsAppend) {
                            Sql = string.Format(InitObject.GetScriptServiceInstance().UpdateStudentPayment, DescBuilder.ToString(), StudentPaymentId);
                            DBHelper.MySqlHelper.ExecuteNonQuery(trans, BznsBase.GetCommandType, Sql, null);
                        }

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

        private StudentBalanceAccountInfo GetStudentBalanceAccountInfo(string sid)
        {

            var SBAInfo = new StudentBalanceAccountInfo();

            try
            {
                // GetStudentBalanceAccountInfoBySID
                var Sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentBalanceAccountInfoBySID, sid);
                var dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    SBAInfo.AccountId = dt.Rows[0]["account_id"].ToString();
                    SBAInfo.BalanceId = dt.Rows[0]["stu_balance_id"].ToString();
                    SBAInfo.StudentId = dt.Rows[0]["stu_id"].ToString();
                    SBAInfo.AccountAmt = Convert.ToDecimal(dt.Rows[0]["account_amt"].ToString());
                    SBAInfo.AvlAmt = Convert.ToDecimal(dt.Rows[0]["avl_amt"].ToString());
                    SBAInfo.IsNone = false;
                }
                else
                {
                    SBAInfo.IsNone = true;
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }

            return SBAInfo;

        }

        private ExistAccountInfo GetStudentAllAccount(string sid)
        {

            var EAInfo = new ExistAccountInfo
            {
                IsExistPreAccount = false,
                IsExistTuitionAccount = false,
                IsExistMealsAccount = false,
                IsExistSchoolBusAccount = false,
                IsExistOtherAccount = false,
                PreAccountId = string.Empty,
                TuitionAccountId = string.Empty,
                MealesAccountId = string.Empty,
                SchoolBusAccountId = string.Empty,
                OtherAccountId = string.Empty
            };

            try
            {

                var Sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentAllAccountBySID, sid);
                var dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        if (Convert.ToString(dr["account_type"]).ToUpper() == "PAYMENTSUBJECT01")
                        {
                            EAInfo.IsExistTuitionAccount = true;
                            EAInfo.TuitionAccountId = dr["account_id"].ToString();
                        }

                        if (Convert.ToString(dr["account_type"]).ToUpper() == "PAYMENTSUBJECT02")
                        {
                            EAInfo.IsExistMealsAccount = true;
                            EAInfo.MealesAccountId = dr["account_id"].ToString();
                        }

                        if (Convert.ToString(dr["account_type"]).ToUpper() == "PAYMENTSUBJECT03")
                        {
                            EAInfo.IsExistOtherAccount = true;
                            EAInfo.OtherAccountId = dr["account_id"].ToString();
                        }

                        if (Convert.ToString(dr["account_type"]).ToUpper() == "PAYMENTSUBJECT04")
                        {
                            EAInfo.IsExistSchoolBusAccount = true;
                            EAInfo.SchoolBusAccountId = dr["account_id"].ToString();
                        }

                        if (Convert.ToString(dr["account_type"]).ToUpper() == "PAYMENTSUBJECT05")
                        {
                            EAInfo.IsExistPreAccount = true;
                            EAInfo.PreAccountId = dr["account_id"].ToString();
                        }

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

            return EAInfo;

        }

        private StudentPaymentPlanInfo GetStudentPaymentPlanInfoBySID(string sid)
        {

            var ret = new StudentPaymentPlanInfo();

            try
            {
                var Sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentPaymentPlanInfoBySID, sid);
                var dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    ret.id = Convert.ToInt32(dt.Rows[0]["id"]);
                    ret.rates_id = dt.Rows[0]["rates_id"].ToString();
                    ret.student_id = dt.Rows[0]["student_id"].ToString();
                    ret.student_payment_id = dt.Rows[0]["student_payment_id"].ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return ret;
        }

        private StudentPaymentPlanRefInfo GetStudentPaymentPlanRefInfo(string sppid, string subject)
        {

            var ret = new StudentPaymentPlanRefInfo();

            try
            {
                var Sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentPaymentPlanRefInfoBySPPIDAndSubject, sppid, subject);
                var dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    ret.id = dt.Rows[0]["id"].ToString();
                    ret.rates_id = dt.Rows[0]["rates_id"].ToString();
                    ret.batch_number = dt.Rows[0]["batch_number"].ToString();
                    ret.pay_discount_id = dt.Rows[0]["pay_discount_id"].ToString();
                    ret.payment_discount = dt.Rows[0]["payment_discount"].ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return ret;

        }

        private bool IsExistBillPlan(string Subject, string StartDate, string EndDate, string StudentPaymentRefId)
        {
            var ret = false;
            try
            {
                var Sql = string.Format(InitObject.GetScriptServiceInstance().GetBillPlanBySubjectAndStartDateAndEndDate, StartDate, EndDate, Subject, StudentPaymentRefId);
                var dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    ret = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return ret;
        }

        private RatesInfo GetRatesInfoByID(string rid)
        {
            var ret = new RatesInfo();
            try
            {
                var Sql = string.Format(InitObject.GetScriptServiceInstance().GetRatesByID, rid);
                var dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    ret.id = Convert.ToInt32(dt.Rows[0]["id"]);
                    ret.rates_alias = dt.Rows[0]["rates_alias"].ToString();
                    ret.rates_monthly = Convert.ToDecimal(dt.Rows[0]["rates_monthly"].ToString());
                    ret.rates_id = dt.Rows[0]["rates_id"].ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return ret;
        }

        private bool ExistStudentPaymentPlan(ExistAccountInfo SA)
        {
            var ret = true;
            try
            {
                if (SA.IsExistTuitionAccount || SA.IsExistMealsAccount || SA.IsExistSchoolBusAccount || SA.IsExistPreAccount || SA.IsExistOtherAccount)
                {
                    ret = false;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return ret;
        }

        public bool UpdateRef(List<string> klist) {

            var ret = false;

            try
            {

                foreach (string k in klist)
                {


                    try
                    {

                        WriteForManual(k, " start....",k + ".txt");

                        var sql = string.Format(InitObject.GetScriptServiceInstance().GetStudentsForUpdate, k);
                        var students = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];

                        foreach (DataRow s in students.Rows)
                        {

                            var sid = s["stu_id"].ToString();
                            var tid = string.Empty;
                            var mid = string.Empty;
                            var sbid = string.Empty;

                            var sppSql = string.Empty;
                            var sppDelSql = string.Empty;
                            var sppDt = new DataTable();
                            var sppid = string.Empty;
                            var sppTotal = 0;

                            var spprDelSql = string.Empty;
                            var spprUpdateSql = string.Empty;


                            // get student payment plan

                            sppSql = string.Format(InitObject.GetScriptServiceInstance().GetSPPForUpdate, sid);
                            sppDt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sppSql, null).Tables[0];

                            sppTotal = sppDt.Rows.Count;

                            if (sppTotal > 0)
                            {

                                sppid = sppDt.Rows[0]["student_payment_id"].ToString();

                            }

                            // delete and update of 3,5 student payment plan ref via sppid

                            foreach (DataRow spp in sppDt.Rows)
                            {

                                spprDelSql = string.Format(InitObject.GetScriptServiceInstance().DeleteSPPRForUpdate, spp["student_payment_id"].ToString());
                                DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, spprDelSql, null);

                                spprUpdateSql = string.Format(InitObject.GetScriptServiceInstance().UpdateSPPRForUpdate, sppid, spp["student_payment_id"].ToString());
                                DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, spprUpdateSql, null);

                            }

                            // delete student payment plan ref via sppid

                            if (!DeleteRepeat(sppid, "PAYMENTSUBJECT01"))
                            {
                                WriteForManual(sid, " Repat PAYMENTSUBJECT01", k + ".txt");
                                if (sppTotal > 1)
                                {
                                    sppDelSql = string.Format(InitObject.GetScriptServiceInstance().DeleteSPPForUpdate, sid, sppid);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, sppDelSql, null);
                                }
                                continue;
                            }

                            if (!DeleteRepeat(sppid, "PAYMENTSUBJECT02"))
                            {
                                WriteForManual(sid, " Repat PAYMENTSUBJECT02", k + ".txt");
                                if (sppTotal > 1)
                                {
                                    sppDelSql = string.Format(InitObject.GetScriptServiceInstance().DeleteSPPForUpdate, sid, sppid);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, sppDelSql, null);
                                }
                                continue;
                            }

                            if (!DeleteRepeat(sppid, "PAYMENTSUBJECT04"))
                            {
                                WriteForManual(sid, " Repat PAYMENTSUBJECT04", k + ".txt");
                                if (sppTotal > 1)
                                {
                                    sppDelSql = string.Format(InitObject.GetScriptServiceInstance().DeleteSPPForUpdate, sid, sppid);
                                    DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, sppDelSql, null);
                                }
                                continue;
                            }

                            // get new refs, one student just 3 reocrds.

                            var refsql = string.Format(InitObject.GetScriptServiceInstance().GetNewSPPRForUpdate, sppid);
                            var refdt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, refsql, null).Tables[0];

                            foreach (DataRow r in refdt.Rows)
                            {

                                if (r["payment_rule_subject"].ToString() == "PAYMENTSUBJECT01")
                                {
                                    tid = r["id"].ToString();
                                }

                                if (r["payment_rule_subject"].ToString() == "PAYMENTSUBJECT02")
                                {
                                    mid = r["id"].ToString();
                                }

                                if (r["payment_rule_subject"].ToString() == "PAYMENTSUBJECT04")
                                {
                                    sbid = r["id"].ToString();
                                }

                            }

                            // get bill ref via student id

                            var billrefSql = string.Format(InitObject.GetScriptServiceInstance().GetBillRefByBillIDForUpdate, sid);
                            var billrefdt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, billrefSql, null).Tables[0];

                            foreach (DataRow br in billrefdt.Rows)
                            {

                                if (br["bill_type"].ToString() == "PAYMENTSUBJECT01")
                                {
                                    var DeleteBillRefSql = string.Format(InitObject.GetScriptServiceInstance().UpdateBillRefByIDForUpdate, tid, Convert.ToInt32(br["id"]));
                                    DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, DeleteBillRefSql, null);
                                }

                                if (br["bill_type"].ToString() == "PAYMENTSUBJECT02")
                                {
                                    var DeleteBillRefSql = string.Format(InitObject.GetScriptServiceInstance().UpdateBillRefByIDForUpdate, mid, Convert.ToInt32(br["id"]));
                                    DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, DeleteBillRefSql, null);
                                }

                                if (br["bill_type"].ToString() == "PAYMENTSUBJECT03")
                                {
                                    var DeleteBillRefSql = string.Format(InitObject.GetScriptServiceInstance().UpdateBillRefByIDForUpdate, string.Empty, Convert.ToInt32(br["id"]));
                                    DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, DeleteBillRefSql, null);
                                }

                                if (br["bill_type"].ToString() == "PAYMENTSUBJECT04")
                                {
                                    var DeleteBillRefSql = string.Format(InitObject.GetScriptServiceInstance().UpdateBillRefByIDForUpdate, sbid, Convert.ToInt32(br["id"]));
                                    DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, DeleteBillRefSql, null);
                                }

                                if (br["bill_type"].ToString() == "PAYMENTSUBJECT05")
                                {
                                    var DeleteBillRefSql = string.Format(InitObject.GetScriptServiceInstance().UpdateBillRefByIDForUpdate, string.Empty, Convert.ToInt32(br["id"]));
                                    DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, DeleteBillRefSql, null);
                                }

                            }

                            // delete student payment plan if record greater than 1 

                            if (sppTotal > 1)
                            {
                                sppDelSql = string.Format(InitObject.GetScriptServiceInstance().DeleteSPPForUpdate, sid, sppid);
                                DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, sppDelSql, null);
                            }

                        }

                        WriteForManual(k, " end....", k + ".txt");

                    }
                    catch (Exception ex)
                    {
                        WriteForManual(k, " error.... " + ex.Message.ToString(), k + ".txt");
                        throw;
                    }

                    
                   

                }

            }
            catch (Exception e)
            {

            }
            finally
            {

            }

            return ret;
        }

        public bool DeleteData()
        {

            var ret = true;
            var sql = string.Empty;

            try
            {

                sql = string.Format(InitObject.GetScriptServiceInstance().DeleteSPP, BznsBase.KindergartenId);
                DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null);

                sql = string.Empty;

                sql = string.Format(InitObject.GetScriptServiceInstance().DeleteSPPR, BznsBase.KindergartenId);
                DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null);

                sql = string.Empty;

                sql = string.Format(InitObject.GetScriptServiceInstance().DeleteFB, BznsBase.KindergartenId);
                DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null);


            }
            catch (Exception e)
            {
                ret = false;
            }
            finally
            {

            }

            return ret;
        }

        public bool DeleteRepeat(string sppid, string subject) {

            var ret = true;
            var sql = string.Empty;
            var dt = new DataTable();
            var rid = string.Empty;

            try
            {
                sql = string.Format(InitObject.GetScriptServiceInstance().GetSPPRBySubjectAndSPPIDForUpdate, sppid, subject);
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];

                if (dt.Rows.Count > 1) {

                    rid = dt.Rows[0]["rates_id"].ToString();

                    foreach (DataRow dr in dt.Rows) {

                        if (dr["rates_id"].ToString() != rid) {
                            ret = false;
                        }
                    }

                    if (ret) {

                        // delete from 2 to end record.

                        for (int i = 1; i < dt.Rows.Count; i++) {
                            sql = string.Format(InitObject.GetScriptServiceInstance().DeleteSPPRBySubjectAndSPPIDForUpdate, dt.Rows[i]["id"].ToString());
                            DBHelper.MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null);
                        }
                        
                    }

                }

            }
            catch (Exception)
            {
                ret = false;
                throw;
            }
            finally {

            }

            return ret;

        }

        public void Write(string sno, string desc = "",string fn = "")
        {
            FileStream fs = new FileStream(@"C:\Users\Jack\Desktop\Import V3\Import Log\" + fn + "_log.txt", FileMode.Append);
            var d = string.Concat(Convert.ToString(DateTime.Now), " ", sno, desc, "\r\n");
            byte[] data = System.Text.Encoding.Default.GetBytes(d);
            fs.Position = fs.Length;
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }

        public void WriteForManual(string sno, string desc = "", string fn = "")
        {
            FileStream fs = new FileStream(@"C:\Users\Jack\Desktop\Import V3\Import Log\" + fn + "_manual.txt", FileMode.Append);
            var d = string.Concat(Convert.ToString(DateTime.Now), " ", sno, desc, "\r\n");
            byte[] data = System.Text.Encoding.Default.GetBytes(d);
            fs.Position = fs.Length;
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }


    }


}
