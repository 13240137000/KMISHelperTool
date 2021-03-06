﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    partial class ScriptService
    {
        
        // K kindergarten_id M Money S Subject

        public string InsertStudentPayment = "INSERT INTO stu_payment_plan(student_payment_id,student_id,class_id,school_id,class_type,effect_yn,create_id,create_dts ) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";

        public string InsertStudentPaymentRef = "INSERT INTO stu_payment_plan_ref (id,rates_id,batch_number,student_payment_id,payment_type,payment_rule_subject,payment_money,pay_discount_id,payment_discount,payment_actual_money,payment_daily_money,status,create_id,create_dts,billing_start_date,payment_discount_value) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}','{8}',{9},{10},{11},'{12}','{13}','{14}',{15})";

        public string GetRateIdByKMS = "SELECT fr.* FROM fin_rates fr INNER JOIN fin_rates_kindergarten_ref frkr ON frkr.rates_id = fr.rates_id WHERE fr.rates_monthly = {0} AND fr.rates_subject = '{1}' AND frkr.kindergarten_id = '{2}' AND fr.year_id = '{3}'";

        public string GetRateIdByKS = "SELECT fr.* FROM fin_rates fr INNER JOIN fin_rates_kindergarten_ref frkr ON frkr.rates_id = fr.rates_id WHERE fr.rates_subject = '{0}' AND frkr.kindergarten_id = '{1}' AND fr.year_id = '{2}'";

        public string GetRateIdByAliasName = "SELECT fr.* FROM fin_rates fr INNER JOIN fin_rates_kindergarten_ref frkr ON frkr.rates_id = fr.rates_id WHERE fr.rates_subject = '{0}' AND frkr.kindergarten_id = '{1}' AND fr.year_id = '{2}' AND fr.rates_alias = '{3}' limit 1";

        public string GetDiscountIdByKMS = "SELECT fd.* FROM fin_discount fd INNER JOIN fin_discount_kindergarten_ref fdkr ON fdkr.discount_id = fd.discount_id WHERE fd.discount_subject = '{0}' AND fdkr.kindergarten_id = '{1}' AND discount_value = {2}";

        public string GetDiscountIdByKMSN = "SELECT fd.* FROM fin_discount fd INNER JOIN fin_discount_kindergarten_ref fdkr ON fdkr.discount_id = fd.discount_id WHERE fd.discount_subject = '{0}' AND fdkr.kindergarten_id = '{1}' AND discount_nm = '{2}' ";

        public string UpdateStudentPayment = "UPDATE stu_payment_plan SET student_payment_intro = '{0}' WHERE student_payment_id = '{1}'";

        public string InsertDiscountApprovedLog = "INSERT INTO stu_discount_review_tasklog (id,bus_id,task_name,deal_id,deal_time,app_action,create_time) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";

        public string InsertFinBill = "INSERT INTO fin_bill(bill_id,stu_id,acad_year_id,payment_plan_id,class_id,bill_status,bill_month,bill_alert,effect_yn,create_dts,bill_date_start,bill_date_end) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')";

        public string InsertFinBillRef = "INSERT INTO fin_bill_ref(bill_ref_id,bill_id,bill_type,bill_no,payment_plan_ref_id,bill_month,bill_status,create_dts,bill_date_start,bill_date_end,bill_money) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10})";

        public string InsertFinBillRefNoInit = "INSERT INTO fin_bill_ref(bill_ref_id,bill_id,bill_type,bill_no,payment_plan_ref_id,bill_month,bill_status,create_dts,bill_money) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8})";

        public string InsertFinBillPlan = "INSERT INTO fin_bill_plan (bill_plan_id,bill_no,bill_type,payment_plan_ref_id,bill_status,first_month_yn,effect_yn,pay_yn,create_dts,teaching_days_total,bill_original_price,bill_discount_price,bill_date_start,bill_date_end) VALUES ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}',{9},{10},{11},'{12}','{13}')";

        public string InsertFinBillNoLoop = "INSERT INTO fin_bill_plan (bill_plan_id,bill_no,bill_type,payment_plan_ref_id,bill_status,first_month_yn,effect_yn,pay_yn,create_dts,teaching_days_total,bill_original_price,bill_discount_price) VALUES ('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}',{9},{10},{11})";

        public string GetStudy = "SELECT date_id,date_time,date_type FROM acad_holiday_dates WHERE date_time BETWEEN '{0}' AND '{1}' ORDER BY date_time ASC";

        public string GetHolidays = "SELECT hds.date_id,hds.date_type,hds.acad_year_id,date_time FROM acad_school_calendar sc INNER JOIN acad_holiday_data hd on sc.holiday_id = sc.holiday_id INNER JOIN acad_holiday_dates hds on hd.holiday_id = hds.holiday_id WHERE sc.school_calendar_id = '{0}' AND hds.acad_year_id = '{1}'";

        public string UpdateFinBill = "UPDATE fin_bill SET bill_sum_money = {0}, bill_sum_payment = {1} WHERE bill_id = '{2}'";

        public string GetBillListByStudentId = "SELECT * FROM fin_bill WHERE stu_id = '{0}' and bill_status = 'PAYMENTSTATE01' order by id desc";

        public string GetBillRefListByBillId = "SELECT * FROM fin_bill_ref WHERE bill_id = '{0}'";

        public string PaymentInsert = "INSERT INTO fin_payment(payment_id,bill_id,payment_money,payment_way,payment_time,payment_no,payment_status,create_by,create_dts,receipt_number) VALUES ('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}')";

        public string PaymentRefInsert = "INSERT INTO fin_payment_bill_ref(id,bill_ref_id,payment_id,payment_money,status,create_dts) VALUES ('{0}','{1}','{2}',{3},'0','{4}')";

        public string GetStudentBalanceListByStudentId = "SELECT * FROM stu_balance WHERE student_id = '{0}'";

        public string StudentBalanceAccountInsert = "INSERT INTO stu_balance_account(account_id,stu_balance_id,stu_id,account_amt,account_type,create_id,create_dts) VALUES ('{0}','{1}','{2}',{3},'{4}','{5}','{6}')";

        public string SBAUpdate = "update stu_balance_account set account_amt = account_amt + {0},avl_amt = avl_amt + {1},update_id='{2}',update_dts = '{3}' where account_id = '{4}'";

        public string StudentBalanceAccountUpdate = "UPDATE stu_balance_account SET account_amt = account_amt + {0} WHERE account_id = '{1}'";

        public string GetStudentBalanceAccountListByStudentIDAndSubject = "SELECT * FROM stu_balance_account WHERE stu_id = '{0}' AND account_type = '{1}'";

        public string TuitionAccountInsert = "INSERT INTO stu_tuition_account(tuition_id,stu_account_id,stu_id,acad_year_id,account_amt,effect_yn,tbc_balance,bill_ref_id,create_id,create_dts) VALUES ('{0}','{1}','{2}','{3}',{4},'{5}',{6},'{7}','{8}','{9}')";

        public string MealsAccountInsert = "INSERT INTO stu_meal_account(meal_id,stu_account_id,stu_id,acad_year_id,account_amt,effect_yn,tbc_balance,bill_ref_id,create_id,create_dts) VALUES ('{0}','{1}','{2}','{3}',{4},'{5}',{6},'{7}','{8}','{9}')";

        public string SchoolBusAccountInsert = "INSERT INTO stu_school_bus_account(school_bus_id,stu_account_id,stu_id,acad_year_id,account_amt,effect_yn,tbc_balance,bill_ref_id,create_id,create_dts) VALUES ('{0}','{1}','{2}','{3}',{4},'{5}',{6},'{7}','{8}','{9}')";

        public string PrepaidTuitionAccountInsert = "INSERT INTO stu_prepaid_tuition_account(prepaid_tuition_id,stu_account_id,stu_id,acad_year_id,account_amt,effect_yn,tbc_balance,bill_ref_id,create_id,create_dts) VALUES ('{0}','{1}','{2}','{3}',{4},'{5}',{6},'{7}','{8}','{9}')";

        public string ItemAccountInsert = "INSERT INTO stu_item_account(item_id,stu_account_id,stu_id,acad_year_id,account_amt,effect_yn,tbc_balance,bill_ref_id,create_id,create_dts) VALUES ('{0}','{1}','{2}','{3}',{4},'{5}',{6},'{7}','{8}','{9}')";

        public string BalanceUpdate = "UPDATE stu_balance SET balance_total = balance_total + {0},balance_income = balance_income + {1} WHERE student_id = '{2}'";

        public string BillUpdate = "UPDATE fin_bill SET bill_status = 'PAYMENTSTATE02',bill_sum_payment = bill_sum_money WHERE bill_id = '{0}'";

        public string BillRefUpdate = "update fin_bill_ref set bill_status ='PAYMENTSTATE02',bill_payment = bill_money  where bill_id = '{0}'";

        public string SubjectLogInsert = "INSERT INTO stu_subject_account_log(stu_id,account_type,subject_account_id,log_type,log_info,log_money,log_time,bill_ref_id,source_account) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}')";

        public string StudentBalanceLogInsert = "INSERT INTO stu_balance_log(log_type,student_id,log_money,log_time,log_way,del_yn,stu_balance_account_id,bill_id,log_info) VALUES ('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}','{8}')";

        public string BillPlanUpdate = "UPDATE fin_bill_plan SET pay_yn = 'Y' WHERE payment_plan_ref_id = '{0}'";

        public string GetStudentPaymentPlanListByID = "SELECT * FROM stu_payment_plan WHERE student_id = '{0}'";

        public string GetStudentPaymentPlanRefListByStudentID = "SELECT * FROM stu_payment_plan_ref WHERE student_payment_id = '{0}'";

        public string GetStudentPyamentPlanListByStudentID = "select * from stu_payment_plan where student_id = '{0}' and effect_yn = 'Y' and del_yn = 'N'";

        public string GetBalanceAccountByStudentIDAndType = "select * from stu_balance_account where stu_id = '{0}' and account_type = '{1}'";

    }
}
