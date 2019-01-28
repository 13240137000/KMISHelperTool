using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    partial class ScriptService
    {

        public string getStudentList = "SELECT * FROM acad_student_base_info";

        public string getNationCode = "SELECT nation_code,nation_name from config_nation WHERE nation_name = '{0}'";

        public string StudentInsert = "INSERT INTO stu_base_info(acad_year_id,kindergarten_id,stu_id,stu_real_nm,stu_idcard_type,stu_idcard_no,stu_last_name,stu_first_name,admission_dts,stu_nationality,stu_nation_type,stu_sex,birth_dt,create_dts,stu_status,stu_no,create_id) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')";

        public string ParentInsert = "INSERT INTO stu_family(parent_id,stu_id,parent_nm,mobile_no,relationship,guardian_yn,create_id,education,job,company) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')";

        public string ParentRefInsert = "INSERT INTO stu_family_ref(stu_id,parent_id,guardian_yn) VALUES ('{0}','{1}','{2}')";

        public string PrivacyInsert = "INSERT INTO stu_extend_privacy(stu_id,parent_id,privacy_yn) VALUES ('{0}','{1}','{2}')";

        public string StudentIntoClass = "INSERT INTO stu_class(class_id,stu_id,stu_class_status,create_id,status,create_dts) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')";

        public string StudentBalanceInsert = "INSERT INTO stu_balance(balance_id,student_id,create_id,create_dts) VALUES ('{0}','{1}','{2}','{3}')";

        public string UpdateRelationship = "update stu_family set relationship = '{0}' where stu_id = '{1}' and parent_nm = '{2}'";

    }
}
