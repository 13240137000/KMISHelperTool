using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    partial class ScriptService
    {

        public string InterestInsert = "INSERT INTO delay_class(delay_class_id,class_nm,delay_type,class_start_date,class_end_date,total_hours,lesson_price,total_amount,lesson_type,status,kindergarten_id,create_dts,create_by,charge_type) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},'{8}','{9}','{10}','{11}','{12}','{13}')";

        public string GetInterestListByName = "SELECT * FROM delay_class WHERE class_nm = '{0}' AND kindergarten_id = '{1}'";

        public string InterestStudentInsert = "INSERT INTO delay_class_stu(class_stu_id,delay_class_id,stu_id,stu_type) VALUES ('{0}','{1}','{2}','{3}')";

        public string CategoryInsert = "INSERT INTO delay_extracurricular_type(type_id,type_name,kindergarten_id,create_by,create_dts) VALUES ('{0}','{1}','{2}','{3}','{4}')";

        public string GetCourseCategoryByName = "SELECT * FROM delay_extracurricular_type WHERE kindergarten_id = '{0}' AND type_name = '{1}'";

        public string InterestBillInsert = "INSERT INTO fin_bill(bill_id,stu_id,acad_year_id,class_id,bill_status,bill_month,bill_sum_money,bill_sum_payment,bill_type,bill_alert,effect_yn,create_dts) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}','{10}','{11}')";

        public string InterestBillRefInsert = "INSERT INTO fin_bill_ref(bill_ref_id,bill_id,bill_type,bill_no,payment_plan_ref_id,bill_month,teaching_days_actual,teaching_days_total,bill_date_start,bill_date_end,bill_status,bill_money,bill_payment,bill_payment_date,create_dts,create_by) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},'{8}','{9}','{10}',{11},{12},'{13}','{14}','{15}')";

        public string InterestBillPaymentInsert = "INSERT INTO fin_payment(payment_id,bill_id,bill_no,payment_money,payment_way,payment_time,payment_no,receipt_number,payment_status,create_by,create_dts) VALUES ('{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}')";

        public string InterestBillPaymentRefInsert = "INSERT INTO fin_payment_bill_ref(id,bill_ref_id,payment_id,payment_money,status,create_dts) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')";

        public string InterestStudentBalanceAccountInsert = "INSERT INTO stu_balance_account(account_id,stu_balance_id,stu_id,account_amt,account_type,create_id,create_dts) VALUES ('{0}','{1}','{2}',{3},'{4}','{5}','{6}')";

        public string DelayInsert = "INSERT INTO stu_delay_account(delay_id,stu_account_id,stu_id,acad_year_id,account_amt,effect_yn,tbc_balance,bill_ref_id,create_id,create_dts) VALUES ('{0}','{1}','{2}','{3}',{4},'{5}',{6},'{7}','{8}','{9}')";

        public string InterestBalanceUpdate = "UPDATE stu_balance SET balance_total = balance_total + {0}, balance_income = balance_income + {1} WHERE student_id = '{2}'";

        public string InterestSubjectLogInsert = "INSERT INTO stu_subject_account_log(stu_id,account_type,subject_account_id,log_type,log_info,log_money,log_time,bill_ref_id,source_account) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}')";

        public string InterestStudentBalanceLogInsert = "INSERT INTO stu_balance_log(log_type,student_id,log_money,log_time,log_way,del_yn,stu_balance_account_id,bill_id,log_info) VALUES ('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}','{8}')";

    }
}
