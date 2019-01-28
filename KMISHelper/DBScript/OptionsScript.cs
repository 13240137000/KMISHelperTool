using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    partial class ScriptService
    {
        public string GetBrandList = "SELECT brand_id,brand_code,brand_name FROM acad_brand WHERE delete_mark = 0";

        public string GetKindergartenList = "SELECT kindergarten_id,kindergarten_nm FROM acad_kindergarten WHERE del_yn = 'N'";

        public string GetBrandUserList = "SELECT uu.user_id,uu.realname FROM upms_role ur INNER JOIN upms_user_role uur ON ur.role_id = uur.role_id INNER JOIN upms_user uu ON uur.user_id = uu.user_id WHERE ur.name in ('ROLE_BRAND','ROLE_GENERALDIRECTOR')";

        public string GetKindergartenUserList = "SELECT uu.user_id,uu.realname FROM upms_role ur INNER JOIN upms_user_role uur ON ur.role_id = uur.role_id INNER JOIN upms_user uu ON uur.user_id = uu.user_id WHERE ur.name in ('ROLE_DIRECTOR','ROLE_KINDE_ADMIN')";
    }
}
