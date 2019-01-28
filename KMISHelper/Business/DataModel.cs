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
