using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    public partial class ScriptService
    {
        public string TeacherInsert = "INSERT INTO acad_teacher(teacher_id,kindergarten_id,teacher_real_nm,teacher_last_nm,teacher_first_nm,teacher_sex,teacher_nationality,teacher_nation,teacher_idcard_type,teacher_idcard_no,birth_dt,teacher_mobile_no,email_no,preschool_yn,foreign_staff_yn,teacher_level,education,profession,graduated_school,create_id,create_dts) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')";

        public string UserInsert = "INSERT INTO upms_user(user_id,username,password,salt,realname,phone,email,kindergarten_id,brand_id,locked,sex,login_cnt) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',0,{9},100)";

        public string RoleInsert = "INSERT INTO upms_user_role(user_role_id,user_id,role_id) VALUES ('{0}','{1}','{2}')";

        public string TeacherClassInsert = "INSERT INTO acad_class_staff(class_id,staff_id,staff_type,sort,headmaster_yn) VALUES ('{0}','{1}','{2}',0,'{3}')";
    }
}
