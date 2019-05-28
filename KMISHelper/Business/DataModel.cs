using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.Business
{

    struct CategoryInfo
    {
        public string CategoryId { get; set; }
        public string Name { get; set; }
    }

    struct StudentInfo
    {
        public string StudentId { get; set; }
        public string ClassId { get; set; }
    }

    struct CourseInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Hours { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string ChargeType { get; set; }

    }

    struct OnceInfo
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }

    struct BillInfo
    {
        public string BillId { get; set; }
        public string StudentId { get; set; }
        public string YearId { get; set; }
        public string ClassId { get; set; }
        public decimal SumMoney { get; set; }
    }

    struct BillRefInfo
    {
        public string RefID { get; set; }

        // Type is finance subject.
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public decimal Money { get; set; }
        public decimal Payment { get; set; }
    }

    struct StudentBalanceInfo
    {
        public string StudentId { get; set; }
        public string BalanceId { get; set; }
        public decimal Total { get; set; }
        public decimal Income { get; set; }

    }

    struct PreTuitionInfo
    {
        public bool IsDeductionPreTuition { get; set; }
        public decimal PreTuitionMoney { get; set; }
        public bool IsLog { get; set; }
        public int Index { get; set; }
    }

    struct MoneyInfo {
        public string Month { get; set; }
        public int MoneyPos { get; set; }

    }

    struct StudentBalanceAccountInfo {
        public string AccountId { get; set; }
        public string BalanceId { get; set; }
        public string StudentId { get; set; }
        public decimal AccountAmt { get; set; }
        public decimal AvlAmt { get; set; }
        public bool IsNone { get; set; }


    }

    struct ExistAccountInfo{
        public bool IsExistPreAccount { get; set; }
        public bool IsExistTuitionAccount { get; set; }
        public bool IsExistMealsAccount { get; set; }
        public bool IsExistSchoolBusAccount { get; set; }
        public bool IsExistOtherAccount { get; set; }
        public string PreAccountId { get; set; }
        public string TuitionAccountId { get; set; }
        public string MealesAccountId { get; set; }
        public string SchoolBusAccountId { get; set; }
        public string OtherAccountId { get; set; }
    }

    struct StudentPaymentPlanInfo {
        public int id { get; set; }
        public string student_payment_id { get; set; }
        public string student_id { get; set; }
        public string rates_id { get; set; }
    }

    struct StudentPaymentPlanRefInfo {

        public string id { get; set; }
        public string rates_id { get; set; }
        public string batch_number { get; set; }
        public string student_payment_id { get; set; }
        public string payment_rule_subject { get; set; }
        public string payment_type { get; set; }
        public decimal payment_money { get; set; }
        public string pay_discount_id { get; set; }
        public string payment_discount { get; set; }
        public decimal payment_discount_value { get; set; }
        public decimal payment_actual_money { get; set; }
        public decimal payment_daily_money { get; set; }
        public string week_days { get; set; }
        public int status { get; set; }
        public DateTime billing_start_date { get; set; }
        public int plan_month { get; set; }
        public string process_instance_id { get; set; }
        public DateTime discount_valid_date { get; set; }
    }

    struct BillPlanInfo {
        public int id { get; set; }
        public string bill_plan_id { get; set; }
        public string bill_no { get; set; }
        public string bill_type { get; set; }
        public string payment_plan_ref_id { get; set; }
        public decimal daily_unit_price { get; set; }
        public int teaching_days_actual { get; set; }
        public int teaching_days_total { get; set; }
        public decimal bill_original_price { get; set; }
        public decimal bill_discount_price { get; set; }
        public DateTime bill_date_start { get; set; }
        public DateTime bill_date_end { get; set; }
        public int bill_status { get; set; }
        public string first_month_yn { get; set; }
        public string effect_yn { get; set; }
        public string pay_yn { get; set; }
        public DateTime create_dts { get; set; }
        public string create_by { get; set; }
        public string update_by { get; set; }
        public DateTime update_dts { get; set; }

    }

    struct RatesInfo {
        public int id { get; set; }
        public string rates_id { get; set; }
        public string rates_alias { get; set; }
        public decimal rates_monthly { get; set; }
    }

    public enum SubjectLogType
    {
        //虚增
        WATERED,
        //实增
        REALGROWTH,
        //虚减
        ASTHENIA,
        //实减
        ACTUALREDUCTION,
        //充抵
        OFFSET
    }





}
