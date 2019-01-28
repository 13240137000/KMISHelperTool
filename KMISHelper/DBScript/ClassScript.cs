using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    public partial class ScriptService
    {
        // Y:acad year K:kindergarten id N:class name

        public string ClassPlanInsert = "INSERT INTO acad_class_plan(class_plan_id,acad_year_id,class_plan_title,class_plan_intro,plan_status,audit_desp,plan_stage,create_by,delete_mark,kindergarten_id,brand_id,create_dts) VALUES ('{0}','{1}','{2}','{3}',1,'同意',1,'{4}',0,'{5}','{6}','{7}')";

        public string GetClassPlanList = "SELECT class_plan_id,class_plan_title FROM acad_class_plan WHERE delete_mark = 0 and kindergarten_id = '{0}'";

        public string ClassInsert = "INSERT INTO acad_class(acad_year_id,class_plan_id,class_id,grade_type,class_type,school_calendar_id,class_nm,class_label,max_capacity,exceed_yn,class_status,teaching_begin_dt,teaching_end_dt,create_id,create_dts,kindergarten_id) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},'{9}','Y','{10}','{11}','{12}','{13}','{14}')";

        public string ClassTempInsert = "INSERT INTO acad_class_temp(acad_year_id,class_plan_id,class_id,grade_type,class_type,school_calendar_id,class_nm,class_label,max_capacity,exceed_yn,class_status,teaching_begin_dt,teaching_end_dt,create_by,create_dts,kindergarten_id) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},'{9}','Y','{10}','{11}','{12}','{13}','{14}')";

        public string ClassPlanLogInsert = "INSERT INTO acad_class_plan_oplog(id,bus_id,task_Name,deal_id,deal_time,app_opinion,app_action,create_time) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";

        public string GetClassList = "SELECT class_id,class_nm FROM acad_class WHERE kindergarten_id = '{0}'";

        public string GetClassListById = "SELECT class_id,class_nm,class_type,school_calendar_id,acad_year_id FROM acad_class WHERE class_id = '{0}'";

        public string GetClassIdByYKN = "SELECT class_id FROM acad_class WHERE acad_year_id = '{0}' AND kindergarten_id = '{1}' AND class_label like '%{2}%' limit 1";

        public string GetClassIDByStudentID = "SELECT sbi.stu_id,sc.class_id FROM stu_base_info sbi LEFT JOIN stu_class sc on sc.stu_id = sbi.stu_id WHERE sbi.stu_no = '{0}' limit 1";

        public string GetTerm = "SELECT term_id,term_begin_date,term_end_date,school_calendar_id FROM acad_school_term WHERE school_calendar_id = '{0}' ORDER BY school_calendar_id";

        public string GetCalInfoByTitle = "SELECT school_calendar_id,school_calendar_title,class_type FROM acad_school_calendar WHERE school_calendar_title = '{0}' limit 1";

        public ScriptService()
        {

        }

    }
}
